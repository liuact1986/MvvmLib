using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests
{
    [TestClass]
    public class BindableListTests
    {

        [TestMethod]
        public void TestRemoveAt()
        {
            var l = new BindableList<string>();

            l.Add("a");
            l.Add("b");
            l.Add("c");

            Assert.AreEqual("a", l[0]);
            Assert.AreEqual("b", l[1]);
            Assert.AreEqual("c", l[2]);

            l.RemoveAt(0);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual("b", l[0]);
            Assert.AreEqual("c", l[1]);

            l.RemoveAt(0);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual("c", l[0]);

            l.RemoveAt(0);
            Assert.AreEqual(0, l.Count);
        }


        [TestMethod]
        public void TestRemoveAt_2()
        {
            var l = new BindableList<string>();

            l.Add("a");
            l.Add("b");
            l.Add("c");

            Assert.AreEqual("a", l[0]);
            Assert.AreEqual("b", l[1]);
            Assert.AreEqual("c", l[2]);

            l.RemoveAt(2);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual("a", l[0]);
            Assert.AreEqual("b", l[1]);


            l.RemoveAt(1);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual("a", l[0]);

            l.RemoveAt(0);
            Assert.AreEqual(0, l.Count);
        }

        [TestMethod]
        public void TestClear()
        {
            var l = new BindableList<string>();

            l.Add("a");
            l.Add("b");
            l.Add("c");

            Assert.AreEqual("a", l[0]);
            Assert.AreEqual("b", l[1]);
            Assert.AreEqual("c", l[2]);

            l.Clear();
            Assert.AreEqual(0, l.Count);
        }
    }
}
