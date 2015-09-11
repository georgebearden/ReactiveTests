using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
  public class PerformanceTests
  {
    public class SingletonObservable : IObservable<DateTime>, IDisposable
    {
      private readonly Subject<DateTime> _subject;
      private readonly CancellationTokenSource _cancelToken;

      public SingletonObservable()
      {
        _subject = new Subject<DateTime>();
        _cancelToken = new CancellationTokenSource();

        Task.Run( async () =>
          {
            while ( !_cancelToken.IsCancellationRequested )
            {
              _subject.OnNext( DateTime.Now );
              await Task.Delay( 1 );
            }
          } );
      }

      public void Dispose()
      {
        if ( !_cancelToken.IsCancellationRequested )
          _cancelToken.Cancel();
      }

      public IDisposable Subscribe( IObserver<DateTime> observer )
      {
        return _subject.Subscribe( observer );
      }
    }

    [Test]
    public void SingletonObservbleTest()
    {
      var singletonObservable = new SingletonObservable();
      var disposables = new HashSet<IDisposable>();
      int nextCount = 0;

      for (int subscriberCount = 0; subscriberCount < 10000; subscriberCount++)
      {
        var subscriber = singletonObservable.Subscribe( _ => { nextCount++; } );
        disposables.Add( subscriber );
      }

      Thread.Sleep( 5000 );

      foreach ( var disposable in disposables )
        disposable.Dispose();
    }

    [Test]
    public void TransientObservableTest()
    {
      var transientObservable = Observable.Create<DateTime>( observer =>
        {
          var cancelToken = new CancellationTokenSource();

          Task.Run( async () =>
          {
            while ( !cancelToken.IsCancellationRequested )
            {
              observer.OnNext( DateTime.Now );
              await Task.Delay( 1 );
            }
          } );

          return Disposable.Create( () =>
            {
              if ( !cancelToken.IsCancellationRequested )
                cancelToken.Cancel();
            } );
        } );

      var disposables = new HashSet<IDisposable>();
      int nextCount = 0;

      for ( int subscriberCount = 0; subscriberCount < 10000; subscriberCount++ )
      {
        var subscriber = transientObservable.Subscribe( _ => { nextCount++; } );
        disposables.Add( subscriber );
      }

      Thread.Sleep( 5000 );

      foreach ( var disposable in disposables )
        disposable.Dispose();
    }
  }
}
