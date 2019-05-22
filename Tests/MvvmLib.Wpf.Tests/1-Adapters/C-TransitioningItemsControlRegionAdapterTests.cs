using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Animation;
using MvvmLib.Navigation;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Adapters
{
    public class ReflectionHelper
    {
        private static ReflectionHelper _instance = new ReflectionHelper();

        public static ReflectionHelper Instance
        {
            get { return _instance; }
        }

        private static BindingFlags GetFlags(bool nonPublic)
        {
            var flags = nonPublic ?
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
               : BindingFlags.Instance | BindingFlags.Public;
            return flags;
        }

        public FieldInfo GetField(Type type, string fieldName)
        {
            var flags = GetFlags(true);
            var field = type.GetField(fieldName, flags);
            return field;
        }

        public bool SetField(Type type, string fieldName, object instance, object value)
        {
            var field = GetField(type, fieldName);
            if(field != null)
            {
                field.SetValue(instance, value);
                return true;
            }
            return false;
        }
        public bool SetField<T>(string fieldName, T instance, object value)
        {
            return SetField(typeof(T), fieldName, instance, value);
        }
    }

    [TestClass]
    public class TransitioningItemsControlRegionAdapterTests
    {
        [TestMethod]
        public void Region_Gets_An_ItemsRegionAdapter_For_Inner_ItemsControl()
        {
            var control = new TransitioningItemsControl();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("T1", control, registry);

            Assert.IsTrue(region.RegionAdapter is TransitioningItemsControlRegionAdapter);
        }

        [TestMethod]
        public void Sync_Inner_ItemsControl_On_Add_Entry_Region_History()
        {
            var control = new TransitioningItemsControl();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("T2", control, registry);

            SetAndGetInnerItemsControl(control);

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
        public void Sync_Inner_ItemsControl_On_Remove_Entry_Region_History()
        {
            var control = new TransitioningItemsControl();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("T3", control, registry);

            SetAndGetInnerItemsControl(control);

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
        public void Sync_Inner_ItemsControl_On_Replace_Entry()
        {
            var control = new TransitioningItemsControl();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("T4", control, registry);

            SetAndGetInnerItemsControl(control);

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

        private ItemsControl SetAndGetInnerItemsControl(TransitioningItemsControl control)
        {
            var innerItemsControl = new ItemsControl();
            if (!ReflectionHelper.Instance.SetField<TransitioningItemsControl>("innerItemsControl", control, innerItemsControl))
            {
                throw new Exception("Fail to set inner items control");
            }
            return innerItemsControl;
        }

        [TestMethod]
        public void Sync_Inner_ItemsControl_On_Clear_Region_History()
        {
            var control = new TransitioningItemsControl();
            var registry = new RegionsRegistry();
            var region = new ItemsRegion("T5", control, registry);

            SetAndGetInnerItemsControl(control);

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

        //[TestMethod]
        //public void Sync_Inner_ItemsControl_On_Move_Region_History()
        //{
        //    //var oldIndex = e.OldStartingIndex;
        //    //index = e.NewStartingIndex;
        //    //var removedItem = control.Items[oldIndex];
        //    //control.Items.RemoveAt(oldIndex);
        //    //control.Items.Insert(index, removedItem);

        //    //var oldIndex = e.OldStartingIndex;
        //    //index = e.NewStartingIndex;
        //    //var itemToMove = control.Ite

        //    // item1 index 1 => index 2
        //    // item2 index 2 => index 1

        //    var control = new TransitioningItemsControl();
        //    var registry = new RegionsRegistry();
        //    var region = new ItemsRegion("T6", control, registry);

        //    SetAndGetInnerItemsControl(control);

        //    var viewA = new ViewA();
        //    var eA = new NavigationEntry(typeof(ViewA), viewA, "A", null);
        //    region.History.Add(eA);
        //    var viewB = new ViewB();
        //    var eB = new NavigationEntry(typeof(ViewB), viewB, "B", null);
        //    region.History.Add(eB);
        //    var viewC = new ViewC();
        //    var eC = new NavigationEntry(typeof(ViewC), viewC, "C", null);
        //    region.History.Add(eC);
        //    Assert.AreEqual(eA, region.History.Entries[0]);
        //    Assert.AreEqual(eB, region.History.Entries[1]);
        //    Assert.AreEqual(eC, region.History.Entries[2]);
        //    Assert.AreEqual(viewA, control.Items[0]);
        //    Assert.AreEqual(viewB, control.Items[1]);
        //    Assert.AreEqual(viewC, control.Items[2]);

        //    region.History.Move(1, 2); // move eB from index 1 (removed) to index 2 (insert) 
        //    // eA
        //    // eC
        //    // eB
        //    Assert.AreEqual(eA, region.History.Entries[0]);
        //    Assert.AreEqual(eC, region.History.Entries[1]);
        //    Assert.AreEqual(eB, region.History.Entries[2]);
        //    Assert.AreEqual(viewA, control.Items[0]);
        //    Assert.AreEqual(viewC, control.Items[1]);
        //    Assert.AreEqual(viewB, control.Items[2]);
        //}
    }

}
