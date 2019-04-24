using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.IoC.Tests
{

    public class ViewA
    {
        public ViewA()
        {

        }
    }

    public class ViewAWithInjection
    {
        public string MyString { get; }

        public ViewAWithInjection(string myString)
        {
            MyString = myString;
        }
    }



}
