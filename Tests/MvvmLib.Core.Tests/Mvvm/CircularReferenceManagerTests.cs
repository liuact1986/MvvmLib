using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class CircularReferenceManagerTests
    {
        public CircularReferenceManager GetService()
        {
            return new CircularReferenceManager();
        }

        [TestMethod]
        public void DeepClone_Public()
        {
            var service = GetService();

            var i1 = new InstanceTest { MyString = "Original 1" };
            var c1 = new InstanceTest { MyString = "Cloned 1" };

            var i2 = new InstanceTest2 { MyString = "Original 2" };
            var c2 = new InstanceTest2 { MyString = "Cloned 2" };

            service.AddInstance(i1, c1);
            service.AddInstance(i2, c2);

            var r1 = service.TryGetInstance(i1);

            Assert.AreEqual("Cloned 1", ((InstanceTest)r1).MyString);
        }

        [TestMethod]
        public void RegisterMultiple_Instances_Of_Type()
        {
            var service = GetService();

            var i1 = new InstanceTest { MyString = "Original 1" };
            var c1 = new InstanceTest { MyString = "Cloned 1" };

            var i2 = new InstanceTest { MyString = "Original 2" };
            var c2 = new InstanceTest { MyString = "Cloned 2" };

            service.AddInstance(i1, c1);
            service.AddInstance(i2, c2);

            var r1 = service.TryGetInstance(i1);

            Assert.AreEqual("Cloned 1", ((InstanceTest)r1).MyString);
        }

        [TestMethod]
        public void Register_One_Instance()
        {
            var service = GetService();

            var i1 = new InstanceTest { MyString = "Original 1" };

            var c1 = new InstanceTest { MyString = "Cloned 1" };
            var c2 = new InstanceTest { MyString = "Cloned 2" };

            service.AddInstance(i1, c1);
            service.AddInstance(i1, c2);

            var r1 = service.TryGetInstance(i1);

            Assert.AreEqual("Cloned 1", ((InstanceTest)r1).MyString);
        }
    }

    public class InstanceTest
    {
        public string MyString { get; set; }
    }

    public class InstanceTest2
    {
        public string MyString { get; set; }
    }
}
