using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveTests
{
  public class SingletonObservable<T> : IObservable<T>, IDisposable
  {
    private readonly Func<IObserver<T>, IDisposable> _impl;
    private readonly CancellationTokenSource _cancelToken;
    private readonly Lazy<Subject<T>> _subject;

    public SingletonObservable( Func<IObserver<T>, IDisposable> impl )
    {
      _impl = impl;
      _cancelToken = new CancellationTokenSource();

      _subject = new Lazy<Subject<T>>( () =>
      {
        var subject = new Subject<T>();
        //_impl();
        return subject;
      } );
    }

    public void Dispose()
    {
      if ( !_cancelToken.IsCancellationRequested )
        _cancelToken.Cancel();
    }

    public IDisposable Subscribe( IObserver<T> observer )
    {
      return _subject.Value.Subscribe( observer );
    }
  }
}
