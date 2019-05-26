using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{
    [TestClass]
    public class NavigationEntryCollectionTests
    {
        [TestMethod]
        public void InsertItem_Insert_And_Notify_Changes()
        {
            var collection = new NavigationEntryCollection();
            int count = 0;
            int countString = 0;
            NotifyCollectionChangedEventArgs ev = null;
            collection.CollectionChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            collection.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Count")
                    countString++;
            };

            Assert.AreEqual(0, collection.Count);

            // Add index 0
            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            collection.Add(eA);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(1, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, ev.Action);
            Assert.AreEqual(0, ev.NewStartingIndex);
            Assert.AreEqual(1, ev.NewItems.Count);
            Assert.AreEqual(eA, ev.NewItems[0]);
            Assert.AreEqual(-1, ev.OldStartingIndex);
            Assert.AreEqual(1, countString);

            // Add index 1
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            collection.Add(eB);
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eB, collection[1]);
            Assert.AreEqual(2, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, ev.Action);
            Assert.AreEqual(1, ev.NewStartingIndex);
            Assert.AreEqual(1, ev.NewItems.Count);
            Assert.AreEqual(eB, ev.NewItems[0]);
            Assert.AreEqual(-1, ev.OldStartingIndex);
            Assert.AreEqual(2, countString);

            // insert index 1
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            collection.Insert(1, eC);
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eC, collection[1]);
            Assert.AreEqual(eB, collection[2]);
            Assert.AreEqual(3, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, ev.Action);
            Assert.AreEqual(1, ev.NewStartingIndex);
            Assert.AreEqual(1, ev.NewItems.Count);
            Assert.AreEqual(eC, ev.NewItems[0]);
            Assert.AreEqual(-1, ev.OldStartingIndex);
            Assert.AreEqual(3, countString);

            collection = null;
        }

        [TestMethod]
        public void SetItem_Set_And_Notify_Changes()
        {
            var collection = new NavigationEntryCollection();
            int count = 0;
            NotifyCollectionChangedEventArgs ev = null;

            // Add index 0
            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            collection.Add(eA);
            // Add index 1
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            collection.Add(eB);

            collection.CollectionChanged += (s, e) =>
            {
                count++;
                ev = e;
            };

            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            collection[1] = eC;

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eC, collection[1]);
            Assert.AreEqual(1, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, ev.Action);
            Assert.AreEqual(1, ev.NewStartingIndex);
            Assert.AreEqual(1, ev.NewItems.Count);
            Assert.AreEqual(eC, ev.NewItems[0]);
            Assert.AreEqual(1, ev.OldStartingIndex);
            Assert.AreEqual(eB, ev.OldItems[0]);

            collection = null;
        }

        [TestMethod]
        public void RemoveItem_Remove_And_Notify_Changes()
        {
            var collection = new NavigationEntryCollection();
            int count = 0;
            int countString = 0;
            NotifyCollectionChangedEventArgs ev = null;

            // Add index 0
            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            collection.Add(eA);
            // Add index 1
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            collection.Add(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            collection.Add(eC);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eB, collection[1]);
            Assert.AreEqual(eC, collection[2]);

            collection.CollectionChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            collection.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Count")
                    countString++;
            };

            // remove last
            collection.RemoveAt(2);
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eB, collection[1]);
            Assert.AreEqual(1, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, ev.Action);
            Assert.AreEqual(-1, ev.NewStartingIndex);
            Assert.AreEqual(null, ev.NewItems);
            Assert.AreEqual(2, ev.OldStartingIndex);
            Assert.AreEqual(eC, ev.OldItems[0]);
            Assert.AreEqual(1, countString);

            // remove first
            collection.RemoveAt(0);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(eB, collection[0]);
            Assert.AreEqual(2, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, ev.Action);
            Assert.AreEqual(-1, ev.NewStartingIndex);
            Assert.AreEqual(null, ev.NewItems);
            Assert.AreEqual(0, ev.OldStartingIndex);
            Assert.AreEqual(eA, ev.OldItems[0]);
            Assert.AreEqual(2, countString);

            // 
            collection.RemoveAt(0);
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(3, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, ev.Action);
            Assert.AreEqual(-1, ev.NewStartingIndex);
            Assert.AreEqual(null, ev.NewItems);
            Assert.AreEqual(0, ev.OldStartingIndex);
            Assert.AreEqual(eB, ev.OldItems[0]);
            Assert.AreEqual(3, countString);

            collection = null;
        }

    
        [TestMethod]
        public void ClearItems_Clear_And_Notify_Changes()
        {
            var collection = new NavigationEntryCollection();
            int count = 0;
            int countString = 0;
            NotifyCollectionChangedEventArgs ev = null;

            // Add index 0
            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            collection.Add(eA);
            // Add index 1
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            collection.Add(eB);

            collection.CollectionChanged += (s, e) =>
            {
                count++;
                ev = e;
            };
            collection.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Count")
                    countString++;
            };

            collection.Clear();

            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(1, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, ev.Action);
            Assert.AreEqual(-1, ev.NewStartingIndex);
            Assert.AreEqual(null, ev.NewItems);
            Assert.AreEqual(-1, ev.OldStartingIndex);
            Assert.AreEqual(null, ev.OldItems);

            collection = null;
        }

        [TestMethod]
        public void Sync_ItemsControl_On_Move_Region_History()
        {
            var collection = new NavigationEntryCollection();
            int count = 0;
            NotifyCollectionChangedEventArgs ev = null;

            var viewA = new ViewA();
            var contextA = new ViewAViewModel();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", contextA);
            collection.Add(eA);
            // Add index 1
            var viewB = new ViewB();
            var contextB = new ViewBViewModel();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", contextB);
            collection.Add(eB);
            var viewC = new ViewC();
            var contextC = new ViewCViewModel();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", contextC);
            collection.Add(eC);
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eB, collection[1]);
            Assert.AreEqual(eC, collection[2]);

            collection.CollectionChanged += (s, e) =>
            {
                count++;
                ev = e;
            };

            collection.Move(1, 2); // move eB from index 1 (removed) to index 2 (insert) 
            // eA
            // eC
            // eB
            Assert.AreEqual(eA, collection[0]);
            Assert.AreEqual(eC, collection[1]);
            Assert.AreEqual(eB, collection[2]);
            Assert.AreEqual(1, count);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, ev.Action);
            Assert.AreEqual(1, ev.OldStartingIndex);
            Assert.AreEqual(2, ev.NewStartingIndex);
            Assert.AreEqual(eB, ev.NewItems[0]);
            Assert.AreEqual(eB, ev.OldItems[0]);

            collection = null;
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
}
