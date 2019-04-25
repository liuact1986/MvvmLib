using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.IoC.Tests.Options
{

    [TestClass]
    public class TypeRegistrationOptionsTests
    {
        [TestMethod]
        public void Set_OnResolved()
        {
            var registration = new TypeRegistration(typeof(Item), "item", typeof(Item));

            var clearCacheForType = new Action<Type, string>((p, t) => { });

            var service = Activator.CreateInstance(typeof(TypeRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration, clearCacheForType }, null) as TypeRegistrationOptions;

            Assert.AreEqual(null, registration.OnResolved);

            var action = new Action<ContainerRegistration, object>((c, p) => { });

            service.OnResolved(action);

            Assert.AreEqual(action, registration.OnResolved);
        }

        [TestMethod]
        public void Set_As_Singleton()
        {
            var registration = new TypeRegistration(typeof(Item), "item", typeof(Item));

            var clearCacheForType = new Action<Type, string>((p, t) => { });

            var service = Activator.CreateInstance(typeof(TypeRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration, clearCacheForType }, null) as TypeRegistrationOptions;

            Assert.AreEqual(false, registration.IsSingleton);

            service.AsSingleton();

            Assert.AreEqual(true, registration.IsSingleton);

            service.AsMultiInstances();

            Assert.AreEqual(false, registration.IsSingleton);
        }

        [TestMethod]
        public void Set_The_Value_Container()
        {
            var registration = new TypeRegistration(typeof(Item), "item", typeof(Item));

            var clearCacheForType = new Action<Type, string>((p, t) => { });

            var service = Activator.CreateInstance(typeof(TypeRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration, clearCacheForType }, null) as TypeRegistrationOptions;

            Assert.AreEqual(0, registration.ValueContainer.Count);

            var c = new ValueContainer { { "k1", "v1" } };
            service.WithValueContainer(c);

            Assert.AreEqual(c, registration.ValueContainer);
        }

        [TestMethod]
        public void Set_The_Value_Container_Failed_With_Invalid_Values()
        {
            var registration = new TypeRegistration(typeof(Item), "item", typeof(Item));

            var clearCacheForType = new Action<Type, string>((p, t) => { });

            var service = Activator.CreateInstance(typeof(TypeRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration, clearCacheForType }, null) as TypeRegistrationOptions;

            bool failed = false;
            try
            {
                var c = new ValueContainer { { "k1", new Item() } };
                service.WithValueContainer(c);
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }


    }
}
