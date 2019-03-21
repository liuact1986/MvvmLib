using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmLib.IoC.Tests
{
    public interface IMyService
    {
        string MyProp { get; set; }
        string Message { get; set; }
        string GetMessage();
    }

    public class MyService : IMyService
    {
        public string MyProp { get; set; } = "MyProp";
        public string Message { get; set; } = "Hello world!";

        public string GetMessage()
        {
            return Message;
        }
    }

    public interface IMyService2
    {
        string MyProp { get; set; }
    }

    public class MyService2 : IMyService2
    {
        public string MyProp { get; set; } = "MyProp 2";
    }
}
