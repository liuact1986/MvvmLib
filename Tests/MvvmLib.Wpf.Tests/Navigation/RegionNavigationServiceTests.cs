using System;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests
{

    [TestClass]
    public class RegionNavigationServiceTests
    {
        [TestMethod]
        public void GetContentRegion()
        {
            var regionsRegistry = new RegionsRegistry();
            var r = new RegionNavigationService(regionsRegistry);

            var regionName = "M2";

            var control = new ContentControl();
            control.Name = "c1";

            var control2 = new ContentControl();
            control2.Name = "c2";

            regionsRegistry.RegisterContentRegion(regionName, control);
            regionsRegistry.RegisterContentRegion(regionName, control2);

            var c1 = r.GetContentRegion(regionName); // last
            var c2 = r.GetContentRegion(regionName, "c2");

            Assert.AreEqual("c2", c1.ControlName);
            Assert.AreEqual("c2", c2.ControlName);
        }


        [TestMethod]
        public void GetItemsRegion()
        {
            var regionsRegistry = new RegionsRegistry();
            var r = new RegionNavigationService(regionsRegistry);

            var regionName = "M1";

            var control = new ItemsControl();
            control.Name = "i1";

            var control2 = new ItemsControl();
            control2.Name = "i2";

            regionsRegistry.RegisterItemsRegion(regionName, control);
            regionsRegistry.RegisterItemsRegion(regionName, control2);

            var c1 = r.GetItemsRegion(regionName);
            var c2 = r.GetItemsRegion(regionName, "i2");

            Assert.AreEqual("i2", c1.ControlName); // last
            Assert.AreEqual("i2", c2.ControlName);
        }
    }
}
