using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC.TypeInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmLib.IoC.Tests.TypeInfo
{
    [TestClass]
    public class TypeInformationManagerTests
    {
        [TestMethod]
        public void GetConstructor_With_Public_And_Private_Ctor()
        {
            var m = new TypeInformationManager();

            var ctor1 = m.GetConstructor(typeof(Item), true);

            var ctor2 = m.GetConstructor(typeof(ItemWithPrivateCtor), false);
            var ctor3 = m.GetConstructor(typeof(ItemWithPrivateCtor), true);

            Assert.IsNotNull(ctor1);
            Assert.IsNull(ctor2);
            Assert.IsNotNull(ctor3);
        }

        [TestMethod]
        public void GetConstructor_Get_Empty_Ctor_Not_Ordered()
        {
            var m = new TypeInformationManager();

            var ctor1 = m.GetConstructor(typeof(ItemWithMultiCtor), true);

            Assert.AreEqual(0, ctor1.GetParameters().Length);
        }

        [TestMethod]
        public void GetConstructor_Get_Preferred_Ctor()
        {
            var m = new TypeInformationManager();

            var ctor1 = m.GetConstructor(typeof(ItemWithPreferredCtor), true);

            Assert.AreEqual(1, ctor1.GetParameters().Length);
            Assert.AreEqual(typeof(int), ctor1.GetParameters()[0].ParameterType);
        }


        [TestMethod]
        public void GetTypeInformation_Cache()
        {
            var m = new TypeInformationManager();

            var t1 = m.GetTypeInformation(typeof(ItemWithPreferredCtor), true);

            Assert.AreEqual(1, t1.Parameters.Length);
            Assert.AreEqual(typeof(int), t1.Parameters[0].ParameterType);

            Assert.IsNotNull(t1.Constructor);

            Assert.AreEqual(1, m.TypeCache.Count);
        }

        [TestMethod]
        public void Getproperties()
        {
            var m = new TypeInformationManager();

            var p = m.GetPropertiesWithDependencyAttribute(typeof(ItemWithDependency), true);

            Assert.AreEqual(2, p.Count);
            Assert.AreEqual(null, p[0].Name);
            Assert.AreEqual("MyString", p[0].Property.Name);
            Assert.AreEqual("k1", p[1].Name);
            Assert.AreEqual("MyItem", p[1].Property.Name);

            Assert.AreEqual(1, m.PropertiesWithDependencyAttributeCache.Count);
        }
    }



}
