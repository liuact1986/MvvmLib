using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{
    [TestClass]
    public class ListHistoryTest
    {
        [TestMethod]
        public void Insert()
        {
            var history = new ListHistory();
            var page = new Page("home");
            var parameter = "my value";
            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Insert(0, n);

            Assert.AreEqual(1, history.List.Count);
            Assert.IsNotNull(history.Current);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(typeof(Page), history.Current.SourceType);
            Assert.AreSame(page, history.Current.ViewOrObject);
            Assert.AreSame("home", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Current.Parameter);

            var page2 = new Page("page 1");
            var parameter2 = "my value 2";
            var n2 = new NavigationEntry(typeof(Page), page2, parameter2, null);
            history.Insert(1, n2);

            Assert.AreEqual(2, history.List.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreSame(page2, history.Current.ViewOrObject);
            Assert.AreSame("page 1", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter2, history.Current.Parameter);

            var page3 = new Page("page 2");
            var parameter3 = "my value 3";
            var n3 = new NavigationEntry(typeof(Page), page3, parameter3, null);
            history.Insert(2, n3);

            Assert.AreEqual(3, history.List.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreSame(page3, history.Current.ViewOrObject);
            Assert.AreSame("page 2", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter3, history.Current.Parameter);

            // previous
            Assert.AreSame(page2, history.Previous.ViewOrObject);
            Assert.AreSame("page 1", ((Page)history.Previous.ViewOrObject).name);
            Assert.AreEqual(parameter2, history.Previous.Parameter);

            // root
            Assert.AreSame(page, history.Root.ViewOrObject);
            Assert.AreSame("home", ((Page)history.Root.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Root.Parameter);
        }

        [TestMethod]
        public void Remove()
        {
            var history = new ListHistory();
            var page = new Page("home");
            var parameter = "my value";
            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Insert(0, n);

            var page2 = new Page("page 1");
            var parameter2 = "my value 2";
            var n2 = new NavigationEntry(typeof(Page), page2, parameter2, null);
            history.Insert(1, n2); // last => index = list.count

            // home
            // page 1

            history.RemoveAt(1); // => home

            Assert.AreEqual(1, history.List.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.IsNotNull(history.Current);
            Assert.AreEqual(typeof(Page), history.Current.SourceType);
            Assert.AreSame(page, history.Current.ViewOrObject);
            Assert.AreSame("home", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Current.Parameter);
        }

    }
}
