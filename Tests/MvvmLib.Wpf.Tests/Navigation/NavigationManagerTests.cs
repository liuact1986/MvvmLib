using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class NavigationManagerTests
    {
        [TestMethod]
        public void Create_And_Get_SharedSource()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>();
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>());

            var s2 = NavigationManager.GetSharedSource<MySharedItem>();
            Assert.AreEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
        }

        [TestMethod]
        public void GetOrCreateSharedSource_Returns_Same_Instance()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
            var s1 = NavigationManager.GetOrCreateSharedSource<MySharedItem>();
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>());

            var s2 = NavigationManager.GetOrCreateSharedSource<MySharedItem>();
            Assert.AreEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
        }

        [TestMethod]
        public async Task Create_New_SharedSource()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>();
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>());

            await s1.AddNewAsync("A");

            Assert.AreEqual(1, s1.Items.Count);

            var s2 = NavigationManager.GetNewSharedSource<MySharedItem>();
            Assert.AreNotEqual(s1, s2);
            Assert.AreEqual(0, s2.Items.Count);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
        }

        [TestMethod]
        public void Create_Multiple_SharedSources()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>();
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem2>());
            var s2 = NavigationManager.CreateSharedSource<MySharedItem2>();
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem2>());

            Assert.AreEqual(2, NavigationManager.SharedSources.Count);

            Assert.AreNotEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem2>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem2>());
            Assert.AreEqual(1, NavigationManager.SharedSources.Count);
            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>());
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>());
            Assert.AreEqual(0, NavigationManager.SharedSources.Count);
        }

        [TestMethod]
        public void Create_And_Get_KeyedSharedSource()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>("k1");
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));

            var s2 = NavigationManager.GetSharedSource<MySharedItem>("k1");
            Assert.AreEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
        }

        [TestMethod]
        public void GetOrCreateKeyedSharedSource_Returns_Same_Instance()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            var s1 = NavigationManager.GetOrCreateSharedSource<MySharedItem>("k1");
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));

            var s2 = NavigationManager.GetOrCreateSharedSource<MySharedItem>("k1");
            Assert.AreEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
        }

        [TestMethod]
        public async Task Create_New_KeyedSharedSource()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>("k1");
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));

            await s1.AddNewAsync("A");

            Assert.AreEqual(1, s1.Items.Count);

            var s2 = NavigationManager.GetNewSharedSource<MySharedItem>("k1");
            Assert.AreNotEqual(s1, s2);
            Assert.AreEqual(0, s2.Items.Count);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
        }

        [TestMethod]
        public void Create_Multiple_KeyedSharedSources()
        {
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            var s1 = NavigationManager.CreateSharedSource<MySharedItem>("k1");
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k2"));
            var s2 = NavigationManager.CreateSharedSource<MySharedItem>("k2");
            Assert.AreEqual(true, NavigationManager.ContainsSharedSource<MySharedItem>("k2"));

            Assert.AreEqual(1, NavigationManager.KeyedSharedSources.Count);
            Assert.AreEqual(2, NavigationManager.KeyedSharedSources[typeof(MySharedItem)].Count);

            Assert.AreNotEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>("k2"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k2"));
            Assert.AreEqual(1, NavigationManager.KeyedSharedSources[typeof(MySharedItem)].Count);
            Assert.AreEqual(true, NavigationManager.RemoveSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.ContainsSharedSource<MySharedItem>("k1"));
            Assert.AreEqual(false, NavigationManager.KeyedSharedSources.ContainsKey(typeof(MySharedItem)));
            Assert.AreEqual(0, NavigationManager.KeyedSharedSources.Count);
        }


        [TestMethod]
        public void Create_And_Get_NavigationSource()
        {
            Assert.AreEqual(false, NavigationManager.AllNavigationSources.ContainsKey("SourceA"));
            var s1 = NavigationManager.CreateDefaultNavigationSource("SourceA");
            Assert.AreEqual(true, NavigationManager.AllNavigationSources["SourceA"].Contains(s1));
            var s2 = NavigationManager.GetDefaultNavigationSource("SourceA");
            Assert.AreEqual(s1, s2);

            Assert.AreEqual(true, NavigationManager.RemoveNavigationSource("SourceA", s1));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources["SourceA"].Contains(s1));
            Assert.AreEqual(true, NavigationManager.RemoveNavigationSources("SourceA"));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources.ContainsKey("SourceA"));
        }

        [TestMethod]
        public void Create_Multiple_NavigationSources()
        {
            Assert.AreEqual(false, NavigationManager.AllNavigationSources.ContainsKey("SourceA"));
            var s1 = NavigationManager.CreateDefaultNavigationSource("SourceA");
            Assert.AreEqual(true, NavigationManager.AllNavigationSources["SourceA"].Contains(s1));

            var s2 = new KeyedNavigationSource("B");
            NavigationManager.AddNavigationSource("SourceA", s2);
            Assert.AreEqual(true, NavigationManager.AllNavigationSources["SourceA"].Contains(s2));

            var s3 = NavigationManager.GetDefaultNavigationSource("SourceA");
            Assert.AreEqual(s1, s3);

            var s4 = NavigationManager.AllNavigationSources["SourceA"].NavigationSources.FirstOrDefault(n => n is KeyedNavigationSource && ((KeyedNavigationSource)n).Key == "B");
            Assert.AreEqual(s2, s4);

            Assert.AreEqual(true, NavigationManager.RemoveNavigationSource("SourceA", s1));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources["SourceA"].Contains(s1));
            Assert.AreEqual(true, NavigationManager.RemoveNavigationSource("SourceA", s2));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources["SourceA"].Contains(s2));
            Assert.AreEqual(true, NavigationManager.RemoveNavigationSources("SourceA"));
            Assert.AreEqual(false, NavigationManager.AllNavigationSources.ContainsKey("SourceA"));
        }
    }
}
