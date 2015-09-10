using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace ReactiveTests.Tests
{
  public class ObservableTests
  {
    [Test]
    public void ObservableCreateIsLazy()
    {
      int observableInstanceCount = 0;

      var observable = Observable.Create<DateTime>( async observer =>
      {
        observableInstanceCount++;
        bool isRunning = true;

        while ( isRunning )
        {
          observer.OnNext( DateTime.Now );
          await Task.Delay( 100 );
        }

        return Disposable.Create( () => isRunning = false );
      } );

      // observableInstanceCount should not have been incremented as nothing has subscribed yet
      Assert.AreEqual( 0, observableInstanceCount );

      var subscription = observable.Subscribe( _ => { } );
      Assert.AreEqual( 1, observableInstanceCount );
    }

    [Test]
    public void DoesObservableCreateReturnsSameInstance()
    {
      int observableInstanceCount = 0;

      var observable = Observable.Create<DateTime>( async observer =>
        {
          observableInstanceCount++;
          bool isRunning = true;

          while (isRunning)
          {
            observer.OnNext( DateTime.Now );
            await Task.Delay( 100 );
          }

          return Disposable.Create( () => isRunning = false );
        } );

      var subscription1 = observable.Subscribe( _ => { } );
      Assert.AreEqual( 1, observableInstanceCount );

      var subscription2 = observable.Subscribe( _ => { } );
      Assert.AreEqual( 2, observableInstanceCount );
    }

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

    [Test]
    public void CanWatchSocketState()
    {
      var socketStateSubscription = Observables.CreateSocketStateObservable(
        "localhost", 5555, SocketType.Stream, ProtocolType.Tcp )
        .Subscribe( state =>
        {
          Assert.AreEqual( SocketStates.Up, state );
        } );

      var tcpServer = new TcpListener( IPAddress.Parse( "127.0.0.1" ), 5555 );
      tcpServer.Start();

      tcpServer.Stop();
      socketStateSubscription.Dispose();
    }
  }
}
