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
            Assert.AreEqual("1", navigationSource.Name);

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
        public async Task Creates_A_NavigationSource_Returns_The_Same_Instance()
        {
            var n1 = NavigationManager.CreateNavigationSource("k1");
            var n2 = NavigationManager.GetNavigationSource("k1");
            Assert.AreEqual(n1, n2);

            Assert.IsTrue(NavigationManager.RemoveNavigationSource("k1"));
            Assert.AreEqual(false, NavigationManager.NavigationSources.ContainsKey("k1"));
        }

        [TestMethod]
        public async Task Get_Or_Create_NavigationSource_Returns_The_Same_Instance()
        {
            var n1 = NavigationManager.GetOrCreateNavigationSource("k2");
            var n2 = NavigationManager.GetOrCreateNavigationSource("k2");
            Assert.AreEqual(n1, n2);
            Assert.IsTrue(NavigationManager.RemoveNavigationSource("k2"));
        }

        [TestMethod]
        public async Task GetOrCreateSharedSource_Returns_The_Same_Instance()
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
