using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC;
using MvvmLib.IoC.Factories;
using MvvmLib.IoC.Tests;

namespace MvvmLib.Tests.IoC.ObjectCreation
{
    [TestClass]
    public class FactoryManagerTests
    {

        private ConstructorInfo GetFirstConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            return constructors.FirstOrDefault();
        }

        [TestMethod]
        public void Cache_Factory()
        {
            var service = new FactoryManager();

            Assert.AreEqual(typeof(ExpressionDelegateFactory), service.DelegateFactory.GetType());
            Assert.AreEqual(DelegateFactoryType.Expression, service.DelegateFactoryType);

            var i1 = service.CreateInstanceWithEmptyConstructor(typeof(Item), GetFirstConstructor(typeof(Item)));
            Assert.AreEqual(typeof(Item), i1.GetType());
            Assert.AreEqual(1, service.FactoryCache.Count);

            var i2 = service.CreateInstanceWithEmptyConstructor(typeof(Item), GetFirstConstructor(typeof(Item)));
            Assert.AreEqual(1, service.FactoryCache.Count);
        }

        [TestMethod]
        public void Cache_Parametrized_Factory()
        {
            var service = new FactoryManager();

            Assert.AreEqual(typeof(ExpressionDelegateFactory), service.DelegateFactory.GetType());
            Assert.AreEqual(DelegateFactoryType.Expression, service.DelegateFactoryType);

            var i1 = service.CreateInstanceWithParameterizedConstructor(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)), new object[] { "p1" });
            Assert.AreEqual(typeof(ItemWithString), i1.GetType());
            Assert.AreEqual("p1", ((ItemWithString)i1).myString);
            Assert.AreEqual(1, service.ParameterizedFactoryCache.Count);

            var i2 = service.CreateInstanceWithParameterizedConstructor(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)), new object[] { "p1" });
            Assert.AreEqual(1, service.ParameterizedFactoryCache.Count);
        }

        [TestMethod]
        public void Clear_Factories_Caches_On_Delegate_Factory_Type_Changed()
        {
            var service = new FactoryManager();

            Assert.AreEqual(typeof(ExpressionDelegateFactory), service.DelegateFactory.GetType());
            Assert.AreEqual(DelegateFactoryType.Expression, service.DelegateFactoryType);

            var i1 = service.CreateInstanceWithEmptyConstructor(typeof(Item), GetFirstConstructor(typeof(Item)));
            var i2 = service.CreateInstanceWithParameterizedConstructor(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)), new object[] { "p1" });
            Assert.AreEqual(1, service.FactoryCache.Count);
            Assert.AreEqual(1, service.ParameterizedFactoryCache.Count);

            service.DelegateFactoryType = DelegateFactoryType.Expression;
            Assert.AreEqual(1, service.FactoryCache.Count);
            Assert.AreEqual(1, service.ParameterizedFactoryCache.Count);

            service.DelegateFactoryType = DelegateFactoryType.Reflection;
            Assert.AreEqual(0, service.FactoryCache.Count);
            Assert.AreEqual(0, service.ParameterizedFactoryCache.Count);
            Assert.AreEqual(typeof(ReflectionDelegateFactory), service.DelegateFactory.GetType());
        }

        [TestMethod]
        public void Get_Instances_With_Reflection_Factories()
        {
            var service = new FactoryManager();

            service.DelegateFactoryType = DelegateFactoryType.Reflection;

            Assert.AreEqual(typeof(ReflectionDelegateFactory), service.DelegateFactory.GetType());

            var i1 = service.CreateInstanceWithEmptyConstructor(typeof(Item), GetFirstConstructor(typeof(Item)));
            Assert.AreEqual(typeof(Item), i1.GetType());
            Assert.AreEqual(1, service.FactoryCache.Count);

            var i2 = service.CreateInstanceWithParameterizedConstructor(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)), new object[] { "p1" });
            Assert.AreEqual(typeof(ItemWithString), i2.GetType());
            Assert.AreEqual("p1", ((ItemWithString)i2).myString);
            Assert.AreEqual(1, service.ParameterizedFactoryCache.Count);
        }
    }
}
