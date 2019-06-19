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
        public void Navigate_Sets_The_Content_Of_The_Content_Control()
        {
            MyNavViewModelA.Reset();

            var contentControl = new ContentControl();
            var navigationSource = new ContentControlNavigationSource("1", contentControl);
            Assert.AreEqual("1", navigationSource.SourceName);

            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);

            navigationSource.Navigate(typeof(MyNavViewModelA), "p1");

            Assert.AreEqual(typeof(MyNavViewModelA), navigationSource.Current.GetType());
            Assert.IsNotNull(contentControl.Content);
            Assert.AreEqual(typeof(MyNavViewModelA), contentControl.Content.GetType());
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.CurrentIndex);
        }
    }

}
