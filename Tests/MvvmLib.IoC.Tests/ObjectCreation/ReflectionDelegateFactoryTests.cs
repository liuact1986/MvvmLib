using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC;
using MvvmLib.IoC.Factories;
using MvvmLib.IoC.Tests;
using System;
using System.Linq;
using System.Reflection;

namespace MvvmLib.Tests.IoC.ObjectCreation
{
    [TestClass]
    public class ReflectionDelegateFactoryTests
    {
        public ReflectionDelegateFactory GetService()
        {
            return new ReflectionDelegateFactory();
        }

        private ConstructorInfo GetFirstConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            return constructors.FirstOrDefault();
        }

        [TestMethod]
        public void CreateConstructor()
        {
            var service = this.GetService();

            var c2 = service.CreateConstructor<Item>(typeof(Item), GetFirstConstructor(typeof(Item)));
            var c3 = service.CreateConstructor<ViewA>(typeof(ViewA), GetFirstConstructor(typeof(ViewA)));

            var r2 = c2();
            var r3 = c3();

            Assert.AreEqual(typeof(Item), r2.GetType());
            Assert.AreEqual(typeof(ViewA), r3.GetType());
        }

        [TestMethod]
        public void CreateConstructor_WithObject()
        {
            var service = this.GetService();

            var c2 = service.CreateConstructor<object>(typeof(Item), GetFirstConstructor(typeof(Item)));
            var c3 = service.CreateConstructor<object>(typeof(ViewA), GetFirstConstructor(typeof(ViewA)));

            var r2 = c2();
            var r3 = c3();

            Assert.AreEqual(typeof(Item), r2.GetType());
            Assert.AreEqual(typeof(ViewA), r3.GetType());
        }

        [TestMethod]
        public void CreateParameterizedConstructor()
        {
            var service = this.GetService();

            //var c1 = service.CreateParameterizedConstructor<string>(typeof(string), GetFirstConstructor(typeof(Item)));
            var c2 = service.CreateParameterizedConstructor<ItemWithString>(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)));
            var c3 = service.CreateParameterizedConstructor<ViewAWithInjection>(typeof(ViewAWithInjection), GetFirstConstructor(typeof(ViewAWithInjection)));

            //var r1 = c1(new object[] { "p1" });
            var r2 = c2(new object[] { "p1" });
            var r3 = c3(new object[] { "p1" });

            //Assert.AreEqual(typeof(string), r1.GetType());
            //Assert.AreEqual("p1", r1);

            Assert.AreEqual(typeof(ItemWithString), r2.GetType());
            Assert.AreEqual("p1", r2.myString);

            Assert.AreEqual(typeof(ViewAWithInjection), r3.GetType());
            Assert.AreEqual("p1", r3.MyString);
        }

        [TestMethod]
        public void CreateParameterizedConstructor_WithObject()
        {
            var service = this.GetService();

            //var c1 = service.CreateParameterizedConstructor<object>(typeof(string), GetFirstConstructor(typeof(Item)));
            var c2 = service.CreateParameterizedConstructor<object>(typeof(ItemWithString), GetFirstConstructor(typeof(ItemWithString)));
            var c3 = service.CreateParameterizedConstructor<object>(typeof(ViewAWithInjection), GetFirstConstructor(typeof(ViewAWithInjection)));

            //var r1 = c1(new object[] { "p1" });
            var r2 = c2(new object[] { "p1" });
            var r3 = c3(new object[] { "p1" });

            ////Assert.AreEqual(typeof(string), r1.GetType());
            ////Assert.AreEqual("p1", r1);

            Assert.AreEqual(typeof(ItemWithString), r2.GetType());
            Assert.AreEqual("p1", ((ItemWithString)r2).myString);

            Assert.AreEqual(typeof(ViewAWithInjection), r3.GetType());
            Assert.AreEqual("p1", ((ViewAWithInjection)r3).MyString);
        }
    }
}
