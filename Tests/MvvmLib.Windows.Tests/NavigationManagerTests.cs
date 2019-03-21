
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using MvvmLib.Navigation;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Windows.Tests
{

    [TestClass]
    public class NavigationManagerTests
    {
        public NavigationManager GetService()
        {
            return new NavigationManager();
        }

        [UITestMethod]
        public void Register_WithStatic()
        {
            var frame = new Frame { Name = "f1" };
            var frame2 = new Frame { Name = "f1" };

            var n1 = NavigationManager.Register(frame);
            var n2 = NavigationManager.Register(frame2, "f2");

            var service = GetService();

            var i1 = service.GetDefault();
            var i2 = service.GetNamed("f2");

            Assert.IsTrue(service.IsRegistered());
            Assert.IsTrue(service.IsRegistered("f2"));

            Assert.AreEqual(n1, i1);
            Assert.AreEqual(n2, i2);
        }
    }
}
