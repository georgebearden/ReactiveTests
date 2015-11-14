using NUnit.Framework;
using System;
using System.Net;
using System.Reactive.Linq;

namespace ReactiveTests.Tests
{
  public class HttpServerTests
  {
    [Test]
    public void CanStartHttpServer()
    {
      var httpServer = new HttpServer("http://127.0.0.1:8888/");

      var requests = httpServer.Subscribe(httpContext =>
      {
        Console.WriteLine("got request");
      });

      var request = WebRequest.Create("http://127.0.0.1:8888/");
    }
  }
}
