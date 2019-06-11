using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class ContentControlNavigationSourceTests
    {
        [TestMethod]
        public async Task Navigate_Sets_The_Content_Of_The_Content_Control()
        {
            NavigatableView.Reset();

            var contentControl = new ContentControl();
            var navigationSource = new ContentControlNavigationSource("1", contentControl);
            Assert.AreEqual("1", navigationSource.SourceName);

            await navigationSource.NavigateAsync(typeof(NavigatableView), "p1");

            Assert.AreEqual(typeof(NavigatableView), navigationSource.Current.GetType());
            Assert.IsNotNull(contentControl.Content);
            Assert.AreEqual(typeof(NavigatableView), contentControl.Content.GetType());
        }
    }

    [TestClass]
    public class NavigationManagerTests
    {
        [TestMethod]
        public void Creates_A_NavigationSource_Returns_The_Same_Instance()
        {
            var n1 = NavigationManager.CreateNavigationSource("k1");
            var n2 = NavigationManager.GetNavigationSource("k1");
            Assert.AreEqual(n1, n2);

            Assert.IsTrue(NavigationManager.UnregisterNavigationSource("k1", n1));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources["k1"].IsRegistered(n1));
        }

        [TestMethod]
        public void Creates_And_Register_NavigationSources()
        {
            var n1 = NavigationManager.CreateNavigationSource("k0");
            var n2 = new NavigationSource();
            NavigationManager.RegisterNavigationSource("k0", n2);

            Assert.AreEqual(2, NavigationManager.AllNavigationSources["k0"].Count);

            Assert.IsTrue(NavigationManager.UnregisterNavigationSource("k0", n1));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources["k0"].IsRegistered(n1));
            Assert.AreEqual(true, NavigationManager.AllNavigationSources["k0"].IsRegistered(n2));
            Assert.AreEqual(1, NavigationManager.AllNavigationSources["k0"].Count);

            Assert.IsTrue(NavigationManager.UnregisterNavigationSource("k0", n2));
            Assert.AreEqual(0, NavigationManager.AllNavigationSources["k0"].Count);

            Assert.IsTrue(NavigationManager.UnregisterNavigationSources("k0"));
            Assert.IsFalse(NavigationManager.AllNavigationSources.ContainsKey("k0"));
        }

        [TestMethod]
        public void Get_Or_Create_NavigationSource_Returns_The_Same_Instance()
        {
            var n1 = NavigationManager.GetOrCreateNavigationSource("k2");
            var n2 = NavigationManager.GetOrCreateNavigationSource("k2");
            Assert.AreEqual(n1, n2);
            Assert.IsTrue(NavigationManager.UnregisterNavigationSources("k2"));
            Assert.IsFalse(NavigationManager.AllNavigationSources.ContainsKey("k2"));
        }

        [TestMethod]
        public void GetOrCreateSharedSource_Returns_The_Same_Instance()
        {
            var n1 = NavigationManager.GetOrCreateSharedSource<Item>();
            var n2 = NavigationManager.GetOrCreateSharedSource<Item>();
            Assert.AreEqual(n1, n2);
        }
    }

    public class Item
    {

    }


}
