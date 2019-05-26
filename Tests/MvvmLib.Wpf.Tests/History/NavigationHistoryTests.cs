using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{

    [TestClass]
    public class NavigationHistoryTests
    {
        [TestMethod]
        public void Navigate_Scenario_1()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);
            Assert.AreEqual(2, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.BackStack[1]);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            history = null;
        }

        [TestMethod]
        public void GoBack_And_Navigate_Scenario_2()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);

            Assert.AreEqual(2, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.BackStack[1]);
            Assert.AreEqual(eC, history.Current); //
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            // move back

            history.GoBack();
            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(1, history.ForwardStack.Count);
            Assert.AreEqual(eC, history.ForwardStack[0]);

            history.GoBack();
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(eB, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(2, history.ForwardStack.Count);
            Assert.AreEqual(eC, history.ForwardStack[0]);
            Assert.AreEqual(eB, history.ForwardStack[1]);

            // navigate clear
            var viewD = new ViewD();
            var contextD = new ViewDViewModel();
            var eD = new NavigationEntry(typeof(ViewD), viewD, "D", contextD);
            history.Navigate(eD);
            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eD, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            history = null;
        }

        [TestMethod]
        public void GoForward()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);
            history.GoBack();
            history.GoBack();
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(eB, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(2, history.ForwardStack.Count);
            Assert.AreEqual(eC, history.ForwardStack[0]);
            Assert.AreEqual(eB, history.ForwardStack[1]);

            // A <= B C
            // ...A => B  C
            history.GoForward();
            Assert.AreEqual(1, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(1, history.ForwardStack.Count);
            Assert.AreEqual(eC, history.ForwardStack[0]);

            // ...A  B => C
            history.GoForward();
            Assert.AreEqual(2, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.BackStack[1]);
            Assert.AreEqual(eC, history.Current); // 
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            history = null;
        }

        [TestMethod]
        public void NavigateToRoot()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);
            Assert.AreEqual(2, history.BackStack.Count);
            Assert.AreEqual(eA, history.BackStack[0]);
            Assert.AreEqual(eB, history.BackStack[1]);
            Assert.AreEqual(eC, history.Current); //
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            history.NavigateToRoot();
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            history = null;
        }

        [TestMethod]
        public void Notify_When_Can_GoBackChange()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            int count = 0;
            CanGoBackEventArgs ev = null;
            history.CanGoBackChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            Assert.AreEqual(0, count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            Assert.AreEqual(0, count); // cannot go back
            Assert.AreEqual(0, count);

            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            Assert.AreEqual(1, count); // can go back (backstack count 1)
            Assert.AreEqual(true, ev.CanGoBack);

            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);
            Assert.AreEqual(1, count);

            history.GoBack(); // B (backstack count 1)
            Assert.AreEqual(1, count);

            history.GoBack(); // A (backstack count 0) cannot go back
            Assert.AreEqual(2, count);
            Assert.AreEqual(false, ev.CanGoBack);

            history.GoForward();
            Assert.AreEqual(3, count); // can go back
            Assert.AreEqual(true, ev.CanGoBack);

            history = null;
        }

        [TestMethod]
        public void Notify_When_Can_GoForwardChange()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            int count = 0;
            CanGoForwardEventArgs ev = null;
            history.CanGoForwardChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            Assert.AreEqual(0, count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);
            Assert.AreEqual(0, count);
            Assert.AreEqual(null, ev);

            // [A...B]..C <=
            // [A]..B..[C]
            history.GoBack();
            Assert.AreEqual(1, count);
            Assert.AreEqual(true, ev.CanGoForward);

            // A..[B..C]
            history.GoBack();
            Assert.AreEqual(1, count);

            // [A]..B..[C]
            history.GoForward();
            Assert.AreEqual(1, count);

            // [A...B]..C
            history.GoForward();
            Assert.AreEqual(2, count);
            Assert.AreEqual(false, ev.CanGoForward);

            history = null;
        }

        [TestMethod]
        public void Notify_On_Current_Change()
        {
            var history = new NavigationHistory();

            int count = 0;
            int countPropertyChanged = 0;
            CurrentEntryChangedEventArgs ev = null;
            history.CurrentChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            history.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Current")
                    countPropertyChanged++;
            };
            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, countPropertyChanged);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            Assert.AreEqual(1, count);
            Assert.AreEqual(1, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);

            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            Assert.AreEqual(2, count);
            Assert.AreEqual(2, countPropertyChanged);
            Assert.AreEqual(eB, ev.CurrentEntry);

            history.GoBack();
            Assert.AreEqual(3, count);
            Assert.AreEqual(3, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);

            history.GoForward();
            Assert.AreEqual(4, count);
            Assert.AreEqual(4, countPropertyChanged);
            Assert.AreEqual(eB, ev.CurrentEntry);

            history.NavigateToRoot();
            Assert.AreEqual(5, count);
            Assert.AreEqual(5, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);
        }

        [TestMethod]
        public void Notify_On_Clear()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.BackStack.Count);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, history.ForwardStack.Count);

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            history.Navigate(eA);
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            history.Navigate(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            history.Navigate(eC);

            int countEvCanGoBack = 0;
            int countEvCanGoForward = 0;
            CanGoBackEventArgs evCanGoBack = null;
            CanGoForwardEventArgs evCanGoForward = null;
            history.CanGoBackChanged += (s, e) =>
            {
                countEvCanGoBack++;
                evCanGoBack = e;
            };
            history.CanGoForwardChanged += (s, e) =>
            {
                countEvCanGoForward++;
                evCanGoForward = e;
            };
            int countCurrentChanged = 0;
            int countPropertyChanged = 0;
            CurrentEntryChangedEventArgs evCurrentChanged = null;
            history.CurrentChanged += (s, e) =>
            {
                countCurrentChanged++;
                evCurrentChanged = e;
            };
            history.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Current")
                    countPropertyChanged++;
            };

            Assert.AreEqual(0, countEvCanGoBack);
            Assert.AreEqual(0, countEvCanGoForward);
            Assert.AreEqual(0, countCurrentChanged);
            Assert.AreEqual(0, countPropertyChanged);

            history.Clear();
            Assert.AreEqual(1, countEvCanGoBack);
            Assert.AreEqual(false, evCanGoBack.CanGoBack);
            Assert.AreEqual(1, countEvCanGoForward);
            Assert.AreEqual(false, evCanGoForward.CanGoForward);
            Assert.AreEqual(1, countCurrentChanged);
            Assert.AreEqual(1, countPropertyChanged);
            Assert.AreEqual(null, evCurrentChanged.CurrentEntry);

            history = null;
        }

        [TestMethod]
        public void NavigateToRoot_Throw()
        {
            var history = new NavigationHistory();
            bool success = true;
            try
            {
                history.NavigateToRoot();
            }
            catch (Exception ex)
            {
                success = false;
            }

            Assert.AreEqual(false, success);
        }

        [TestMethod]
        public void GoBack_Throw()
        {
            var history = new NavigationHistory();
            bool success = true;
            try
            {
                history.GoBack();
            }
            catch (Exception ex)
            {
                success = false;
            }

            Assert.AreEqual(false, success);
        }

        [TestMethod]
        public void GoForward_Throw()
        {
            var history = new NavigationHistory();
            bool success = true;
            try
            {
                history.GoForward();
            }
            catch (Exception ex)
            {
                success = false;
            }

            Assert.AreEqual(false, success);
        }
    }
}
