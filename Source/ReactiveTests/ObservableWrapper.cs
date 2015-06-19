using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ReactiveTests
{
  public class ObservableWrapper
  {
    private readonly Subject<DateTime> _subject = new Subject<DateTime>();
    private readonly Timer _timer = new Timer(1000);

    public ObservableWrapper()
    {
      _timer.Elapsed += OnTick;
      _timer.Start();
    }

    void OnTick(object sender, ElapsedEventArgs e)
    {
      _subject.OnNext(e.SignalTime);
    }

    public IObservable<DateTime> Observable { get { return _subject; } }
  }
}
