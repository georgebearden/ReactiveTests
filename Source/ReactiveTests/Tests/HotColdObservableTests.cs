using NUnit.Framework;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveTests.Tests
{
  public class HotColdObservableTests
  {
    /// <summary>
    /// A cold observable will create a new instance of the observable sequence
    /// from the specified subscribe method implementation each time it is called.
    /// </summary>
    [Test]
    public void CreateColdObservable()
    {
      int creationCount = 0;

      var coldObservable = Observable.Create<Unit>(_ =>
      {
        creationCount++;
        return Disposable.Empty;
      });

      coldObservable.Subscribe(_ => { }).Dispose();
      Assert.AreEqual(1, creationCount);

      coldObservable.Subscribe(_ => { }).Dispose();
      Assert.AreEqual(2, creationCount);
    }

    /// <summary>
    /// A hot observable will return the same instance of the observable sequence
    /// from the specified subscribe method implementation 
    /// </summary>
    [Test]
    public void CreateHotObservable()
    {
      int creationCount = 0;
      var coldObservable = Observable.Create<Unit>(_ =>
      {
        creationCount++;
        return Disposable.Empty;
      });

      var hotObservable = coldObservable.Publish();
      var hotDisposable = hotObservable.Connect();

      hotObservable.Subscribe(_ => { }).Dispose();
      Assert.AreEqual(1, creationCount);

      hotObservable.Subscribe(_ => { }).Dispose();
      Assert.AreEqual(1, creationCount);
    }

    [Test]
    public void ObservableCreateIsLazy()
    {
      var isCreated = false;
      var coldObservable = Observable.Create<Unit>(_ =>
      {
        isCreated = true;
        return Disposable.Empty;
      });

      Assert.False(isCreated);
    }

    [Test]
    public void CallingConnectOnConnectableObservableCreatesTheObservableSequence()
    {
      var isCreated = false;
      var coldObservable = Observable.Create<Unit>(_ =>
      {
        isCreated = true;
        return Disposable.Empty;
      });
      // Observable.Create should be lazy
      Assert.False(isCreated);

      var hotObservable = coldObservable.Publish();
      // Publish makes the observable sequence share the subscription but still does not instantiate it
      Assert.False(isCreated);

      var hotDisposable = hotObservable.Connect();
      // Connect actually instantiates the subscription.
      Assert.True(isCreated);
    }
  }
}
