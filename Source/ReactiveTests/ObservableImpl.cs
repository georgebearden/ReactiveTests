using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ReactiveTests
{
  public class ObservableImpl : IObservable<DateTime>
  {
    private readonly Subject<DateTime> _subject = new Subject<DateTime>();
    private readonly Timer _timer = new Timer(1000);

    public ObservableImpl()
    {
      _timer.Elapsed += OnTick;
      _timer.Start();
    }

    private void OnTick(object sender, ElapsedEventArgs e)
    {
      _subject.OnNext(e.SignalTime);
    }

    public IDisposable Subscribe(IObserver<DateTime> observer)
    {
      return _subject.Subscribe(observer);
    }
  }
}
