using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
  public class SingletonObservableTests
  {
    [Test]
    public void SingletonObservableDoesNotify()
    {
      var observable = new SingletonObservable<DateTime>( (observer) =>
      {
        bool isAlive = true;

        Task.Run( async () =>
          {
            while (isAlive)
            {
              observer.OnNext( DateTime.Now );
              await Task.Delay( 100 );
            }
          } );

        return Disposable.Create( () => isAlive = false );
      } );
    }
  }
}
