using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows.Controls;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{

    public class Page : ContentControl
    {
        public string name;

        public Page() { }
        public Page(string name)
        {
            this.name = name;
        }
    }

    [TestClass]
    public class NavigationHistoryTest
    {
        public void AddHome(NavigationHistory history)
        {
            var page = new Page("home");
            var parameter = "my value";

            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);
        }

        public void AddPage1(NavigationHistory history)
        {
            var page = new Page("page 1");
            var parameter = "my value 2";

            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);
        }

        public void AddPage2(NavigationHistory history)
        {
            var page = new Page("page 2");
            var parameter = "my value 3";

            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);
        }

        [TestMethod]
        public void Navigate_SetCurrent()
        {
            var history = new NavigationHistory();
            var page = new Page("home");
            var parameter = "my value";

            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);

            Assert.IsNotNull(history.Current);
            Assert.AreEqual(typeof(Page), history.Current.SourceType);
            Assert.AreSame(page, history.Current.ViewOrObject);
            Assert.AreSame("home", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Current.Parameter);
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(0, history.ForwardStack.Count);
        }

        [TestMethod]
        public void Navigate_Push_Current_To_BackStack_Then_SetCurrent()
        {
            var history = new NavigationHistory();

            AddHome(history);

            var page = new Page("page 1");
            var parameter = "my value 2";

            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);

            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(0, history.ForwardStack.Count);

            // last
            var entry = history.Previous;
            Assert.AreEqual(typeof(Page), entry.SourceType);
            Assert.AreSame("home", ((Page)entry.ViewOrObject).name);
            Assert.AreEqual("my value", entry.Parameter);

            // current
            Assert.IsNotNull(history.Current);
            Assert.AreEqual(typeof(Page), history.Current.SourceType);
            Assert.AreSame("page 1", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Current.Parameter);
        }

        [TestMethod]
        public void TestGoBack()
        {
            var history = new NavigationHistory();

            AddHome(history);
            AddPage1(history); // backstack home and page 1
            AddPage2(history); // current

            Assert.AreEqual(2, history.BackStack.Count);

            var current = history.Current;
            Assert.AreEqual(typeof(Page), current.SourceType);
            Assert.AreSame("page 2", ((Page)current.ViewOrObject).name);
            Assert.AreEqual("my value 3", current.Parameter);

            // page 2 (current) is push to forward
            // page 1 is current
            // backstack 1
            var entry = history.GoBack();
            Assert.AreSame("page 1", ((Page)entry.ViewOrObject).name);
            Assert.AreEqual("my value 2", entry.Parameter);

            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(1, history.ForwardStack.Count);

            var lastBack = history.Previous;
            Assert.AreSame("home", ((Page)lastBack.ViewOrObject).name);
            Assert.AreEqual("my value", lastBack.Parameter);

            var lastForward = history.Next;
            Assert.AreSame("page 2", ((Page)lastForward.ViewOrObject).name);
            Assert.AreEqual("my value 3", lastForward.Parameter);
        }

        [TestMethod]
        public void Testnavigate_ClearForwardStack()
        {
            var history = new NavigationHistory();

            AddHome(history);
            AddPage1(history); // backstack home and page 1
            AddPage2(history); // current

            history.GoBack();
            history.GoBack();

            Assert.AreEqual(2, history.ForwardStack.Count);

            var page = new Page("page 3");
            var parameter = "my value 4";
            var n = new NavigationEntry(typeof(Page), page, parameter, null);
            history.Navigate(n);

            Assert.AreEqual(0, history.ForwardStack.Count);

            Assert.AreEqual("page 3", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual(parameter, history.Current.Parameter);
        }

        [TestMethod]
        public void NavigateToRoot()
        {
            var history = new NavigationHistory();

            AddHome(history);
            AddPage1(history); // backstack home and page 1
            AddPage2(history); // current


            history.NavigateToRoot();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(0, history.ForwardStack.Count);


            Assert.AreEqual("home", ((Page)history.Current.ViewOrObject).name);
            Assert.AreEqual("my value", history.Current.Parameter);
        }
    }
}
