using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;
using System;

namespace MvvmLib.Core.Tests.Message
{

    [TestClass]
    public class WeakDelegateTests
    {
        [TestMethod]
        public void Not_Collect_With_Method()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action)c.MySimpleMethod, false);

            GC.Collect();

            Assert.IsNotNull(w.Target);
        }

        [TestMethod]
        public void Collect_With_Method()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action)c.MySimpleMethod, false);

            c = null;
            GC.Collect();

            Assert.IsNull(w.Target);
        }

        [TestMethod]
        public void KeepAlive()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action)c.MySimpleMethod, true);

            c = null;
            GC.Collect();

            Assert.IsNotNull(w.Target);
        }

        [TestMethod]
        public void Not_Collect_With_Parameterized_Method()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action<string>)c.MyMethod, false);

            GC.Collect();

            Assert.IsNotNull(w.Target);
        }

        [TestMethod]
        public void Collect_With_Parameterized_Method()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action<string>)c.MyMethod, false);

            c = null;
            GC.Collect();

            Assert.IsNull(w.Target);
        }

        [TestMethod]
        public void KeppAlive_With_Parameterized_Method()
        {
            var c = new MyWeakClass();

            var w = new WeakDelegate((Action<string>)c.MyMethod, true);

            c = null;
            GC.Collect();

            Assert.IsNotNull(w.Target);
        }

        [TestMethod]
        public void Work_With_Static_Method()
        {
            var w = new WeakDelegate((Action)MyWeakClass.MyStaticMethod, false);

            Assert.IsNotNull(w.Target);
        }
    }

    public class MyWeakClass
    {
        public void MySimpleMethod()
        {

        }

        public void MyMethod(string myString)
        {

        }

        public static void MyStaticMethod()
        {

        }
    }

}
