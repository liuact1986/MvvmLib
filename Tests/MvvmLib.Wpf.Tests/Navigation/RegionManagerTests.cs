using System;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests
{

    [TestClass]
    public class RegionManagerTests
    {
        // static

        [TestMethod]
        public void RegisterContentRegion()
        {
            RegionManager.ClearRegions();
            var regionName = "MyContentRegion";

            var control = new ContentControl();
            control.Name = "c1";

            var control2 = new ContentControl();
            control2.Name = "c2";

            var region = RegionManager.AddContentRegion(regionName, control);

            Assert.AreEqual(typeof(ContentRegion), region.GetType());
            Assert.AreEqual(regionName, region.RegionName);
            Assert.AreEqual(control, region.Control);
            Assert.AreEqual("c1", region.ControlName);

            RegionManager.AddContentRegion(regionName, control2);

            var r2 = RegionManager.GetContentRegionByName(regionName, "c2");
            Assert.AreEqual("c2", r2.ControlName);

            Assert.IsTrue(RegionManager.UnregisterContentRegions(regionName));

            RegionManager.ClearRegions();
        }

        [TestMethod]
        public void RegisterItemsRegion()
        {
            RegionManager.ClearRegions();

            var regionName = "MyItemsRegion";

            var control = new ItemsControl();
            control.Name = "i1";

            var control2 = new ItemsControl();
            control2.Name = "i2";

            var region = RegionManager.AddItemsRegion(regionName, control);

            Assert.AreEqual(typeof(ItemsRegion), region.GetType());
            Assert.AreEqual(regionName, region.RegionName);
            Assert.AreEqual(control, region.Control);
            Assert.AreEqual("i1", region.ControlName);

            RegionManager.AddItemsRegion(regionName, control2);

            var r2 = RegionManager.GetItemsRegionByName(regionName, "i2");
            Assert.AreEqual("i2", r2.ControlName);

            Assert.IsTrue(RegionManager.UnregisterItemsRegions(regionName));

            RegionManager.ClearRegions();
        }

        // implementation

        [TestMethod]
        public void GetContentRegion()
        {
            RegionManager.ClearRegions();

            var regionName = "M2";

            var control = new ContentControl();
            control.Name = "c1";

            var control2 = new ContentControl();
            control2.Name = "c2";

            RegionManager.AddContentRegion(regionName, control);
            RegionManager.AddContentRegion(regionName, control2);

            var r = new RegionManager();

            var c1 = r.GetContentRegion(regionName); // last
            var c2 = r.GetContentRegion(regionName, "c2");

            Assert.AreEqual("c2", c1.ControlName);
            Assert.AreEqual("c2", c2.ControlName);

            RegionManager.ClearRegions();
        }


        [TestMethod]
        public void GetItemsRegion()
        {
            RegionManager.ClearRegions();

            var regionName = "M1";

            var control = new ItemsControl();
            control.Name = "i1";

            var control2 = new ItemsControl();
            control2.Name = "i2";

            RegionManager.AddItemsRegion(regionName, control);
            RegionManager.AddItemsRegion(regionName, control2);

            var r = new RegionManager();

            var c1 = r.GetItemsRegion(regionName);
            var c2 = r.GetItemsRegion(regionName, "i2");

            Assert.AreEqual("i2", c1.ControlName); // last
            Assert.AreEqual("i2", c2.ControlName);

            RegionManager.ClearRegions();
        }
    }
}
