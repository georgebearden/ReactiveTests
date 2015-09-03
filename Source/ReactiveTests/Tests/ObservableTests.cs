using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;

namespace ReactiveTests.Tests
{
  public class ObservableTests
  {
    [Test]
    public void ObservableImplTest()
    {
      var observer = new Mock<IObserver<DateTime>>();

      using (var observable = new ObservableImpl().Subscribe(observer.Object))
        Thread.Sleep(2000);

      observer.Verify(obs => obs.OnNext(It.IsAny<DateTime>()), Times.AtLeastOnce());
    }

    [Test]
    public void ObservableWrapperTest()
    {
      var observer = new Mock<IObserver<DateTime>>();

      using (var observable = new ObservableWrapper().Observable.Subscribe(observer.Object))
        Thread.Sleep(2000);

      observer.Verify(obs => obs.OnNext(It.IsAny<DateTime>()), Times.AtLeastOnce());
    }

    [Test]
    public void ObservableTimerCallsOnNext()
    {
      var observer = new Mock<IObserver<DateTime>>();

      var timer = Observables.CreateObservableTimer( TimeSpan.FromSeconds( 1 ) );
      using ( var _ = timer.Subscribe( observer.Object ) )
      {
        Thread.Sleep( TimeSpan.FromSeconds( 2 ) );
      }

      observer.Verify( obs => obs.OnNext( It.IsAny<DateTime>() ), Times.AtLeastOnce() );
    }

    [Test]
    public void CanConnectToObservableHttpListener()
    {
      string uri = @"http://localhost:8080/";
      var mutex = new Mutex();

      var httpListener = Observables.CreateObservableHttpListener( uri )
        .Subscribe( request =>
        {
          Assert.NotNull( request );
          request.Response.Close();
        } );

      var httpRequest = (HttpWebRequest)WebRequest.Create( uri );
      httpRequest.GetResponse().GetResponseStream();

      httpListener.Dispose();
    }
  }
}
