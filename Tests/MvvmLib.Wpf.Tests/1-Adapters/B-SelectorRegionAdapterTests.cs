using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Adapters
{
    [TestClass]
    public class SelectorRegionAdapterTests
    {
        [TestMethod]
        public void Region_Gets_An_SelectionRegionAdapter_For_Selector_Control()
        {
            var r1 = new ItemsRegion("R1", new ListBox(), new RegionsRegistry());
            Assert.IsTrue(r1.RegionAdapter is SelectorRegionAdapter);

            var r2 = new ItemsRegion("R2", new ListView(), new RegionsRegistry());
            Assert.IsTrue(r2.RegionAdapter is SelectorRegionAdapter);

            var r3 = new ItemsRegion("R1", new TabControl(), new RegionsRegistry());
            Assert.IsTrue(r3.RegionAdapter is SelectorRegionAdapter);
        }

        [TestMethod]
        public void Sync_Control_On_Add_Entry_Region_History()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S1", control, registry);

            var viewA = new ViewA();
            region.History.Add(new NavigationEntry(typeof(ViewA), viewA, "A", null));
            Assert.AreEqual(1, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);

            var viewB = new ViewB();
            region.History.Add(new NavigationEntry(typeof(ViewB), viewB, "B", null));
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewB, control.Items[1]);

            var viewC = new ViewC();
            region.History.Insert(1, new NavigationEntry(typeof(ViewC), viewC, "C", null));
            Assert.AreEqual(3, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewC, control.Items[1]);
            Assert.AreEqual(viewB, control.Items[2]);
        }

        [TestMethod]
        public void Sync_Control_On_Remove_Entry_Region_History()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S2", control, registry);

            var viewA = new ViewA();
            region.History.Add(new NavigationEntry(typeof(ViewA), viewA, "A", null));
            var viewB = new ViewB();
            region.History.Add(new NavigationEntry(typeof(ViewB), viewB, "B", null));
            var viewC = new ViewC();
            region.History.Add(new NavigationEntry(typeof(ViewC), viewC, "C", null));
            var viewD = new ViewD();
            region.History.Add(new NavigationEntry(typeof(ViewD), viewD, "D", null));
            Assert.AreEqual(4, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewB, control.Items[1]);
            Assert.AreEqual(viewC, control.Items[2]);
            Assert.AreEqual(viewD, control.Items[3]);

            region.History.RemoveAt(3); // last
            Assert.AreEqual(3, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewB, control.Items[1]);
            Assert.AreEqual(viewC, control.Items[2]);

            region.History.RemoveAt(1); // at index 1
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewC, control.Items[1]);

            region.History.RemoveAt(0); // first
            Assert.AreEqual(1, control.Items.Count);
            Assert.AreEqual(viewC, control.Items[0]);
        }

        [TestMethod]
        public void Sync_Control_On_Replace_Entry()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S3", control, registry);

            var viewA = new ViewA();
            region.History.Add(new NavigationEntry(typeof(ViewA), viewA, "A", null));
            var viewB = new ViewB();
            region.History.Add(new NavigationEntry(typeof(ViewB), viewB, "B", null));
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewB, control.Items[1]);

            var viewC = new ViewC();
            region.History.Entries[1] = new NavigationEntry(typeof(ViewC), viewC, "C", null); // index 1
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewC, control.Items[1]);

            var viewD = new ViewD();
            region.History.Entries[0] = new NavigationEntry(typeof(ViewD), viewD, "D", null); // index 0
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewD, control.Items[0]);
            Assert.AreEqual(viewC, control.Items[1]);
        }

        [TestMethod]
        public void Sync_Control_On_Clear_Region_History()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S4", control, registry);

            var viewA = new ViewA();
            region.History.Add(new NavigationEntry(typeof(ViewA), viewA, "A", null));
            var viewB = new ViewB();
            region.History.Add(new NavigationEntry(typeof(ViewB), viewB, "B", null));
            Assert.AreEqual(2, control.Items.Count);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewB, control.Items[1]);

            region.History.Clear();
            Assert.AreEqual(0, control.Items.Count);
        }

        [TestMethod]
        public void Sync_Control_On_Move_Region_History()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S5", control, registry);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", null);
            region.History.Add(eA);
            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", null);
            region.History.Add(eB);
            var viewC = new ViewC();
            var eC = new NavigationEntry(typeof(ViewC), viewC, "C", null);
            region.History.Add(eC);
            Assert.AreEqual(eA, region.History.Entries[0]);
            Assert.AreEqual(eB, region.History.Entries[1]);
            Assert.AreEqual(eC, region.History.Entries[2]);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewB, control.Items[1]);
            Assert.AreEqual(viewC, control.Items[2]);

            region.History.Move(1, 2); // move eB from index 1 (removed) to index 2 (insert) 
            // eA
            // eC
            // eB
            Assert.AreEqual(eA, region.History.Entries[0]);
            Assert.AreEqual(eC, region.History.Entries[1]);
            Assert.AreEqual(eB, region.History.Entries[2]);
            Assert.AreEqual(viewA, control.Items[0]);
            Assert.AreEqual(viewC, control.Items[1]);
            Assert.AreEqual(viewB, control.Items[2]);
        }

        [TestMethod]
        public void Sync_History_On_Control_Selection_Changed()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S6", control, registry);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", null);
            region.History.Add(eA);
            Assert.AreEqual(0, control.SelectedIndex);
            Assert.AreEqual(viewA, control.SelectedItem);
            Assert.AreEqual(0, region.History.CurrentIndex);
            Assert.AreEqual(eA, region.History.Current);

            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", null);
            region.History.Add(eB);
            Assert.AreEqual(1, control.SelectedIndex);
            Assert.AreEqual(viewB, control.SelectedItem);
            Assert.AreEqual(1, region.History.CurrentIndex);
            Assert.AreEqual(eB, region.History.Current);

            control.SelectedIndex = 0;
            Assert.AreEqual(0, control.SelectedIndex);
            Assert.AreEqual(viewA, control.SelectedItem);
            Assert.AreEqual(0, region.History.CurrentIndex);
            Assert.AreEqual(eA, region.History.Current);

            control.SelectedIndex = 1;
            Assert.AreEqual(1, control.SelectedIndex);
            Assert.AreEqual(viewB, control.SelectedItem);
            Assert.AreEqual(1, region.History.CurrentIndex);
            Assert.AreEqual(eB, region.History.Current);
        }

        [TestMethod]
        public void Sync_Control_On_History_Current_Changed()
        {
            var control = new ListBox();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("S7", control, registry);

            var viewA = new ViewA();
            var eA = new NavigationEntry(typeof(ViewA), viewA, "A", null);
            region.History.Add(eA);
            Assert.AreEqual(0, control.SelectedIndex);
            Assert.AreEqual(viewA, control.SelectedItem);
            Assert.AreEqual(0, region.History.CurrentIndex);
            Assert.AreEqual(eA, region.History.Current);

            var viewB = new ViewB();
            var eB = new NavigationEntry(typeof(ViewB), viewB, "B", null);
            region.History.Add(eB);
            Assert.AreEqual(1, control.SelectedIndex);
            Assert.AreEqual(viewB, control.SelectedItem);
            Assert.AreEqual(1, region.History.CurrentIndex);
            Assert.AreEqual(eB, region.History.Current);

            region.History.Select(0);
            Assert.AreEqual(0, control.SelectedIndex);
            Assert.AreEqual(viewA, control.SelectedItem);
            Assert.AreEqual(0, region.History.CurrentIndex);
            Assert.AreEqual(eA, region.History.Current);

            region.History.Select(1);
            Assert.AreEqual(1, control.SelectedIndex);
            Assert.AreEqual(viewB, control.SelectedItem);
            Assert.AreEqual(1, region.History.CurrentIndex);
            Assert.AreEqual(eB, region.History.Current);

            region.SynchronizationMode = SynchronizationHandling.None;

            region.History.Select(0);
            Assert.AreEqual(1, control.SelectedIndex);
            Assert.AreEqual(viewB, control.SelectedItem);

        }
    }

}
