using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace ReactiveTests.Tests
{
  public class ObservableTimerTests
  {
    public IObservable<DateTime> CreateObservableTimer()
    {
      return Observable.Create<DateTime>(observer =>
        {
          ElapsedEventHandler elapsed = (s, e) => observer.OnNext(e.SignalTime);

          var timer = new System.Timers.Timer(1000);
          timer.Elapsed += elapsed;
          timer.Start();

          return Disposable.Create(() =>
          {
            timer.Elapsed -= elapsed;
            timer.Dispose();
          });
        });
    }

    [Fact]
    public void ObservableTimerCallsOnNext()
    {
      var observer = new Mock<IObserver<DateTime>>();

      var timer = CreateObservableTimer();
      using (var disposable = timer.Subscribe(observer.Object))
      {
        Thread.Sleep(2000);
      }

      observer.Verify(obs => obs.OnNext(It.IsAny<DateTime>()), Times.AtLeastOnce());
    }
  }
}
