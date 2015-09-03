using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ReactiveTests
{
  public static class Observables
  {
    public static IObservable<DateTime> CreateObservableTimer( TimeSpan interval )
    {
      return Observable.Create<DateTime>( observer =>
      {
        ElapsedEventHandler elapsed = ( s, e ) => observer.OnNext( e.SignalTime );

        var timer = new System.Timers.Timer( interval.TotalMilliseconds );
        timer.Elapsed += elapsed;
        timer.Start();

        return Disposable.Create( () =>
        {
          if ( timer != null )
          {
            timer.Elapsed -= elapsed;
            timer.Dispose();
          }
        } );
      } );
    }

    public static IObservable<HttpListenerContext> CreateObservableHttpListener( params string[] prefixes )
    {
      return Observable.Create<HttpListenerContext>( async observer =>
      {
        var cancelToken = new CancellationTokenSource();

        var server = new HttpListener();
        prefixes.ForEach( server.Prefixes.Add );
        server.Start();

        while ( !cancelToken.IsCancellationRequested )
        {
          var httpContext = await Task.Run( () => server.GetContext(), cancelToken.Token );
          observer.OnNext( httpContext );
        }

        return Disposable.Create( () =>
        {
          if ( !cancelToken.IsCancellationRequested )
            cancelToken.Cancel();

          if ( server != null )
            server.Stop();
        } );
      } );
    }
  }
}
