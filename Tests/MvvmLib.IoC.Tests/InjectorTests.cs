using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC;
using MvvmLib.IoC.Tests;

namespace MvvmLib.Tests.IoC
{
    [TestClass]
    public class InjectorTests
    {
        // register type <T, TImplementation> and <T>
        // register singleton  <T, TImplementation> and <T>
        // register instance <T>
        // register factory <T> (()=>)


        // Get Instance
        //      - automatic discover non registered types <T>
        // use expression factory or reflection factory, ...
        // Get New instance for singleton or types

        public Injector GetService()
        {
            return new Injector();
        }

        // Register Factory

        [TestMethod]
        public void RegisterFactory()
        {
            var service = GetService();

            service.RegisterFactory(typeof(Item), () => new Item());
            service.RegisterFactory(typeof(Item), "k2", () => new Item());

            Assert.IsTrue(service.IsRegistered(typeof(Item)));
            Assert.IsTrue(service.IsRegistered(typeof(Item), "k2"));

            var r1 = service.GetInstance(typeof(Item)) as Item;

            Assert.AreEqual("my default value", r1.myString);
        }

        [TestMethod]
        public void RegisterFactory_WithSameDefaultKey_Throw()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                service.RegisterFactory(typeof(Item), () => new Item());
                service.RegisterFactory(typeof(Item), () => new Item());
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }
            Assert.IsTrue(fail);
            Assert.AreEqual("A type \"Item\" is already registered", error);
        }

        [TestMethod]
        public void RegisterFactory_WithSameKey_Throw()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                service.RegisterFactory(typeof(Item), "k1", () => new Item());
                service.RegisterFactory(typeof(Item), "k1", () => new Item());
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }
            Assert.IsTrue(fail);
            Assert.AreEqual("A type \"Item\" with the name \"k1\" is already registered", error);
        }

        // Register Instance

        [TestMethod]
        public void RegisterInstance()
        {
            var service = GetService();

            var item = new Item() { myString = "My value" };
            var item2 = new Item() { myString = "My value 2" };

            service.RegisterInstance(typeof(Item), item);
            service.RegisterInstance(typeof(Item), "k2", item2);

            Assert.IsTrue(service.IsRegistered(typeof(Item)));
            Assert.IsTrue(service.IsRegistered(typeof(Item), "k2"));

            var r1 = service.GetInstance(typeof(Item)) as Item;
            var r2 = service.GetInstance(typeof(Item), "k2") as Item;

            Assert.AreEqual("My value", r1.myString);
            Assert.AreEqual("My value 2", r2.myString);
        }

        [TestMethod]
        public void RegisterInstance_WithSameDefaultKey_Throw()
        {
            var service = GetService();

            var injector = new Injector();

            bool fail = false;
            string error = "";

            try
            {
                var item = new Item() { myString = "My value" };
                var item2 = new Item() { myString = "My value 2" };

                service.RegisterInstance(typeof(Item), item);
                service.RegisterInstance(typeof(Item), item2);
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }
            Assert.IsTrue(fail);
            Assert.AreEqual("A type \"Item\" is already registered", error);
        }

        [TestMethod]
        public void RegisterInstance_WithSameKey_Throw()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                var item = new Item() { myString = "My value" };
                var item2 = new Item() { myString = "My value 2" };

                service.RegisterInstance(typeof(Item), "k2", item);
                service.RegisterInstance(typeof(Item), "k2", item);
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }
            Assert.IsTrue(fail);
            Assert.AreEqual("A type \"Item\" with the name \"k2\" is already registered", error);
        }

        // register type

        [TestMethod]
        public void RegisterType()
        {
            var service = GetService();

            service.RegisterType(typeof(Item));
            service.RegisterType(typeof(Item), "k2");

            Assert.IsTrue(service.IsRegistered(typeof(Item)));
            Assert.IsTrue(service.IsRegistered(typeof(Item), "k2"));

            var r1 = service.GetInstance(typeof(Item)) as Item;
            var r2 = service.GetInstance(typeof(Item), "k2") as Item;

            Assert.AreEqual("my default value", r1.myString);
            Assert.IsNotNull(r2);
        }

        [TestMethod]
        public void RegisterSingleton()
        {
            var service = GetService();

            service.RegisterType(typeof(Item)).AsSingleton();

            var r1 = service.GetInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // cached instance
            var r2 = service.GetInstance(typeof(Item)) as Item;
            Assert.AreEqual("My value", r2.myString);
        }

        [TestMethod]
        public void RegisterType_Returns_Aways_A_New_Instance()
        {
            var service = GetService();

            service.RegisterType(typeof(Item));

            var r1 = service.GetInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // new instance
            var r2 = service.GetInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r2.myString);
        }


        [TestMethod]
        public void RegisterType_WithPreferredConstructor()
        {
            var service = GetService();

            service.RegisterType(typeof(MultiCtorClass));

            Assert.IsTrue(service.IsRegistered(typeof(MultiCtorClass)));

            var r1 = service.GetInstance(typeof(MultiCtorClass)) as MultiCtorClass;

            Assert.AreEqual("MyString", r1.MyString);
            Assert.AreEqual(10, r1.MyInt);
        }

        [TestMethod]
        public void Register_With_NonPublicConstructors_False_ThrowException()
        {
            var service = GetService();

            service.NonPublicConstructors = false;

            service.RegisterType(typeof(ClassWithPrivateCtor));

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetInstance(typeof(ClassWithPrivateCtor)) as ClassWithPrivateCtor;
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }
            Assert.IsTrue(fail);
            Assert.AreEqual("Unable to resolve a constructor for type \"ClassWithPrivateCtor\" with non public \"False\"", error);
        }

        [TestMethod]
        public void Register_With_NonPublicConstructors_True_FindPrivateConstructor()
        {
            var service = GetService();

            service.NonPublicConstructors = true;

            service.RegisterType(typeof(ClassWithPrivateCtor));

            var r1 = service.GetInstance(typeof(ClassWithPrivateCtor)) as ClassWithPrivateCtor;

            Assert.IsNotNull(r1);
        }

        [TestMethod]
        public void GetInstance_Returns_Last_Registered_Of_Type()
        {
            var service = GetService();

            service.RegisterType(typeof(Item));
            service.RegisterInstance(typeof(Item), "k2", new Item { myString = "Last" });
            service.RegisterType(typeof(ItemWithParameter));

            var r1 = service.GetInstance(typeof(ItemWithParameter)) as ItemWithParameter;

            Assert.AreEqual("Last", r1.item.myString);
        }

        [TestMethod]
        public void RegisterType_WithValueContainer()
        {
            var injector = GetService();

            injector.RegisterType(typeof(ItemWithString)).WithValueContainer(new Dictionary<string, object> { { "myString", "my value" } });


            injector
                .RegisterInstance(typeof(Item), new Item { myString = "my value" });
            injector.RegisterType(typeof(ItemWithParameters)).WithValueContainer(
                new Dictionary<string, object> {
                   {  "myArray", new string[] { "a", "b" } },
                   { "myString", "my string value" },
                   { "myInt", 10 },
                   { "myBool", true }
               });


            var r1 = injector.GetInstance(typeof(ItemWithString)) as ItemWithString;
            Assert.AreEqual("my value", r1.myString);

            var r2 = injector.GetInstance(typeof(ItemWithParameters)) as ItemWithParameters;

            Assert.AreEqual("my value", r2.item.myString);
            Assert.AreEqual("b", r2.myArray[1]);
            Assert.AreEqual("my string value", r2.myString);
            Assert.AreEqual(10, r2.myInt);
            Assert.AreEqual(true, r2.myBool);
        }

        // register interface + impl

        [TestMethod]
        public void RegisterType_WithInterface()
        {
            var service = GetService();

            service.RegisterType(typeof(IItem), typeof(Item));
            service.RegisterType(typeof(IItem), "k2", typeof(Item));

            Assert.IsTrue(service.IsRegistered(typeof(IItem)));
            Assert.IsTrue(service.IsRegistered(typeof(IItem), "k2"));

            var r1 = service.GetInstance(typeof(IItem)) as Item;
            var r2 = service.GetInstance(typeof(IItem), "k2") as Item;

            Assert.AreEqual("my default value", r1.myString);
            Assert.IsNotNull(r2);
        }


        // register instance and type with same key

        [TestMethod]
        public void RegisterType_And_Instance_With_SameKey_Throw()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                service.RegisterType(typeof(Item));
                service.RegisterInstance(typeof(Item), new Item { myString = "Last" });
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("A type \"Item\" is already registered", error);
        }

        [TestMethod]
        public void GetAll()
        {
            var service = GetService();

            service.RegisterType(typeof(IItem), typeof(Item));
            service.RegisterInstance(typeof(IItem), "k2", new Item { myString = "My value" });

            var r1 = service.GetAllInstances(typeof(IItem)) as List<object>;

            Assert.IsNotNull(r1);
        }

        [TestMethod]
        public void GetAll_Generic()
        {
            var service = GetService();

            service.RegisterType(typeof(Item));
            service.RegisterInstance(typeof(Item), "k2", new Item { myString = "My value" });

            var r1 = service.GetAllInstances<Item>();

            Assert.IsNotNull(r1);
        }

        [TestMethod]
        public void GetInstance_Register_UnregisteredType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance(typeof(Item)) as Item;
            var r2 = service.GetInstance(typeof(Item), "k2") as Item;

            Assert.IsTrue(service.IsRegistered(typeof(Item)));
            Assert.IsTrue(service.IsRegistered(typeof(Item), "k2"));

            Assert.AreEqual("my default value", r1.myString);
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetInstance_Register_UnregisteredTypeAndInnerType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance(typeof(ItemWithParameter)) as ItemWithParameter;

            Assert.IsTrue(service.IsRegistered(typeof(ItemWithParameter)));
            Assert.IsTrue(service.IsRegistered(typeof(Item)));

            Assert.AreEqual("my default value", r1.item.myString);
        }

        [TestMethod]
        public void AutoDiscory_With_Interface_Fail()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetInstance(typeof(IMyService));
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Cannot resolve the unregistered type for \"IMyService\"", error);
        }

        [TestMethod]
        public void Not_Resolve_UnregisteredType_WithProperty_ResolveUnregistered_False()
        {
            var service = GetService();

            service.AutoDiscovery = false;

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetInstance(typeof(ItemWithParameter)) as ItemWithParameter;
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Type \"ItemWithParameter\" not registered", error);
        }

        [TestMethod]
        public void Not_Resolve_Inner_UnregisteredType_WithProperty_ResolveUnregistered_False()
        {
            var service = GetService();

            service.AutoDiscovery = false;

            var r1 = service.RegisterType(typeof(ItemWithParameter));
            Assert.IsTrue(service.IsRegistered(typeof(ItemWithParameter)));

            bool fail = false;
            string error = "";

            try
            {
                var r2 = service.GetInstance(typeof(ItemWithParameter)) as ItemWithParameter;
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Cannot resolve unregistered parameter \"Item\"", error);
        }


        [TestMethod]
        public void GetInstance_Register_InnerValueType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance(typeof(ItemWithParameters)) as ItemWithParameters;

            Assert.AreEqual("my default value", r1.item.myString);
            Assert.AreEqual(null, r1.myArray);
            Assert.AreEqual(null, r1.myString);
            Assert.AreEqual(0, r1.myInt);
            Assert.AreEqual(false, r1.myBool);
        }

        // Get new 

        [TestMethod]
        public void GetNewInstance()
        {
            var service = GetService();

            service.RegisterType(typeof(Item));

            var r1 = service.GetNewInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // new instance
            var r2 = service.GetNewInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetNewInstance_Register_Unregistered()
        {
            var service = GetService();

            var r1 = service.GetNewInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r1.myString);

            Assert.IsTrue(service.IsRegistered(typeof(Item)));

            r1.myString = "My value";

            // new instance
            var r2 = service.GetNewInstance(typeof(Item)) as Item;
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetNewInstance_WithNot_A_Type_Fail()
        {
            var service = GetService();

            service.RegisterInstance(typeof(Item), new Item());

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetNewInstance(typeof(Item)) as Item;
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Cannot get a new instance for the registration type \"InstanceRegistration\"", error);
        }

        // unregister

        [TestMethod]
        public void UnregisterType()
        {
            var t = new TypeInformationManager();
            var o = new ObjectCreationManager();
            var c = new SingletonCache();

            var injector = Activator.CreateInstance(typeof(Injector),
                      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { t, o, c }, null)
                      as Injector;

            injector.RegisterSingleton<Item>();

            Assert.IsTrue(injector.IsRegistered<Item>());

            var instance = injector.GetInstance<Item>();

            Assert.IsTrue(c.Cache.ContainsKey(typeof(Item)));

            Assert.IsTrue(injector.Unregister<Item>());

            Assert.IsFalse(injector.IsRegistered<Item>());
            Assert.IsFalse(c.Cache.ContainsKey(typeof(Item)));
        }

        [TestMethod]
        public void UnregisterInstance()
        {
            var t = new TypeInformationManager();
            var o = new ObjectCreationManager();
            var c = new SingletonCache();
            var injector = Activator.CreateInstance(typeof(Injector),
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { t, o, c }, null)
              as Injector;

            var item2 = new Item { myString = "with k1" };

            injector.RegisterType(typeof(Item));
            injector.RegisterInstance(typeof(Item), "k1", item2);

            Assert.IsTrue(injector.IsRegistered<Item>());
            Assert.IsTrue(injector.IsRegistered<Item>("k1"));

            Assert.IsTrue(injector.Unregister<Item>());

            Assert.IsFalse(injector.IsRegistered<Item>());
            Assert.IsTrue(injector.IsRegistered<Item>("k1"));

            Assert.IsTrue(injector.Unregister<Item>("k1"));
            Assert.IsFalse(injector.IsRegistered<Item>("k1"));
        }

        [TestMethod]
        public void UnregisterAll()
        {
            var injector = GetService();

            var item1 = new Item { myString = "with default key" };
            var item2 = new Item { myString = "with k1" };

            injector
                .RegisterInstance(typeof(Item), item1);
            injector.RegisterInstance(typeof(Item), "k1", item2);

            Assert.IsTrue(injector.IsRegistered<Item>());
            Assert.IsTrue(injector.IsRegistered<Item>("k1"));

            Assert.IsTrue(injector.UnregisterAll<Item>());

            Assert.IsFalse(injector.IsRegistered<Item>());
            Assert.IsFalse(injector.IsRegistered<Item>("k1"));
        }

        //[TestMethod]
        //public void Clear()
        //{
        //    var injector = GetService();
        //    var item2 = new Item { myString = "with k1" };

        //    injector.RegisterSingleton<Item>();
        //    injector.RegisterInstance(typeof(Item), item2, "k1");

        //    injector.GetInstance<Item>();

        //    Assert.IsTrue(injector.IsCached(typeof(Item)));

        //    injector.Clear();

        //    Assert.IsFalse(injector.IsRegistered<Item>());
        //    Assert.IsFalse(injector.IsRegistered<Item>("k1"));
        //    Assert.IsFalse(injector.IsCached(typeof(Item)));
        //}
    }
}
