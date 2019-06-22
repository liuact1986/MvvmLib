using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using MvvmLib.Wpf.Tests.ViewModels;
using MvvmLib.Wpf.Tests.Views;

namespace MvvmLib.Wpf.Tests.Navigation
{
    // "navigate" navigate/ fast/ redirect => selectable or new + (on navigating from) [on navigating/ed to new]
    // "Move" back / forward / move / root => (on navigating from) +  on navigating/ed to

    [TestClass]
    public class NavigationSourceTests
    {
        [TestMethod]
        public void AddNewSource()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(NavSourceViewAViewModel), ((FrameworkElement)sourceA).DataContext.GetType());

            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(NavSourceViewBViewModel), ((FrameworkElement)sourceB).DataContext.GetType());
        }

        [TestMethod]
        public void InsertNewSource_ResolveViewModel_With_ViewModelLocator()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.InsertNewSource(0, typeof(MyViewA), "p");
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(MyViewAViewModel), ((FrameworkElement)sourceA).DataContext.GetType());
        }

        [TestMethod]
        public void InsertSource_Manage_CurrentIndex_And_CurrentChangedEvent_Is_Not_Fired()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");

            navigationSource.MoveTo(0);
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(sourceA, navigationSource.Current); // current is A

            bool isCurrentChangedFired = false;
            navigationSource.CurrentChanged += (s, e) =>
            {
                isCurrentChangedFired = true;
            };

            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");

            Assert.AreEqual(0, navigationSource.CurrentIndex); // not change
            Assert.AreEqual(sourceA, navigationSource.Current);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));

            var sourceC = navigationSource.InsertNewSource(0, typeof(NavSourceViewC), "p3");
            Assert.AreEqual(1, navigationSource.CurrentIndex); // incremented
            Assert.AreEqual(sourceA, navigationSource.Current);
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(2));

            var sourceD = navigationSource.InsertNewSource(1, typeof(NavSourceViewD), "p4");
            Assert.AreEqual(2, navigationSource.CurrentIndex);
            Assert.AreEqual(sourceA, navigationSource.Current);
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceD, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(3));
            Assert.AreEqual(false, isCurrentChangedFired);
        }


        [TestMethod]
        public void InsertNewSource()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.InsertNewSource(0, typeof(NavSourceViewA), "p");
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(NavSourceViewAViewModel), ((FrameworkElement)sourceA).DataContext.GetType());

            var sourceB = navigationSource.InsertNewSource(1, typeof(NavSourceViewB), "p2");
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(NavSourceViewBViewModel), ((FrameworkElement)sourceB).DataContext.GetType());

            var sourceC = navigationSource.InsertNewSource(1, typeof(NavSourceViewC), "p3");
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // do not select
            Assert.AreEqual(typeof(NavSourceViewCViewModel), ((FrameworkElement)sourceC).DataContext.GetType());
        }

        [TestMethod]
        public void RemoveSourceAt()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(2).Parameter);

            // remove at 1
            navigationSource.RemoveSourceAt(1); // B
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(1).Parameter);

            // remove first
            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(0).Parameter);

            // remove last
            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void RemoveSource()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(2).Parameter);

            // remove at 1
            Assert.AreEqual(true, navigationSource.RemoveSource(sourceB)); // B
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(1).Parameter);

            // remove first
            Assert.AreEqual(true, navigationSource.RemoveSource(sourceA));
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(0).Parameter);

            // remove last
            Assert.AreEqual(true, navigationSource.RemoveSource(sourceC));
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void RemoveSource_Manage_CurrentIndex_And_CurrentChangedEvent_Is_Not_Fired()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(2).Parameter);

            navigationSource.MoveTo(1);
            Assert.AreEqual(1, navigationSource.CurrentIndex);
            Assert.AreEqual(sourceB, navigationSource.Current); // current is B

            bool isCurrentChangedFired = false;
            CurrentSourceChangedEventArgs args = null;
            navigationSource.CurrentChanged += (s, e) =>
            {
                args = e;
                isCurrentChangedFired = true;
            };

            // remove at 2
            navigationSource.RemoveSourceAt(2); // C
            Assert.AreEqual(1, navigationSource.CurrentIndex); // not change current index < removed index
            Assert.AreEqual(sourceB, navigationSource.Current);
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);

            // remove first
            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // decremented
            Assert.AreEqual(sourceB, navigationSource.Current);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(false, isCurrentChangedFired);

            // remove last
            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(null, navigationSource.Current);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(true, isCurrentChangedFired);
            Assert.AreEqual(-1, args.CurrentIndex);
            Assert.AreEqual(null, args.Current);
        }

        [TestMethod]
        public void RemoveRange()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            var sourceD = navigationSource.AddNewSource(typeof(NavSourceViewD), "p4");
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(4, navigationSource.Sources.Count);
            Assert.AreEqual(4, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(sourceD, navigationSource.Sources.ElementAt(3));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(typeof(NavSourceViewD), navigationSource.Entries.ElementAt(3).SourceType);
            Assert.AreEqual("p4", navigationSource.Entries.ElementAt(3).Parameter);

            navigationSource.MoveTo(1);
            Assert.AreEqual(1, navigationSource.CurrentIndex);
            Assert.AreEqual(sourceB, navigationSource.Current); // current is B

            // remove at 2
            navigationSource.RemoveSources(2); // C and d
            Assert.AreEqual(1, navigationSource.CurrentIndex); // not change current index < removed index
            Assert.AreEqual(sourceB, navigationSource.Current);
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);

            navigationSource.RemoveSources(-1); // do nothing
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);

            navigationSource.RemoveSources(1); // B
            Assert.AreEqual(0, navigationSource.CurrentIndex); // decremented
            Assert.AreEqual(sourceA, navigationSource.Current);// changed to A
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);

            navigationSource.RemoveSources(0);
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // decremented
            Assert.AreEqual(null, navigationSource.Current);// changed to null
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void CheckCanGoBack_On_Move()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");
            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            var sourceD = navigationSource.AddNewSource(typeof(NavSourceViewD), "p4");
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(4, navigationSource.Sources.Count);
            Assert.AreEqual(4, navigationSource.Entries.Count);
            Assert.AreEqual(sourceA, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(sourceB, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(sourceC, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(sourceD, navigationSource.Sources.ElementAt(3));
            Assert.AreEqual(typeof(NavSourceViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(NavSourceViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(NavSourceViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(typeof(NavSourceViewD), navigationSource.Entries.ElementAt(3).SourceType);
            Assert.AreEqual("p4", navigationSource.Entries.ElementAt(3).Parameter);

            Assert.AreEqual(0, firedCount);

            navigationSource.MoveTo(1);

            Assert.AreEqual(1, firedCount);
            Assert.AreEqual(true, states[0]);

            navigationSource.MoveTo(2);
            Assert.AreEqual(1, firedCount);
            Assert.AreEqual(true, states[0]);

            navigationSource.MoveTo(1);
            Assert.AreEqual(1, firedCount);
            Assert.AreEqual(true, states[0]);

            navigationSource.MoveTo(0);
            Assert.AreEqual(2, firedCount);
            Assert.AreEqual(false, states[1]);

            navigationSource.MoveTo(2);
            Assert.AreEqual(3, firedCount);
            Assert.AreEqual(true, states[2]);

            navigationSource.MoveTo(0);
            Assert.AreEqual(4, firedCount);
            Assert.AreEqual(false, states[3]);
        }

        [TestMethod]
        public void CheckCanGoBack_On_Insert_And_Remove()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");

            navigationSource.MoveTo(0);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            Assert.AreEqual(0, firedCount);

            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2"); // A [B]
            Assert.AreEqual(0, firedCount); // not change
            Assert.AreEqual(false, navigationSource.CanGoBack);

            var sourceC = navigationSource.InsertNewSource(0, typeof(NavSourceViewC), "p3");
            Assert.AreEqual(1, firedCount); // change [C] A [B]
            Assert.AreEqual(true, states[0]);
            Assert.AreEqual(true, navigationSource.CanGoBack);

            var sourceD = navigationSource.InsertNewSource(0, typeof(NavSourceViewD), "p4");
            Assert.AreEqual(1, firedCount); // not change [D C] A [B]

            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(1, firedCount); // not change [C] A [B]

            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(2, firedCount); // change A [B]
            Assert.AreEqual(false, states[1]);
            Assert.AreEqual(false, navigationSource.CanGoBack);

            navigationSource.RemoveSourceAt(1);
            Assert.AreEqual(2, firedCount); // not change A

            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(2, firedCount); // not change null

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoBack_On_Clear_Not_Fired_If_Not_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");

            navigationSource.MoveTo(0);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.ClearSources();
            Assert.AreEqual(0, firedCount); // not change
            Assert.AreEqual(false, navigationSource.CanGoBack);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoBack_On_Clear_Fired_If_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2"); // A [B]

            navigationSource.MoveTo(1);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            Assert.AreEqual(0, firedCount);
            Assert.AreEqual(true, navigationSource.CanGoBack);

            navigationSource.ClearSources();
            Assert.AreEqual(1, firedCount); // change
            Assert.AreEqual(false, states[0]);
            Assert.AreEqual(false, navigationSource.CanGoBack);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoBack_On_RemoveRange_Not_Fired_If_Not_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");

            navigationSource.MoveTo(0);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.RemoveSources(0);
            Assert.AreEqual(0, firedCount); // not change
            Assert.AreEqual(false, navigationSource.CanGoBack);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoBack_On_RemoveRange_Fired_If_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2"); // A [B]

            navigationSource.MoveTo(1);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoBackChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoBack);
            };

            Assert.AreEqual(0, firedCount);
            Assert.AreEqual(true, navigationSource.CanGoBack);

            navigationSource.RemoveSources(0);
            Assert.AreEqual(1, firedCount); // change
            Assert.AreEqual(false, states[0]);
            Assert.AreEqual(false, navigationSource.CanGoBack);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoForward_On_Insert_And_Remove()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(false, navigationSource.CanGoForward);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            //Assert.AreEqual(false, navigationSource.CanGoForward);

            navigationSource.MoveTo(0);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoForwardChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoForward);
            };

            Assert.AreEqual(0, firedCount);

            var sourceB = navigationSource.InsertNewSource(0, typeof(NavSourceViewB), "p2");
            Assert.AreEqual(0, firedCount); // not change [B] A
            Assert.AreEqual(false, navigationSource.CanGoForward);

            var sourceC = navigationSource.AddNewSource(typeof(NavSourceViewC), "p3");
            Assert.AreEqual(1, firedCount); // change [B] A [C]
            Assert.AreEqual(true, states[0]);
            Assert.AreEqual(true, navigationSource.CanGoForward);

            var sourceD = navigationSource.AddNewSource(typeof(NavSourceViewD), "p4");
            Assert.AreEqual(1, firedCount); // not change [B] A [C D]

            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(1, firedCount); // not change A [C D]

            navigationSource.RemoveSourceAt(2);
            Assert.AreEqual(1, firedCount); // not change A [C]

            navigationSource.RemoveSourceAt(1);
            Assert.AreEqual(2, firedCount); // change A
            Assert.AreEqual(false, states[1]);
            Assert.AreEqual(false, navigationSource.CanGoForward);

            navigationSource.RemoveSourceAt(0);
            Assert.AreEqual(2, firedCount); // not change null

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoForward_On_Clear_Not_Fired_If_Not_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");

            navigationSource.MoveTo(1);
            Assert.AreEqual(false, navigationSource.CanGoForward);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoForwardChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoForward);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.ClearSources();
            Assert.AreEqual(0, firedCount); // not change
            Assert.AreEqual(false, navigationSource.CanGoForward);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoForward_On_Clear_Fired_If_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");

            navigationSource.MoveTo(0);
            Assert.AreEqual(true, navigationSource.CanGoForward);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoForwardChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoForward);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.ClearSources();
            Assert.AreEqual(1, firedCount); 
            Assert.AreEqual(false, states[0]);
            Assert.AreEqual(false, navigationSource.CanGoForward);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoForward_On_RemoveRange_Not_Fired_If_Not_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");

            navigationSource.MoveTo(1);
            Assert.AreEqual(false, navigationSource.CanGoForward);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoForwardChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoForward);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.RemoveSources(0);
            Assert.AreEqual(0, firedCount); // not change
            Assert.AreEqual(false, navigationSource.CanGoForward);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void CheckCanGoForward_On_RemoveRange_Fired_If_Change()
        {
            var navigationSource = new NavigationSource();

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            var sourceA = navigationSource.AddNewSource(typeof(NavSourceViewA), "p");
            var sourceB = navigationSource.AddNewSource(typeof(NavSourceViewB), "p2");

            navigationSource.MoveTo(0);
            Assert.AreEqual(true, navigationSource.CanGoForward);

            int firedCount = 0;
            List<bool> states = new List<bool>();
            navigationSource.CanGoForwardChanged += (s, e) =>
            {
                firedCount += 1;
                states.Add(e.CanGoForward);
            };

            Assert.AreEqual(0, firedCount);

            navigationSource.RemoveSources(0);
            Assert.AreEqual(1, firedCount); // not change
            Assert.AreEqual(false, states[0]);
            Assert.AreEqual(false, navigationSource.CanGoForward);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
        }

        // can go back => current index > 0
        // can go forward => currentIndex < this.sources.Count - 1

        [TestMethod]
        public void Navigate_With_Source_Name()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();

            SourceResolver.RegisterTypeForNavigation<MyViewModelNavigationAwareAndGuardsStatic>("A");

            navigationSource.Navigate("A");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual(null, navigationSource.Entries.ElementAt(0).Parameter);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void Navigate_With_Source_Name_And_Parameter()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();

            SourceResolver.RegisterTypeForNavigation<MyViewModelNavigationAwareAndGuardsStatic>("B");

            navigationSource.Navigate("B", "p1");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p1", navigationSource.Entries.ElementAt(0).Parameter);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void Navigate_With_ViewModel()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;

            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic), "p");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic), "p2");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(0).Parameter);

            // can deactivate
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p3");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p2", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(1, navigationSource.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p4");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            var nextSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(nextSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic2), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p4", navigationSource.Entries.ElementAt(1).Parameter);
        }

        [TestMethod]
        public void Navigate_With_View()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CActivate = false;

            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;
            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p2");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p2", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p3");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p3", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(0).Parameter);

            // can deactivate
            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p3");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(0).Parameter);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p4");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("p3", navigationSource.Entries.ElementAt(0).Parameter);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p5");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual("p5", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual("p5", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            var nextSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(nextSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic2), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("p5", navigationSource.Entries.ElementAt(1).Parameter);
        }

        [TestMethod]
        public void SideNav()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(navigationSource.Current, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("A", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());

            navigationSource.Navigate(typeof(MyNavViewB), "B");
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(navigationSource.Current, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("B", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            navigationSource.Navigate(typeof(MyNavViewC), "C");
            var currentSource = navigationSource.Current;
            Assert.AreEqual(2, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("C", navigationSource.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            // go back C => B
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewB.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelB.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual("B", MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual("B", MyNavViewModelB.POnNavigatedTo);
            var prevSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(prevSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("B", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());

            // go forward B => C
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewB.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelB.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual("C", MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual("C", MyNavViewModelC.POnNavigatedTo);
            var s2 = navigationSource.Current;
            Assert.AreEqual(2, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s2, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual("C", navigationSource.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            // move C => A
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewA.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelA.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual("A", MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual("A", MyNavViewModelA.POnNavigatedTo);
            var s3 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s3, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("A", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());
        }

        [TestMethod]
        public void Navigate_Clear_Entries_And_Sources_After_CurrentIndex()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.GoBack();
            navigationSource.GoBack();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s1, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 2).GetType());
            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("A", navigationSource.Entries.ElementAt(0).Parameter);

            navigationSource.Navigate(typeof(MyNavViewD), "D");
            // [A] D
            var s2 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyNavViewD), s2.GetType());
            Assert.AreEqual(s2, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(2, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewD), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("D", navigationSource.Entries.ElementAt(1).Parameter);
        }

        [TestMethod]
        public void NavigateToRoot()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.NavigateToRoot();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(s1, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(1, navigationSource.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("A", navigationSource.Entries.ElementAt(0).Parameter);
        }

        [TestMethod]
        public void Clear()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.ClearSources();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(null, navigationSource.Current);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Redirect()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");

            navigationSource.Redirect(typeof(MyViewModelRedirect), "B");
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("B", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(1, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Redirect_With_Entries_Before()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            navigationSource.Redirect(typeof(MyViewModelRedirect), "C");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("C", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(2, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Redirect_With_Source_Name()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            SourceResolver.RegisterTypeForNavigation<MyViewModelRedirect>("Y");

            navigationSource.Redirect("Y");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual(null, navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(2, navigationSource.Entries.Count);

            SourceResolver.ClearTypesForNavigation();
        }


        [TestMethod]
        public void Redirect_With_Source_Name_And_Parameter()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            SourceResolver.RegisterTypeForNavigation<MyViewModelRedirect>("Z");

            navigationSource.Redirect("Z", "C");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual("C", navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(2, navigationSource.Entries.Count);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void MoveTo_With_Source()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.Entries.Count);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");


            navigationSource.MoveTo(navigationSource.Sources.First());

            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Current.GetType());
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual("A", navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(3, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Sync_With_History()
        {
            var navigationSource = new NavigationSource();
            var history = new NavigationEntryCollection();
            var eA = new NavigationEntry(typeof(MyNavViewA), "A");
            var eB = new NavigationEntry(typeof(MyNavViewB), "B");
            var eC = new NavigationEntry(typeof(MyNavViewC), "C");
            history.Add(eA);
            history.Add(eB);
            history.Add(eC);
            Assert.AreEqual(3, history.Count);
            Assert.AreEqual(eA, history.ElementAt(0));
            Assert.AreEqual(eB, history.ElementAt(1));
            Assert.AreEqual(eC, history.ElementAt(2));

            var sources = new List<object> { new MyNavViewA(), new MyNavViewB(), new MyNavViewC() };

            navigationSource.Sync(history, sources, 1);

            Assert.AreEqual(3, navigationSource.Entries.Count);
            Assert.AreEqual(eA.SourceType, navigationSource.Entries.ElementAt(0).SourceType);
            Assert.AreEqual(eA.Parameter, navigationSource.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(eB.SourceType, navigationSource.Entries.ElementAt(1).SourceType);
            Assert.AreEqual(eB.Parameter, navigationSource.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(eC.SourceType, navigationSource.Entries.ElementAt(2).SourceType);
            Assert.AreEqual(eC.Parameter, navigationSource.Entries.ElementAt(2).Parameter);

            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(eA.SourceType, navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(eB.SourceType, navigationSource.Sources.ElementAt(1).GetType());
            Assert.AreEqual(eC.SourceType, navigationSource.Sources.ElementAt(2).GetType());

            Assert.AreEqual(1, navigationSource.CurrentIndex);
        }

        [TestMethod]
        public void DoNotClear_WithClearSources_OnNavigate_False()
        {
            var navigationSource = new NavigationSource();
            navigationSource.ClearSourcesOnNavigate = false;

            navigationSource.Navigate(typeof(NavSourceViewA), "A");
            navigationSource.Navigate(typeof(NavSourceViewB), "B");
            navigationSource.Navigate(typeof(NavSourceViewC), "C");
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(3, navigationSource.Entries.Count);

            navigationSource.MoveTo(0);
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            navigationSource.Navigate(typeof(NavSourceViewD), "D");

            Assert.AreEqual(4, navigationSource.Sources.Count);
            Assert.AreEqual(4, navigationSource.Entries.Count);

            navigationSource.MoveTo(3);
            Assert.AreEqual(3, navigationSource.CurrentIndex);

            navigationSource.NavigateToRoot();
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(4, navigationSource.Sources.Count);
            Assert.AreEqual(4, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Clear_WithClearSources_OnNavigate_True()
        {
            var navigationSource = new NavigationSource();
            navigationSource.ClearSourcesOnNavigate = true;

            navigationSource.Navigate(typeof(NavSourceViewA), "A");
            navigationSource.Navigate(typeof(NavSourceViewB), "B");
            navigationSource.Navigate(typeof(NavSourceViewC), "C");
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(3, navigationSource.Entries.Count);

            navigationSource.MoveTo(0);
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            navigationSource.Navigate(typeof(NavSourceViewD), "D");

            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(2, navigationSource.Entries.Count);

            navigationSource.Navigate(typeof(NavSourceViewA), "A");
            navigationSource.Navigate(typeof(NavSourceViewB), "B");
            navigationSource.Navigate(typeof(NavSourceViewC), "C");

            navigationSource.MoveTo(3);
            Assert.AreEqual(3, navigationSource.CurrentIndex);

            navigationSource.NavigateToRoot();
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.Entries.Count);
        }

        [TestMethod]
        public void Commands()
        {
            var navigationSource = new NavigationSource();

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.NavigateToRootCommand.CanExecute(null);

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelA));
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);
            navigationSource.NavigateToRootCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelB));
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoBackCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoForwardCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.RedirectCommand.Execute(typeof(MyNavViewModelC));
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatedToInvoked);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.NavigateToRootCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);
        }

        [TestMethod]
        public void Navigate_Updates_Parameter()
        {
            var navigationSource = new NavigationSource();

            MyViewModelThatChangeParameter.Parameter = null;

            navigationSource.Navigate(typeof(MyViewThatChangeParameter), "p");

            Assert.AreEqual("p-canactivateview--canactivateviewmodel--onavigatingtoviewmodel--navigatedtoviewmodel-", MyViewModelThatChangeParameter.Parameter);

            MyViewModelThatChangeParameter.Parameter = null;

            navigationSource.Navigate(typeof(MySimpleViewModel), "p2");

            Assert.AreEqual("p2-candeactivateview--candeactivateviewmodel--onavigatingfromviewmodel-", MyViewModelThatChangeParameter.Parameter);
        }
    }

    public class MyViewModelRedirect
    {

    }

    public class MyNavViewD : UserControl { }

    public class MyNavViewA : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewA()
        {
            this.DataContext = new MyNavViewModelA();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyNavViewB : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewB()
        {
            this.DataContext = new MyNavViewModelB();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyNavViewC : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewC()
        {
            this.DataContext = new MyNavViewModelC();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyNavViewModelA : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyNavViewModelB : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyNavViewModelC : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuardsStatic2 : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }


    public class NavSourceViewA : UserControl
    {
        public NavSourceViewA()
        {
            DataContext = new NavSourceViewAViewModel();
        }
    }

    public class NavSourceViewAViewModel
    {

    }

    public class NavSourceViewB : UserControl
    {
        public NavSourceViewB()
        {
            DataContext = new NavSourceViewBViewModel();
        }
    }

    public class NavSourceViewBViewModel
    {

    }

    public class NavSourceViewC : UserControl
    {
        public NavSourceViewC()
        {
            DataContext = new NavSourceViewCViewModel();
        }
    }

    public class NavSourceViewCViewModel
    {

    }

    public class NavSourceViewD : UserControl
    {
        public NavSourceViewD()
        {
            DataContext = new NavSourceViewDViewModel();
        }
    }

    public class NavSourceViewDViewModel
    {

    }
}
