using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveTests.Tests
{
  public class ObservableTests
  {
    [Fact]
    public void ObservableImplTest()
    {
      var observer = new Mock<IObserver<DateTime>>();

      using (var observable = new ObservableImpl().Subscribe(observer.Object))
        Thread.Sleep(2000);

      observer.Verify(obs => obs.OnNext(It.IsAny<DateTime>()), Times.AtLeastOnce());
    }

    [Fact]
    public void ObservableWrapperTest()
    {
      var observer = new Mock<IObserver<DateTime>>();

      using (var observable = new ObservableWrapper().Observable.Subscribe(observer.Object))
        Thread.Sleep(2000);

      observer.Verify(obs => obs.OnNext(It.IsAny<DateTime>()), Times.AtLeastOnce());
    }
  }
}
