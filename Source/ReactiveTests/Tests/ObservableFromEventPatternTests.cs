using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveTests.Tests
{
  public class ObservableFromEventPatternTests
  {
    public class Foo
    {
      public event EventHandler FooHappened;

      public void RaiseFooHappened()
      {
        if (FooHappened != null)
          FooHappened(this, EventArgs.Empty);
      }
    }

    public class Mouse
    {
      public event EventHandler<PointEventArgs> PointChanged;

      public void RaisePointChanged(PointEventArgs e)
      {
        if (PointChanged != null)
          PointChanged(this, e);
      }
    }

    public class PointEventArgs : EventArgs
    {
      public PointEventArgs(double x, double y)
      {
        X = x;
        Y = y;
      }

      public double X { get; private set; }
      public double Y { get; private set;}
    }

    [Fact]
    public void TestObservableFromEventPattern()
    {
      var foo = new Foo();

      var disposable = Observable.FromEventPattern(
        e => foo.FooHappened += e,
        e => foo.FooHappened -= e)
        .Subscribe(args =>
        {
          // when foo.FooHappened is raised, this action is called with the param args.  Args is an EventPattern 
          // class that contains both the sender and the event args for the event being raised.

          Assert.Equal(EventArgs.Empty, args.EventArgs);
          Assert.ReferenceEquals(foo, args.Sender);
        });

      foo.RaiseFooHappened();
      disposable.Dispose();
    }

    [Fact]
    public void TestObservableFromEventPatternWithObjectAndString()
    {
      var foo = new Foo();

      var disposable = Observable.FromEventPattern(foo, "FooHappened")
        .Subscribe(args =>
        {
          // when foo.FooHappened is raised, this action is called with the param args.  Args is an EventPattern 
          // class that contains both the sender and the event args for the event being raised.

          Assert.Equal(EventArgs.Empty, args.EventArgs);
          Assert.ReferenceEquals(foo, args.Sender);
        });

      foo.RaiseFooHappened();
      disposable.Dispose();
    }

    [Fact]
    public void TestMockMouseMoveObservableFromEventPattern()
    {
      var mouse = new Mouse();
      var timesEventCalled = 0;

      var disposable = Observable.FromEventPattern<PointEventArgs>(
        e => mouse.PointChanged += e,
        e => mouse.PointChanged -= e)
        .Subscribe(args =>
        {
          Assert.ReferenceEquals(mouse, args.Sender);
          Assert.True(args.EventArgs.X != 0 && args.EventArgs.Y != 0);
          timesEventCalled++;
        });

      // Mimic a simple mouse movemet by raising a series of consecutive "mouse move" events
      // and then make sure observable's OnNext method was called the correct number of times.
      new[]
      {
        new PointEventArgs(1, 1), 
        new PointEventArgs(1, 2),
        new PointEventArgs(2, 1),
        new PointEventArgs(2,2 )
      }.ForEach(e => mouse.RaisePointChanged(e));

      Assert.Equal(4, timesEventCalled);
      disposable.Dispose();
    }
  }
}
