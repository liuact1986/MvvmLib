using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows.Controls;
using MvvmLib.Navigation;
using System.Linq;

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
            Assert.AreEqual("home", ((Page)history.BackStack.ElementAt(0).ViewOrObject).name);
            Assert.AreEqual("page 1", ((Page)history.BackStack.ElementAt(1).ViewOrObject).name);
            var current = history.Current;
            Assert.AreEqual(typeof(Page), current.SourceType);
            Assert.AreEqual("page 2", ((Page)current.ViewOrObject).name);
            Assert.AreEqual("my value 3", current.Parameter);

            // page 2 (current) is push to forward
            // page 1 is current
            // backstack 1
            var entry = history.GoBack();
            Assert.AreEqual("page 1", ((Page)entry.ViewOrObject).name);
            Assert.AreEqual("my value 2", entry.Parameter);

            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(1, history.ForwardStack.Count);

            var lastBack = history.Previous;
            Assert.AreEqual("home", ((Page)lastBack.ViewOrObject).name);
            Assert.AreEqual("my value", lastBack.Parameter);

            var lastForward = history.Next;
            Assert.AreEqual("page 2", ((Page)lastForward.ViewOrObject).name);
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

        [TestMethod]
        public void IsNotified_OnCanGoBackChanged()
        {
            var h = new NavigationHistory();

            bool isNotified = false;
            h.CanGoBackChanged += (s, e) =>
            {
                isNotified = true;
            };

            // navigate

            AddHome(h);
            Assert.AreEqual(false, isNotified);

            AddPage1(h);
            Assert.AreEqual(true, isNotified);

            // go back
            isNotified = false;
            h.GoBack();
            Assert.AreEqual(true, isNotified);

            // go forward
            isNotified = false;
            h.GoForward();
            Assert.AreEqual(true, isNotified);

            h = null;
        }

        [TestMethod]
        public void IsNotified_OnCanGoForwardChanged()
        {
            var h = new NavigationHistory();

            bool isNotified = false;
            h.CanGoForwardChanged += (s, e) =>
            {
                isNotified = true;
            };

            AddHome(h);
            AddPage1(h);
            AddPage2(h);
            Assert.AreEqual(false, isNotified);

            // go back
            h.GoBack();
            Assert.AreEqual(true, isNotified);

            // go forward
            isNotified = false;
            h.GoForward();
            Assert.AreEqual(true, isNotified);

            h = null;
        }

        [TestMethod]
        public void Scenario1()
        {
            var h = new NavigationHistory();
            // navigate
            // Home => page A (home added to back stack)

            var home = new HomePage();
            var eHome = new NavigationEntry(typeof(HomePage), home, "pH", home.DataContext);
            h.Navigate(eHome);

            Assert.AreEqual(null, h.Previous);
            Assert.AreEqual(eHome, h.Current);
            Assert.AreEqual(null, h.Next);
            Assert.AreEqual(eHome, h.Root);

            // 2
            var pageA = new PageA();
            var ePageA = new NavigationEntry(typeof(PageA), pageA, "pA", pageA.DataContext);
            h.Navigate(ePageA);

            Assert.AreEqual(eHome, h.Previous);
            Assert.AreEqual(1, h.BackStack.Count);
            Assert.AreEqual(eHome, h.BackStack[0]);
            Assert.AreEqual(ePageA, h.Current);
            Assert.AreEqual(null, h.Next);
            Assert.AreEqual(eHome, h.Root);

            // back
            // Home (current) (back stack empty) <= page A add to forward stack
            h.GoBack();

            Assert.AreEqual(null, h.Previous);
            Assert.AreEqual(0, h.BackStack.Count);
            Assert.AreEqual(eHome, h.Current);
            Assert.AreEqual(ePageA, h.Next);
            Assert.AreEqual(1, h.ForwardStack.Count);
            Assert.AreEqual(ePageA, h.ForwardStack[0]);
            Assert.AreEqual(eHome, h.Root);

            // forward
            // home (added to back stack) => page A (current)
            h.GoForward();
            Assert.AreEqual(eHome, h.Previous);
            Assert.AreEqual(1, h.BackStack.Count);
            Assert.AreEqual(ePageA, h.Current);
            Assert.AreEqual(0, h.ForwardStack.Count);
            Assert.AreEqual(eHome, h.Root);

            // page b, page c
            var pageB = new PageB();
            var ePageB = new NavigationEntry(typeof(PageB), pageB, "pB", pageB.DataContext);
            h.Navigate(ePageB);

            var pageC = new PageC();
            var ePageC = new NavigationEntry(typeof(PageC), pageB, "pC", pageC.DataContext);
            h.Navigate(ePageC);

            Assert.AreEqual(ePageC, h.Current); // page c
            Assert.AreEqual(3, h.BackStack.Count); // home, page a, page b
            Assert.AreEqual(eHome, h.BackStack[0]);
            Assert.AreEqual(ePageA, h.BackStack[1]);
            Assert.AreEqual(ePageB, h.BackStack[2]);
            Assert.AreEqual(0, h.ForwardStack.Count); // 0
            Assert.AreEqual(ePageB, h.Previous);
            Assert.AreEqual(null, h.Next);
            Assert.AreEqual(eHome, h.Root);

            h.GoBack();

            Assert.AreEqual(ePageB, h.Current); // page b
            Assert.AreEqual(2, h.BackStack.Count); // home, page a
            Assert.AreEqual(eHome, h.BackStack[0]);
            Assert.AreEqual(ePageA, h.BackStack[1]);
            Assert.AreEqual(1, h.ForwardStack.Count); // page c
            Assert.AreEqual(ePageC, h.ForwardStack[0]);
            Assert.AreEqual(ePageA, h.Previous);
            Assert.AreEqual(ePageC, h.Next);
            Assert.AreEqual(eHome, h.Root);

            h.GoBack();

            Assert.AreEqual(ePageA, h.Current); // page a
            Assert.AreEqual(1, h.BackStack.Count); // home
            Assert.AreEqual(eHome, h.BackStack[0]);
            Assert.AreEqual(2, h.ForwardStack.Count); // page c, page b
            Assert.AreEqual(ePageC, h.ForwardStack[0]);
            Assert.AreEqual(ePageB, h.ForwardStack[1]);
            Assert.AreEqual(eHome, h.Previous);
            Assert.AreEqual(ePageB, h.Next);
            Assert.AreEqual(eHome, h.Root);

            h.GoBack();

            Assert.AreEqual(eHome, h.Current); // home
            Assert.AreEqual(0, h.BackStack.Count); // 0
            Assert.AreEqual(3, h.ForwardStack.Count); // page c, page b, page a
            Assert.AreEqual(ePageC, h.ForwardStack[0]);
            Assert.AreEqual(ePageB, h.ForwardStack[1]);
            Assert.AreEqual(ePageA, h.ForwardStack[2]);
            Assert.AreEqual(null, h.Previous);
            Assert.AreEqual(ePageA, h.Next);
            Assert.AreEqual(eHome, h.Root);


            // navigate clear forward stack
            var pageX = new PageX();
            var ePageX = new NavigationEntry(typeof(PageX), pageX, "pX", pageX.DataContext);
            h.Navigate(ePageX);
            Assert.AreEqual(ePageX, h.Current); // home
            Assert.AreEqual(0, h.ForwardStack.Count);
            Assert.AreEqual(1, h.BackStack.Count); // 0
            Assert.AreEqual(eHome, h.BackStack[0]);
            Assert.AreEqual(eHome, h.Previous);
            Assert.AreEqual(null, h.Next);
            Assert.AreEqual(eHome, h.Root);
        }
    }

    public class HomePage : Page
    {

    }

    public class PageA : Page
    {

    }

    public class PageB : Page
    {

    }

    public class PageC : Page
    {

    }


    public class PageX : Page
    {

    }

    //public class MyPage : Page
    //{
    //    public string MyText { get; set; }

    //    public MyPage(string myText)
    //    {
    //        this.MyText = myText;
    //    }
    //}
}
