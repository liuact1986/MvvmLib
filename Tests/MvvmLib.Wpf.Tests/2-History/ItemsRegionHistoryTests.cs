using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.History
{

    public class ViewAViewModel { }
    public class ViewBViewModel { }
    public class ViewCViewModel { }
    public class ViewDViewModel { }

    [TestClass]
    public class ItemsRegionHistoryTests
    {
        [TestMethod]
        public void Add_Entry_Adds_The_Entry_At_The_End_Of_Stack_And_Set_The_Current_Index()
        {
            var history = new ItemsRegionHistory();
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(0, history.Entries.Count);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Add(eA);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);

            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Add(eB);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType);
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);

            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Add(eC);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
        }

        [TestMethod]
        public void Add_Entry_Directly_With_Entries_Collection_Sets_The_Current_Index()
        {
            var history = new ItemsRegionHistory();
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(0, history.Entries.Count);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Entries.Add(eA);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);

            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Entries.Add(eB);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType);
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);

            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Entries.Add(eC);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
        }

        [TestMethod]
        public void Inserts_And_Select_The_Inserted_Index()
        {
            var history = new ItemsRegionHistory();
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(0, history.Entries.Count);

            // insert index 0
            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Insert(0, eA);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);

            // at the end
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Add(eB);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType);
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);

            // at index 1
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Insert(1, eC);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eC.Source, history.Entries[1].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[1].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eB.Source, history.Entries[2].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[2].Context);

            // at index 0
            var viewD = new ViewC();
            var eD = new NavigationEntry(typeof(ViewD), viewC, "D", new ViewDViewModel());
            history.Insert(0, eD);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(4, history.Entries.Count);
            Assert.AreEqual(eD, history.Current);
            Assert.AreEqual(eD.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eD.Source, history.Entries[0].Source);
            Assert.AreEqual(eD.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eD.Context, history.Entries[0].Context);
            Assert.AreEqual(eA.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eA.Source, history.Entries[1].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[3].SourceType); // index 3
            Assert.AreEqual(eB.Source, history.Entries[3].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[3].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[3].Context);
        }

        [TestMethod]
        public void Inserts_Directly_With_Entries_Collection_Sets__The_Current_Index()
        {
            var history = new ItemsRegionHistory();
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(0, history.Entries.Count);

            // insert index 0
            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Entries.Insert(0, eA);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(1, history.Entries.Count);
            Assert.AreEqual(eA, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);

            // at the end
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Add(eB);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eB, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType);
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType);
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);

            // at index 1
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Entries.Insert(1, eC);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eC.Source, history.Entries[1].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[1].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eB.Source, history.Entries[2].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[2].Context);

            // at index 0
            var viewD = new ViewC();
            var eD = new NavigationEntry(typeof(ViewD), viewC, "D", new ViewDViewModel());
            history.Entries.Insert(0, eD);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(4, history.Entries.Count);
            Assert.AreEqual(eD, history.Current);
            Assert.AreEqual(eD.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eD.Source, history.Entries[0].Source);
            Assert.AreEqual(eD.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eD.Context, history.Entries[0].Context);
            Assert.AreEqual(eA.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eA.Source, history.Entries[1].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[3].SourceType); // index 3
            Assert.AreEqual(eB.Source, history.Entries[3].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[3].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[3].Context);
        }

        [TestMethod]
        public void Removes_Entry_At_Index_And_Sets_The_Current_Index()
        {
            var history = new ItemsRegionHistory();
            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Add(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Add(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Add(eC);
            var viewD = new ViewC();
            var eD = new NavigationEntry(typeof(ViewD), viewC, "D", new ViewDViewModel());
            history.Add(eD);
            Assert.AreEqual(3, history.CurrentIndex);
            Assert.AreEqual(4, history.Entries.Count);
            Assert.AreEqual(eD, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
            Assert.AreEqual(eD.SourceType, history.Entries[3].SourceType); // index 3
            Assert.AreEqual(eD.Source, history.Entries[3].Source);
            Assert.AreEqual(eD.Parameter, history.Entries[3].Parameter);
            Assert.AreEqual(eD.Context, history.Entries[3].Context);

            // the last
            history.RemoveAt(3);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);

            history.RemoveAt(1);
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(2, history.Entries.Count);
            Assert.AreEqual(eC, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eC.Source, history.Entries[1].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[1].Context);
        }

        [TestMethod]
        public void Remove_On_Current_Index()
        {
            var history = new ItemsRegionHistory();
            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
            history.Add(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
            history.Add(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
            history.Add(eC);
            var viewD = new ViewC();
            var eD = new NavigationEntry(typeof(ViewD), viewC, "D", new ViewDViewModel());
            history.Add(eD);
            Assert.AreEqual(3, history.CurrentIndex);
            Assert.AreEqual(4, history.Entries.Count);
            Assert.AreEqual(eD, history.Current);
            Assert.AreEqual(eA.SourceType, history.Entries[0].SourceType); // index 0
            Assert.AreEqual(eA.Source, history.Entries[0].Source);
            Assert.AreEqual(eA.Parameter, history.Entries[0].Parameter);
            Assert.AreEqual(eA.Context, history.Entries[0].Context);
            Assert.AreEqual(eB.SourceType, history.Entries[1].SourceType); // index 1
            Assert.AreEqual(eB.Source, history.Entries[1].Source);
            Assert.AreEqual(eB.Parameter, history.Entries[1].Parameter);
            Assert.AreEqual(eB.Context, history.Entries[1].Context);
            Assert.AreEqual(eC.SourceType, history.Entries[2].SourceType); // index 2
            Assert.AreEqual(eC.Source, history.Entries[2].Source);
            Assert.AreEqual(eC.Parameter, history.Entries[2].Parameter);
            Assert.AreEqual(eC.Context, history.Entries[2].Context);
            Assert.AreEqual(eD.SourceType, history.Entries[3].SourceType); // index 3
            Assert.AreEqual(eD.Source, history.Entries[3].Source);
            Assert.AreEqual(eD.Parameter, history.Entries[3].Parameter);
            Assert.AreEqual(eD.Context, history.Entries[3].Context);

            history.Select(2);
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eC, history.Current);

            history.RemoveAt(2); // remove eC
            Assert.AreEqual(2, history.CurrentIndex);
            Assert.AreEqual(eD, history.Current);

            history.RemoveAt(2); // remove eD
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eB, history.Current);

            history.Select(0);
            history.RemoveAt(0);
            Assert.AreEqual(0, history.CurrentIndex);
            Assert.AreEqual(eB, history.Current);

            history.RemoveAt(0);
            Assert.AreEqual(-1, history.CurrentIndex);
            Assert.AreEqual(0, history.Entries.Count);
        }

        //[TestMethod]
        //public void Remove_On_Current_Index_Always_Notify()
        //{
        //    var history = new ItemsRegionHistory();
        //    var viewA = new ViewA();
        //    var eA = new NavigationEntry(typeof(ViewA), viewA, "A", new ViewAViewModel());
        //    history.Add(eA);
        //    var viewB = new ViewB();
        //    var eB = new NavigationEntry(typeof(ViewB), viewB, "B", new ViewBViewModel());
        //    history.Add(eB);
        //    var viewC = new ViewC();
        //    var eC = new NavigationEntry(typeof(ViewC), viewC, "C", new ViewCViewModel());
        //    history.Add(eC);
        //    var viewD = new ViewC();
        //    var eD = new NavigationEntry(typeof(ViewD), viewC, "D", new ViewDViewModel());
        //    history.Add(eD);
        //    Assert.AreEqual(3, history.CurrentIndex);
        //    Assert.AreEqual(4, history.Entries.Count);
        //    Assert.AreEqual(eD, history.Current);

        //    int called = 0;
        //    IndexedNavigationEntryEventArgs r = null;
        //    history.CurrentChanged += (s, e) =>
        //    {
        //        called++;
        //        r = e;
        //    };

        //    history.Select(2);
        //    history.RemoveAt(2); // remove eC
        //    Assert.AreEqual(2, called); // select + remove
        //    Assert.AreEqual(2, r.Index);
        //    Assert.AreEqual(eC, r.Entry);

        //    history.RemoveAt(2); // remove eD
        //    Assert.AreEqual(3, called);
        //    Assert.AreEqual(2, r.Index);
        //    Assert.AreEqual(eD, r.Entry);

        //    history.Select(0);
        //    history.RemoveAt(0);
        //    Assert.AreEqual(5, called);
        //    Assert.AreEqual(0, r.Index);
        //    Assert.AreEqual(eC, r.Entry);

        //    history.RemoveAt(0);
        //    Assert.AreEqual(6, called);
        //    Assert.AreEqual(0, r.Index);
        //    Assert.AreEqual(eB, r.Entry);
        //}
    }
}
