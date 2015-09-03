using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
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

        var timer = new Timer( interval.TotalMilliseconds );
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
  }
}
