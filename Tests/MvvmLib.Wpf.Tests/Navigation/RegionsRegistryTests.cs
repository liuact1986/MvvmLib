using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests
{
    [TestClass]
    public class RegionsRegistryTests
    {
        // static

        [TestMethod]
        public void RegisterContentRegion()
        {
            var regionsRegistry = new RegionsRegistry();
            var regionName = "MyContentRegion";

            var control = new ContentControl();
            control.Name = "c1";

            var control2 = new ContentControl();
            control2.Name = "c2";

            var region = regionsRegistry.RegisterContentRegion(regionName, control);

            Assert.AreEqual(typeof(ContentRegion), region.GetType());
            Assert.AreEqual(regionName, region.RegionName);
            Assert.AreEqual(control, region.Control);
            Assert.AreEqual("c1", region.ControlName);

            regionsRegistry.RegisterContentRegion(regionName, control2);

            var r2 = regionsRegistry.GetContentRegion(regionName, "c2");
            Assert.AreEqual("c2", r2.ControlName);

            //Assert.IsTrue(regionsRegistry.UnregisterContentRegions(regionName));
        }

        [TestMethod]
        public void RegisterItemsRegion()
        {
            var regionsRegistry = new RegionsRegistry();

            var regionName = "MyItemsRegion";

            var control = new ItemsControl();
            control.Name = "i1";

            var control2 = new ItemsControl();
            control2.Name = "i2";

            var region = regionsRegistry.RegisterItemsRegion(regionName, control);

            Assert.AreEqual(typeof(ItemsRegion), region.GetType());
            Assert.AreEqual(regionName, region.RegionName);
            Assert.AreEqual(control, region.Control);
            Assert.AreEqual("i1", region.ControlName);

            regionsRegistry.RegisterItemsRegion(regionName, control2);

            var r2 = regionsRegistry.GetItemsRegion(regionName, "i2");
            Assert.AreEqual("i2", r2.ControlName);

            //Assert.IsTrue(regionsRegistry.RemoveItemsRegion(regionName));
        }
    }
}
