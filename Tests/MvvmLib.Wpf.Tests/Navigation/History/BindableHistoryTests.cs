using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{
    [TestClass]
    public class BindableHistoryTests
    {

        //[TestMethod]
        //public void TestRemoveAt()
        //{
        //    var l = new BindableHistory<string>();

        //    l.Add("a");
        //    l.Add("b");
        //    l.Add("c");

        //    Assert.AreEqual("a", l[0]);
        //    Assert.AreEqual("b", l[1]);
        //    Assert.AreEqual("c", l[2]);

        //    l.RemoveAt(0);
        //    Assert.AreEqual(2, l.Count);
        //    Assert.AreEqual("b", l[0]);
        //    Assert.AreEqual("c", l[1]);

        //    l.RemoveAt(0);
        //    Assert.AreEqual(1, l.Count);
        //    Assert.AreEqual("c", l[0]);

        //    l.RemoveAt(0);
        //    Assert.AreEqual(0, l.Count);
        //}


        //[TestMethod]
        //public void TestRemoveAt_2()
        //{
        //    var l = new BindableHistory<string>();

        //    l.Add("a");
        //    l.Add("b");
        //    l.Add("c");

        //    Assert.AreEqual("a", l[0]);
        //    Assert.AreEqual("b", l[1]);
        //    Assert.AreEqual("c", l[2]);

        //    l.RemoveAt(2);
        //    Assert.AreEqual(2, l.Count);
        //    Assert.AreEqual("a", l[0]);
        //    Assert.AreEqual("b", l[1]);


        //    l.RemoveAt(1);
        //    Assert.AreEqual(1, l.Count);
        //    Assert.AreEqual("a", l[0]);

        //    l.RemoveAt(0);
        //    Assert.AreEqual(0, l.Count);
        //}

        //[TestMethod]
        //public void TestClear()
        //{
        //    var l = new BindableHistory<string>();

        //    l.Add("a");
        //    l.Add("b");
        //    l.Add("c");

        //    Assert.AreEqual("a", l[0]);
        //    Assert.AreEqual("b", l[1]);
        //    Assert.AreEqual("c", l[2]);

        //    l.Clear();
        //    Assert.AreEqual(0, l.Count);
        //}

        //[TestMethod]
        //public void Notify_On_Insert()
        //{
        //    bool isNotified = false;
        //    int index = -1;
        //    NavigationEntry notifyEntry = null;

        //    var l = new BindableHistory();
        //    l.EntryAdded += (s, e) =>
        //    {
        //        isNotified = true;
        //        index = e.Index.Value;
        //        notifyEntry = e.Entry;
        //    };

        //    var entry = new NavigationEntry(typeof(ViewA), new ViewA(), "a", null);
        //    l.Add(entry);

        //    Assert.AreEqual(true, isNotified);
        //    Assert.AreEqual(0, index);
        //    Assert.AreEqual(entry, notifyEntry);
        //}

        //[TestMethod]
        //public void Notify_On_Remove()
        //{
        //    bool isNotified = false;
        //    int index = -1;
        //    NavigationEntry notifyEntry = null;

        //    var l = new BindableHistory();

        //    var entry = new NavigationEntry(typeof(ViewA), new ViewA(), "a", null);
        //    l.Add(entry);

        //    l.EntryRemoved += (s, e) =>
        //    {
        //        isNotified = true;
        //        index = e.Index.Value;
        //        notifyEntry = e.Entry;
        //    };

        //    l.RemoveAt(0);

        //    Assert.AreEqual(true, isNotified);
        //    Assert.AreEqual(0, index);
        //    Assert.AreEqual(entry, notifyEntry);
        //}

        [TestMethod]
        public void TestRemoveAt_Index_0()
        {
            var l = new BindableHistory();

            l.Add(new NavigationEntry(typeof(ViewA), new ViewA(), "a", null));
            l.Add(new NavigationEntry(typeof(ViewB), new ViewB(), "b", null));
            l.Add(new NavigationEntry(typeof(ViewC), new ViewC(), "c", null));

            Assert.AreEqual("a", l[0].Parameter);
            Assert.AreEqual("b", l[1].Parameter);
            Assert.AreEqual("c", l[2].Parameter);

            l.RemoveAt(0);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual("b", l[0].Parameter);
            Assert.AreEqual("c", l[1].Parameter);

            l.RemoveAt(0);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual("c", l[0].Parameter);

            l.RemoveAt(0);
            Assert.AreEqual(0, l.Count);
        }


        [TestMethod]
        public void TestRemoveAt_Last()
        {
            var l = new BindableHistory();

            l.Add(new NavigationEntry(typeof(ViewA), new ViewA(), "a", null));
            l.Add(new NavigationEntry(typeof(ViewB), new ViewB(), "b", null));
            l.Add(new NavigationEntry(typeof(ViewC), new ViewC(), "c", null));

            Assert.AreEqual("a", l[0].Parameter);
            Assert.AreEqual("b", l[1].Parameter);
            Assert.AreEqual("c", l[2].Parameter);

            l.RemoveAt(2);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual("a", l[0].Parameter);
            Assert.AreEqual("b", l[1].Parameter);


            l.RemoveAt(1);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual("a", l[0].Parameter);

            l.RemoveAt(0);
            Assert.AreEqual(0, l.Count);
        }

        [TestMethod]
        public void TestClear()
        {
            var l = new BindableHistory();

            l.Add(new NavigationEntry(typeof(ViewA), new ViewA(), "a", null));
            l.Add(new NavigationEntry(typeof(ViewB), new ViewB(), "b", null));
            l.Add(new NavigationEntry(typeof(ViewC), new ViewC(), "c", null));

            Assert.AreEqual("a", l[0].Parameter);
            Assert.AreEqual("b", l[1].Parameter);
            Assert.AreEqual("c", l[2].Parameter);

            l.Clear();
            Assert.AreEqual(0, l.Count);
        }
    }

    public class ViewA
    {

    }

    public class ViewB
    {

    }

    public class ViewC
    {

    }
}
