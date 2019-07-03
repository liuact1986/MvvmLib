using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleB.Services
{
    public interface IMyService
    {
        string GetMessage(string name);
    }

    public class MyService : IMyService
    {
        private string defaultMessage;
        public string DefaultMessage
        {
            get { return defaultMessage; }
            set { defaultMessage = value; }
        }

        public MyService()
        {
            DefaultMessage = "Hello";
        }

        public string GetMessage(string name)
        {
            return $"Hello {name}!";
        }
    }
}
