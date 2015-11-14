using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveTests
{
  public class HttpServer : IObservable<HttpListenerContext>, IDisposable
  {
    private readonly IConnectableObservable<HttpListenerContext> _httpRequests;
    private readonly IDisposable _httpRequestsConnection;

    public HttpServer(params string[] prefixes)
    {
      _httpRequests = Observable.Create<HttpListenerContext>(async observer =>
      {
        var cancelToken = new CancellationTokenSource();

        var server = new HttpListener();
        foreach (var prefix in prefixes)
          server.Prefixes.Add(prefix);

        server.Start();

        while (!cancelToken.IsCancellationRequested)
        {
          var request = await server.GetContextAsync();
          observer.OnNext(request);
        }

        return Disposable.Create(() =>
        {
          if (!cancelToken.IsCancellationRequested)
            cancelToken.Cancel();

          if (server.IsListening)
            server.Close();
        });
      }).Publish();

      _httpRequestsConnection = _httpRequests.Connect();
    }

    public IDisposable Subscribe(IObserver<HttpListenerContext> observer)
    {
      return _httpRequests.Subscribe(observer);
    }

    public void Dispose()
    {
      _httpRequestsConnection.Dispose();
    }
  }
}
