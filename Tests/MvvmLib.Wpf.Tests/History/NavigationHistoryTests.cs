using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.History;
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

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history = null;
        }

        [TestMethod]
        public void GoBack_And_Navigate_Scenario_2()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();           
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);

            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eC, history.Current); //
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            // move back

            history.GoBack(null);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eB, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);

            history.GoBack(null);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(eB, history.Next);
            Assert.AreEqual(eA, history.Root);

            // navigate clear
            var viewD = new ViewD();
            var eD = new NavigationEntry(typeof(ViewD), viewD, "D");
            history.Navigate(eD);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eD, history.Entries.ElementAt(1));
            Assert.AreEqual(eD, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history = null;
        }

        [TestMethod]
        public void Change_Parameter_On_GoBack_GoForward_Move_And_NavigateToRoot()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);

            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Current); //
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
            Assert.AreEqual("A", eA.Parameter);
            Assert.AreEqual("B", eB.Parameter);
            Assert.AreEqual("A", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual("B", history.Entries.ElementAt(1).Parameter);

            // back
            history.GoBack("B!");
            Assert.AreEqual("A", eA.Parameter);
            Assert.AreEqual("B!", eB.Parameter);
            Assert.AreEqual("C", eC.Parameter);
            Assert.AreEqual("A", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual("B!", history.Entries.ElementAt(1).Parameter);
            Assert.AreEqual("C", history.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);

            // forward
            history.GoForward("C!");
            Assert.AreEqual("A", eA.Parameter);
            Assert.AreEqual("B!", eB.Parameter);
            Assert.AreEqual("A", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual("B!", history.Entries.ElementAt(1).Parameter);
            Assert.AreEqual("C!", history.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            // move 
            history.MoveTo(eA, "A!!");
            Assert.AreEqual("A!!", eA.Parameter);
            Assert.AreEqual("B!", eB.Parameter);
            Assert.AreEqual("C!", eC.Parameter);
            Assert.AreEqual("A!!", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual("B!", history.Entries.ElementAt(1).Parameter);
            Assert.AreEqual("C!", history.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(eB, history.Next);
            Assert.AreEqual(eA, history.Root);

            // move index
            history.MoveTo(2, "C!!");
            Assert.AreEqual("A!!", eA.Parameter);
            Assert.AreEqual("B!", eB.Parameter);
            Assert.AreEqual("C!!", eC.Parameter);
            Assert.AreEqual("A!!", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual("B!", history.Entries.ElementAt(1).Parameter);
            Assert.AreEqual("C!!", history.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);


            // root
            history.NavigateToRoot("A!!!");
            Assert.AreEqual("A!!!", eA.Parameter);
            Assert.AreEqual("A!!!", history.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);
        }

        [TestMethod]
        public void GoForward()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            history.GoBack(null);
            history.GoBack(null);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(eB, history.Next);
            Assert.AreEqual(eA, history.Root);

            // A <= B C
            // ...A => B  C
            history.GoForward(null);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eB, history.Current); // 
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);

            // ...A  B => C
            history.GoForward(null);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eC, history.Current); // 
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history = null;
        }

        [TestMethod]
        public void NavigateToRoot()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eC, history.Current); //
            Assert.AreEqual(eB, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history.NavigateToRoot(null);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eA, history.Current); // 
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history = null;
        }

        [TestMethod]
        public void MoveTo()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            var viewD = new ViewD();
            var eD = new NavigationEntry(typeof(ViewD), viewD, "D");
            history.Navigate(eD);
            var viewE = new ViewE();
            var eE = new NavigationEntry(typeof(ViewE), viewE, "E");
            history.Navigate(eE);

            //  [A B C D] E

            Assert.AreEqual(5, history.Entries.Count);
            Assert.AreEqual(4, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eD, history.Entries.ElementAt(3));
            Assert.AreEqual(eE, history.Entries.ElementAt(4));
            Assert.AreEqual(eE, history.Current);
            Assert.AreEqual(eD, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(eA, history.Root);

            history.MoveTo(eB, null);
            // [A] B [C D E]
            Assert.AreEqual(5, history.Entries.Count);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eD, history.Entries.ElementAt(3));
            Assert.AreEqual(eE, history.Entries.ElementAt(4));
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA, history.Previous);
            Assert.AreEqual(eC, history.Next);
            Assert.AreEqual(eA, history.Root);

            history.MoveTo(eD, null);
            // [A] B [C D E]

            Assert.AreEqual(5, history.Entries.Count);
            Assert.AreEqual(3, history.CurrentIndex);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(eD, history.Entries.ElementAt(3));
            Assert.AreEqual(eE, history.Entries.ElementAt(4));
            Assert.AreEqual(eD, history.Current);
            Assert.AreEqual(eC, history.Previous);
            Assert.AreEqual(eE, history.Next);
            Assert.AreEqual(eA, history.Root);

            history = null;
        }

        [TestMethod]
        public void Notify_When_Can_GoBackChange()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            int count = 0;
            CanGoBackEventArgs ev = null;
            history.CanGoBackChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            Assert.AreEqual(0, count);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            Assert.AreEqual(0, count); // cannot go back
            Assert.AreEqual(0, count);

            var viewB = new ViewB();
            
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            Assert.AreEqual(1, count); // can go back (Entries count 1)
            Assert.AreEqual(true, ev.CanGoBack);

            var viewC = new ViewC();
            
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            Assert.AreEqual(1, count);

            history.GoBack(null); // B (Entries count 1)
            Assert.AreEqual(1, count);

            history.GoBack(null); // A (Entries count 0) cannot go back
            Assert.AreEqual(2, count);
            Assert.AreEqual(false, ev.CanGoBack);

            history.GoForward(null);
            Assert.AreEqual(3, count); // can go back
            Assert.AreEqual(true, ev.CanGoBack);

            history = null;
        }

        [TestMethod]
        public void Notify_When_Can_GoForwardChange()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            int count = 0;
            CanGoForwardEventArgs ev = null;
            history.CanGoForwardChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            Assert.AreEqual(0, count);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
            history.Navigate(eC);
            Assert.AreEqual(0, count);
            Assert.AreEqual(null, ev);

            // [A...B]..C <=
            // [A]..B..[C]
            history.GoBack(null);
            Assert.AreEqual(1, count);
            Assert.AreEqual(true, ev.CanGoForward);

            // A..[B..C]
            history.GoBack(null);
            Assert.AreEqual(1, count);

            // [A]..B..[C]
            history.GoForward(null);
            Assert.AreEqual(1, count);

            // [A...B]..C
            history.GoForward(null);
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
            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, countPropertyChanged);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            Assert.AreEqual(1, count);
            Assert.AreEqual(1, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);

            var viewB = new ViewB();
            
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            Assert.AreEqual(2, count);
            Assert.AreEqual(2, countPropertyChanged);
            Assert.AreEqual(eB, ev.CurrentEntry);

            history.GoBack(null);
            Assert.AreEqual(3, count);
            Assert.AreEqual(3, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);

            history.GoForward(null);
            Assert.AreEqual(4, count);
            Assert.AreEqual(4, countPropertyChanged);
            Assert.AreEqual(eB, ev.CurrentEntry);

            history.NavigateToRoot(null);
            Assert.AreEqual(5, count);
            Assert.AreEqual(5, countPropertyChanged);
            Assert.AreEqual(eA, ev.CurrentEntry);
        }

        [TestMethod]
        public void Notify_On_Clear()
        {
            var history = new NavigationHistory();

            Assert.AreEqual(0, history.Entries.Count);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(null, history.Current);
            Assert.AreEqual(null, history.Previous);
            Assert.AreEqual(null, history.Next);
            Assert.AreEqual(null, history.Root);

            var viewA = new ViewA();
            
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A");
            history.Navigate(eA);
            var viewB = new ViewB();
            
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B");
            history.Navigate(eB);
            var viewC = new ViewC();
            
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C");
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
                history.NavigateToRoot(null);
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
                history.GoBack(null);
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
                history.GoForward(null);
            }
            catch (Exception ex)
            {
                success = false;
            }

            Assert.AreEqual(false, success);
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

    public class ViewD
    {

    }

    public class ViewE
    {

    }

    public class ViewF
    {

    }

    public class ViewG
    {

    }

    public class ViewAViewModel
    {

    }

    public class ViewBViewModel
    {

    }

    public class ViewCViewModel
    {

    }

    public class ViewDViewModel
    {

    }

    public class ViewEViewModel
    {

    }

    public class ViewFViewModel
    {

    }

    public class ViewGViewModel
    {

    }
}
