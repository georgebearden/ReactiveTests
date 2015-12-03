using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveTests
{
  public class CustomObservable : ObservableBase<DateTime>
  {
    protected override IDisposable SubscribeCore(IObserver<DateTime> observer)
    {
      return Observable.Create<DateTime>(async observer =>
      {
        var isRunning = true;

        while (isRunning)
        {
          observer.OnNext(DateTime.Now);
          await Task.Delay(100);
        }

        return Disposable.Create(() => isRunning = false);
      });
    }
  }
}
