using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Utils;
using MvvmLib.Wpf.Tests.Mvvm;

namespace MvvmLib.Wpf.Tests.Utils
{
    [TestClass]
    public class PropertyPathHelperTests
    {
        [TestMethod]
        public void Finds_Property_At_Top_Level()
        {
            var p = PropertyPathHelper.GetProperty(typeof(MyFilteredItem), "MyInt");
            Assert.IsNotNull(p);
            Assert.AreEqual("MyInt", p.Name);
            Assert.AreEqual(typeof(int), p.PropertyType);
        }

        [TestMethod]
        public void Finds_Sub_Property()
        {
            var p = PropertyPathHelper.GetProperty(typeof(MyFilteredItem), "MyInner.MyInnerString");
            Assert.IsNotNull(p);
            Assert.AreEqual("MyInnerString", p.Name);
            Assert.AreEqual(typeof(string), p.PropertyType);
        }

        [TestMethod]
        public void Finds_SubSub_Property()
        {
            var p = PropertyPathHelper.GetProperty(typeof(MyFilteredItem), "MyInner.MySubSubItem.MySubSubString");
            Assert.IsNotNull(p);
            Assert.AreEqual("MySubSubString", p.Name);
            Assert.AreEqual(typeof(string), p.PropertyType);
        }
    }

}
