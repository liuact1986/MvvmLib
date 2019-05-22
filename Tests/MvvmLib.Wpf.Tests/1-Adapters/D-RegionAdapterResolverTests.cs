using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Animation;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Adapters
{
    [TestClass]
    public class RegionAdapterResolverTests
    {
        [TestMethod]
        public void Gets_An_SelectionRegionAdapter_For_Selector_Control()
        {
            var r1 = RegionAdapterResolver.GetRegionAdapter(new ListBox());
            Assert.IsTrue(r1 is SelectorRegionAdapter);

            var r2 = RegionAdapterResolver.GetRegionAdapter(new ListView());
            Assert.IsTrue(r2 is SelectorRegionAdapter);

            var r3 = RegionAdapterResolver.GetRegionAdapter(new TabControl());
            Assert.IsTrue(r3 is SelectorRegionAdapter);
        }

        [TestMethod]
        public void Gets_An_ItemsRegionAdapter_For_ItemsControl()
        {
            var r1 = RegionAdapterResolver.GetRegionAdapter(new ItemsControl());
            Assert.IsTrue(r1 is ItemsControlRegionAdapter);
        }

        [TestMethod]
        public void Gets_A_TransitioningItemsControlRegionAdapter_For_TransitioningItemsControl()
        {
            var r1 = RegionAdapterResolver.GetRegionAdapter(new TransitioningItemsControl());
            Assert.IsTrue(r1 is TransitioningItemsControlRegionAdapter);
        }

        [TestMethod]
        public void Gets_A_Custom_Region_Adapter()
        {
            var adapter = new StackPanelRegionAdapter();
            RegionAdapterResolver.RegisterRegionAdapter(typeof(StackPanel), adapter);

            var r1 = RegionAdapterResolver.GetRegionAdapter(new StackPanel());
            Assert.IsTrue(r1 is StackPanelRegionAdapter);
        }

        [TestMethod]
        public void Gets_A_Custom_Registered_Adapter_Before_A_Default()
        {
            var adapter = new MyItemsControlRegionAdapter();
            RegionAdapterResolver.RegisterRegionAdapter(typeof(ItemsControl), adapter);

            var r1 = RegionAdapterResolver.GetRegionAdapter(new ItemsControl());
            Assert.IsTrue(r1 is MyItemsControlRegionAdapter);
        }
    }

}
