using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleSharedServices
{
    public interface IMySharedService
    {
        string GetMessage(string name);
    }

    public class MySharedService : IMySharedService
    {
        public string GetMessage(string name)
        {
            return $"Hello {name}! (shared service)";
        }
    }
}
