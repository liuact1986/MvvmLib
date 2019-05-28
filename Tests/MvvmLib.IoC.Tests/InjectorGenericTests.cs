using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC;
using MvvmLib.IoC.Tests;
using MvvmLib.IoC.Tests.Scanned;

namespace MvvmLib.Tests.IoC
{
    [TestClass]
    public class InjectorGenericTests
    {
        public Injector GetService()
        {
            return new Injector();
        }

        [TestMethod]
        public void Change_DelegateFactoryType()
        {
            var o = new ObjectCreationManager();
            //var service = new Injector(new TypeInformationManager(), o, new SingletonCache());
            var service = Activator.CreateInstance(typeof(Injector),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { new TypeInformationManager(), o, new SingletonCache(), new ScannedTypeManager() }, null)
                as Injector;

            Assert.AreEqual(DelegateFactoryType.Expression, o.DelegateFactoryType);
            Assert.AreEqual(DelegateFactoryType.Expression, service.DelegateFactoryType);

            service.DelegateFactoryType = DelegateFactoryType.Reflection;

            Assert.AreEqual(DelegateFactoryType.Reflection, o.DelegateFactoryType);
            Assert.AreEqual(DelegateFactoryType.Reflection, service.DelegateFactoryType);
        }

        [TestMethod]
        public void Registered_Is_Invoked()
        {
            var service = GetService();
            int count = 0;
            var t = new List<RegistrationEventArgs>();
            service.Registered += (s, e) =>
            {
                count++;
                t.Add(e);
            };

            service.RegisterFactory<Item>(() => new Item());
            service.RegisterInstance<Item>("k1", new Item { myString = "v1" });
            service.RegisterType<Item>("k2");

            Assert.AreEqual(3, count);
            Assert.AreEqual("___Default___", t[0].Registration.Name);
            Assert.AreEqual(typeof(FactoryRegistration), t[0].Registration.GetType());
            Assert.AreEqual("k1", t[1].Registration.Name);
            Assert.AreEqual(typeof(InstanceRegistration), t[1].Registration.GetType());
            Assert.AreEqual("k2", t[2].Registration.Name);
            Assert.AreEqual(typeof(TypeRegistration), t[2].Registration.GetType());
        }

        [TestMethod]
        public void Resolved_Is_Invoked()
        {
            var service = GetService();
            int count = 0;
            var t = new List<ResolutionEventArgs>();
            service.Resolved += (s, e) =>
            {
                count++;
                t.Add(e);
            };

            service.RegisterFactory<Item>(() => new Item());
            service.RegisterInstance<Item>("k1", new Item { myString = "v1" });
            service.RegisterType<Item>("k2");

            var i1 = service.GetInstance<Item>();
            var i2 = service.GetInstance<Item>("k1");
            var i3 = service.GetInstance<Item>("k2");

            Assert.AreEqual(3, count);
            Assert.AreEqual("___Default___", t[0].Registration.Name);
            Assert.AreEqual(i1, t[0].Instance);
            Assert.AreEqual(typeof(FactoryRegistration), t[0].Registration.GetType());
            Assert.AreEqual("k1", t[1].Registration.Name);
            Assert.AreEqual(i2, t[1].Instance);
            Assert.AreEqual(typeof(InstanceRegistration), t[1].Registration.GetType());
            Assert.AreEqual("k2", t[2].Registration.Name);
            Assert.AreEqual(typeof(TypeRegistration), t[2].Registration.GetType());
            Assert.AreEqual(i3, t[2].Instance);
        }

        // Register Factory

        [TestMethod]
        public void RegisterFactory()
        {
            var service = GetService();

            service.RegisterFactory<Item>(() => new Item());
            service.RegisterFactory<Item>("k2", () => new Item());

            Assert.IsTrue(service.IsRegistered<Item>());
            Assert.IsTrue(service.IsRegistered<Item>("k2"));

            var r1 = service.GetInstance<Item>();

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
                service.RegisterFactory<Item>(() => new Item());
                service.RegisterFactory<Item>(() => new Item());
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
                service.RegisterFactory<Item>("k1", () => new Item());
                service.RegisterFactory<Item>("k1", () => new Item());
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

            service.RegisterInstance<Item>(item);
            service.RegisterInstance<Item>("k2", item2);

            Assert.IsTrue(service.IsRegistered<Item>());
            Assert.IsTrue(service.IsRegistered<Item>("k2"));

            var r1 = service.GetInstance<Item>();
            var r2 = service.GetInstance<Item>("k2");

            Assert.AreEqual("My value", r1.myString);
            Assert.AreEqual("My value 2", r2.myString);
        }

        [TestMethod]
        public void RegisterInstance_WithSameDefaultKey_Throw()
        {
            var service = GetService();

            bool fail = false;
            string error = "";

            try
            {
                var item = new Item() { myString = "My value" };
                var item2 = new Item() { myString = "My value 2" };

                service.RegisterInstance<Item>(item);
                service.RegisterInstance<Item>(item2);
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

                service.RegisterInstance<Item>("k2", item);
                service.RegisterInstance<Item>("k2", item);
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

            service.RegisterType<Item>();
            service.RegisterType<Item>("k2");

            Assert.IsTrue(service.IsRegistered<Item>());
            Assert.IsTrue(service.IsRegistered<Item>("k2"));

            var r1 = service.GetInstance<Item>();
            var r2 = service.GetInstance<Item>("k2");

            Assert.AreEqual("my default value", r1.myString);
            Assert.IsNotNull(r2);
        }

        [TestMethod]
        public void RegisterSingleton()
        {
            var service = GetService();

            service.RegisterSingleton<Item>();

            var r1 = service.GetInstance<Item>();
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // cached instance
            var r2 = service.GetInstance<Item>();
            Assert.AreEqual("My value", r2.myString);
        }

        [TestMethod]
        public void RegisterType_Returns_Aways_A_New_Instance()
        {
            var service = GetService();

            service.RegisterType<Item>();

            var r1 = service.GetInstance<Item>();
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // new instance
            var r2 = service.GetInstance<Item>();
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void RegisterType_WithEmptyConstructor_Not_Ordered()
        {
            var service = GetService();

            service.RegisterType<MultiCtorClassWithEmptyCtorNotTheFirst>();

            Assert.IsTrue(service.IsRegistered<MultiCtorClassWithEmptyCtorNotTheFirst>());

            var r1 = service.GetInstance<MultiCtorClassWithEmptyCtorNotTheFirst>();

            Assert.AreEqual(true, r1.IsEmptyCtorInvoked);
        }

        [TestMethod]
        public void RegisterType_WithPreferredConstructor()
        {
            var service = GetService();

            service.RegisterType<MultiCtorClass>();

            Assert.IsTrue(service.IsRegistered<MultiCtorClass>());

            var r1 = service.GetInstance<MultiCtorClass>();

            Assert.AreEqual("MyString", r1.MyString);
            Assert.AreEqual(10, r1.MyInt);
        }

        [TestMethod]
        public void Register_With_NonPublicConstructors_False_ThrowException()
        {
            var service = GetService();

            service.NonPublicConstructors = false;

            service.RegisterType<ClassWithPrivateCtor>();

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetInstance<ClassWithPrivateCtor>();
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

            service.RegisterType<ClassWithPrivateCtor>();

            var r1 = service.GetInstance<ClassWithPrivateCtor>();

            Assert.IsNotNull(r1);
        }

        [TestMethod]
        public void RegisterType_WithValueContainer()
        {
            var injector = GetService();

            injector.RegisterType<ItemWithString>().WithValueContainer(new ValueContainer { { "myString", "my value" } });


            injector
                .RegisterInstance<Item>(new Item { myString = "my value" });
            injector.RegisterType<ItemWithParameters>().WithValueContainer(
               new ValueContainer {
                   {  "myArray", new string[] { "a", "b" } },
                   { "myString", "my string value" },
                   { "myInt", 10 },
                   { "myBool", true }
               });


            var r1 = injector.GetInstance<ItemWithString>();
            Assert.AreEqual("my value", r1.myString);

            var r2 = injector.GetInstance<ItemWithParameters>();

            Assert.AreEqual("my value", r2.item.myString);
            Assert.AreEqual("b", r2.myArray[1]);
            Assert.AreEqual("my string value", r2.myString);
            Assert.AreEqual(10, r2.myInt);
            Assert.AreEqual(true, r2.myBool);
        }


        [TestMethod]
        public void GetInstance_Returns_Last_Registered_Of_Type()
        {
            var service = GetService();

            service.RegisterType<Item>();
            service.RegisterInstance<Item>("k2", new Item { myString = "Last" });
            service.RegisterType<ItemWithParameter>();

            var r1 = service.GetInstance<ItemWithParameter>();

            Assert.AreEqual("Last", r1.item.myString);
        }

        // register interface + impl

        [TestMethod]
        public void RegisterType_WithInterface()
        {
            var service = GetService();

            service.RegisterType<IItem, Item>();
            service.RegisterType<IItem, Item>("k2");

            Assert.IsTrue(service.IsRegistered<IItem>());
            Assert.IsTrue(service.IsRegistered<IItem>("k2"));

            var r1 = service.GetInstance<IItem>();
            var r2 = service.GetInstance<IItem>("k2");

            Assert.AreEqual("my default value", ((Item)r1).myString);
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
                service.RegisterType<Item>();
                service.RegisterInstance<Item>(new Item { myString = "Last" });
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
        public void GetInstance_Register_UnregisteredType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance<Item>();
            var r2 = service.GetInstance<Item>("k2");

            Assert.IsTrue(service.IsRegistered<Item>());
            Assert.IsTrue(service.IsRegistered<Item>("k2"));

            Assert.AreEqual("my default value", r1.myString);
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetInstance_Register_UnregisteredTypeAndInnerType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance<ItemWithParameter>();

            Assert.IsTrue(service.IsRegistered<ItemWithParameter>());
            Assert.IsTrue(service.IsRegistered<Item>());

            Assert.AreEqual("my default value", r1.item.myString);
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
                var r1 = service.GetInstance<ItemWithParameter>();
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

            var r1 = service.RegisterType<ItemWithParameter>();
            Assert.IsTrue(service.IsRegistered<ItemWithParameter>());

            bool fail = false;
            string error = "";

            try
            {
                var r2 = service.GetInstance<ItemWithParameter>();
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Unable to resolve unregistered parameter \"Item\"", error);
        }


        [TestMethod]
        public void GetInstance_Register_InnerValueType_AndReturnInstance()
        {
            var service = GetService();

            var r1 = service.GetInstance<ItemWithParameters>();

            Assert.AreEqual("my default value", r1.item.myString);
            Assert.AreEqual(null, r1.myArray);
            Assert.AreEqual(null, r1.myString);
            Assert.AreEqual(0, r1.myInt);
            Assert.AreEqual(false, r1.myBool);
        }

        // get new

        [TestMethod]
        public void GetNewInstance()
        {
            var service = GetService();

            service.RegisterType<Item>();

            var r1 = service.GetNewInstance<Item>();
            Assert.AreEqual("my default value", r1.myString);

            r1.myString = "My value";

            // new instance
            var r2 = service.GetNewInstance<Item>();
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetNewInstance_Register_Unregistered()
        {
            var service = GetService();

            var r1 = service.GetNewInstance<Item>();
            Assert.AreEqual("my default value", r1.myString);

            Assert.IsTrue(service.IsRegistered<Item>());

            r1.myString = "My value";

            // new instance
            var r2 = service.GetNewInstance<Item>();
            Assert.AreEqual("my default value", r2.myString);
        }

        [TestMethod]
        public void GetNewInstance_WithNot_A_Type_Fail()
        {
            var service = GetService();

            service.RegisterInstance<Item>(new Item());

            bool fail = false;
            string error = "";

            try
            {
                var r1 = service.GetNewInstance<Item>();
            }
            catch (Exception ex)
            {
                fail = true;
                error = ex.Message;
            }

            Assert.IsTrue(fail);
            Assert.AreEqual("Cannot get a new instance for the registration type \"InstanceRegistration\"", error);
        }

        [TestMethod]
        public void Register_With_Func()
        {
            var service = GetService();

            service.AutoDiscovery = false;

            service.RegisterInstance<Item>(new Item { myString = "My item" });
            service.RegisterType<ClassWithFunc>();

            var instance = service.GetInstance<ClassWithFunc>();

            Assert.AreEqual("My item", instance.Result.myString);
        }


        [TestMethod]
        public void Register_With_Func_Resolve_Not_Registered()
        {
            var service = GetService();

            var instance = service.GetInstance<ClassWithFunc>();

            Assert.AreEqual("my default value", instance.Result.myString);
        }

        [TestMethod]
        public void Register_PropertyInjection()
        {
            var service = GetService();

            service.RegisterType<ItemWithPropertyInjection>().OnResolved((registration, instance) =>
            {
                var item = instance as ItemWithPropertyInjection;
                item.MyString = "My value";
                item.MyItem = service.GetInstance<Item>();
            });

            var r = service.GetInstance<ItemWithPropertyInjection>();

            Assert.AreEqual("My value", r.MyString);
        }

        [TestMethod]
        public void Register_PropertyInjection_Circular()
        {
            var service = GetService();

            service.RegisterType<CircularPropertyItem>().OnResolved((registration, instance) =>
            {
                var item = instance as CircularPropertyItem;
                item.Item = item;
            });

            var r = service.GetInstance<CircularPropertyItem>();

            Assert.IsNotNull(r.Item);
            Assert.IsNotNull(r.Item.Item);
        }

        [TestMethod]
        public void BuildUp()
        {
            var service = GetService();

            service.RegisterType<Item>();
            service.RegisterType<ItemBuildUp>();

            var r = service.BuildUp<ItemBuildUp>();

            Assert.IsNotNull(r.MyItem);
            Assert.IsNull(r.MyItem2);
        }

        [TestMethod]
        public void BuildUp_With_Named()
        {
            var service = GetService();

            service.RegisterInstance<Item>(new Item { myString = "v1" });
            service.RegisterInstance<Item>("k2", new Item { myString = "v2" });

            service.RegisterType<ItemBuildUpNamed>();

            var r = service.BuildUp<ItemBuildUpNamed>();

            Assert.IsNotNull(r.MyItem);
            Assert.AreEqual("v2", r.MyItem.myString);
            Assert.IsNull(r.MyItem2);
        }

        [TestMethod]
        public void BuildUp_Resolve_NotRegistered()
        {
            var service = GetService();

            var r = service.BuildUp<ItemBuildUp>();

            Assert.IsNotNull(r.MyItem);
            Assert.IsNull(r.MyItem2);
        }

        [TestMethod]
        public void BuildUp_WithValue()
        {
            var service = GetService();

            service.RegisterType<Item>();
            service.RegisterType<ItemBuildUpWithValue>()
                .WithValueContainer(new ValueContainer { { "MyString", "My string property injected" } });

            var r = service.BuildUp<ItemBuildUpWithValue>();

            Assert.AreEqual("My string property injected", r.MyString);
        }

        [TestMethod]
        public void BuildUp_Instance()
        {
            var service = GetService();

            var instance = new ItemBuildUpNamed { MyItem2 = new Item { myString = "instance value" } };

            service.RegisterInstance<Item>("k2", new Item { myString = "v2" });
            var r = service.BuildUp<ItemBuildUpNamed>(instance);

            Assert.AreEqual("v2", r.MyItem.myString);
            Assert.AreEqual("instance value", r.MyItem2.myString);
        }

        [TestMethod]
        public void BuildUp_WithInterface()
        {
            var service = GetService();

            service.RegisterInstance<IItem>(new Item { myString = "v1" });
            service.RegisterInstance<IItem>("k2", new Item { myString = "v2" });

            var r = service.BuildUp<ItemBuildUpWithInterface>();

            Assert.AreEqual("v1", ((Item)r.MyItem).myString);
            Assert.AreEqual("v2", ((Item)r.MyItem2).myString);
        }

        //[TestMethod]
        //public void GetInstance_Circular()
        //{
        //    var service = GetService();

        //    service.RegisterType<CircularItem>();

        //    var instance = service.GetInstance<CircularItem>();

        //    Assert.IsNotNull(instance.Item);
        //    Assert.IsNotNull(instance.Item.Item);
        //}


        [TestMethod]
        public void RegisterType_With_Interfaces()
        {
            var service = GetService();

            var container = service.RegisterTypeWithInterfaces<LookupDataService>();

            var registrationOptions2 = container[typeof(ILookupDataServiceType2)];
            registrationOptions2.AsSingleton();

            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType1>());
            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType2>());
            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType3>());
            Assert.AreEqual(false, service.IsRegistered<IDisposable>());

            var i1 = service.GetInstance<ILookupDataServiceType1>();
            var i2 = service.GetInstance<ILookupDataServiceType2>();
            var i3 = service.GetInstance<ILookupDataServiceType3>();

            Assert.AreEqual(typeof(LookupDataService), i1.GetType());
            Assert.AreEqual(typeof(LookupDataService), i2.GetType());
            Assert.AreEqual(typeof(LookupDataService), i3.GetType());

            Assert.AreEqual("default 1", i1.Message1);
            Assert.AreEqual("default 2", i2.Message2);
            Assert.AreEqual("default 3", i3.Message3);

            i1.Message1 = "My message 1";
            i2.Message2 = "My message 2";
            i3.Message3 = "My message 3";

            var i1b = service.GetInstance<ILookupDataServiceType1>();
            var i2b = service.GetInstance<ILookupDataServiceType2>();
            var i3b = service.GetInstance<ILookupDataServiceType3>();

            Assert.AreEqual("default 1", i1b.Message1);
            Assert.AreEqual("My message 2", i2b.Message2);
            Assert.AreEqual("default 3", i3b.Message3);
        }

        [TestMethod]
        public void RegisterSingleton_With_Interfaces()
        {
            var service = GetService();

            var container = service.RegisterSingletonWithInterfaces<LookupDataService>();

            var registrationOptions2 = container[typeof(ILookupDataServiceType2)];
            registrationOptions2.AsMultiInstances();

            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType1>());
            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType2>());
            Assert.AreEqual(true, service.IsRegistered<ILookupDataServiceType3>());
            Assert.AreEqual(false, service.IsRegistered<IDisposable>());

            var i1 = service.GetInstance<ILookupDataServiceType1>();
            var i2 = service.GetInstance<ILookupDataServiceType2>();
            var i3 = service.GetInstance<ILookupDataServiceType3>();

            Assert.AreEqual(typeof(LookupDataService), i1.GetType());
            Assert.AreEqual(typeof(LookupDataService), i2.GetType());
            Assert.AreEqual(typeof(LookupDataService), i3.GetType());

            Assert.AreEqual("default 1", i1.Message1);
            Assert.AreEqual("default 2", i2.Message2);
            Assert.AreEqual("default 3", i3.Message3);

            i1.Message1 = "My message 1";
            i2.Message2 = "My message 2";
            i3.Message3 = "My message 3";

            var i1b = service.GetInstance<ILookupDataServiceType1>();
            var i2b = service.GetInstance<ILookupDataServiceType2>();
            var i3b = service.GetInstance<ILookupDataServiceType3>();

            Assert.AreEqual("My message 1", i1b.Message1);
            Assert.AreEqual("default 2", i2b.Message2);
            Assert.AreEqual("My message 3", i3b.Message3);

            // same instance
            Assert.AreEqual(i1b, i3b);
        }

        [TestMethod]
        public void Auto_Registering_With_Interface()
        {
            var injector = new Injector();
            var instance = injector.GetInstance<IMyAwesomeService>();

            Assert.AreEqual(typeof(MyAwesomeService), instance.GetType());
        }

        [TestMethod]
        public void Auto_Registering_With_Interface_With_PreferredImplementation_Attribute()
        {
            var injector = new Injector();
            var instance = injector.GetInstance<IMyAwesomeServiceB>();

            Assert.AreEqual(typeof(MyAwesomeServiceB2), instance.GetType());
        }

        [TestMethod]
        public void Auto_Registering_Cannot_Resolve_Multiple_Implementation_WIthout_Attribute()
        {
            var injector = new Injector();
            bool failed = false;
            string error = null;
            try
            {
                var instance = injector.GetInstance<IMyAwesomeServiceC>();
            }
            catch (Exception ex)
            {
                failed = true;
                error = ex.Message;
            }

            Assert.AreEqual(true, failed);
            Assert.AreEqual("Unable to resolve the type for the interface not registered \"IMyAwesomeServiceC\"", error);
        }

        [TestMethod]
        public void Auto_Registering_With_Interface_That_Have_No_Implemntation()
        {
            var injector = new Injector();
            bool failed = false;
            string error = null;
            try
            {
                var instance = injector.GetInstance<IMYInterfaceWithNoImplementation>();
            }
            catch (Exception ex)
            {
                failed = true;
                error = ex.Message;
            }

            Assert.AreEqual(true, failed);
            Assert.AreEqual("Unable to resolve the type for the interface not registered \"IMYInterfaceWithNoImplementation\"", error);
        }

        [TestMethod]
        public void Auto_Registering_With_Interface_Inherit_From_Interface()
        {
            var injector = new Injector();
            var instance = injector.GetInstance<IMyChildInterfaceA>();

            Assert.AreEqual(typeof(MyChildA), instance.GetType());
        }

        [TestMethod]
        public void Auto_Registering_With_Base_Interface()
        {
            var injector = new Injector();
            var instance = injector.GetInstance<IMyBaseInterface>();

            Assert.AreEqual(typeof(MyChildA), instance.GetType());
        }

        [TestMethod]
        public void Auto_Registering_With_Base_Interface_With_Many_Interface_Implementation()
        {
            var injector = new Injector();
            bool failed = false;
            string error = null;
            try
            {
                var instance = injector.GetInstance<IMyBaseInterfaceB>();
            }
            catch (Exception ex)
            {
                failed = true;
                error = ex.Message;
            }

            Assert.AreEqual(true, failed);
            Assert.AreEqual("Unable to resolve the type for the interface not registered \"IMyBaseInterfaceB\"", error);
        }

        [TestMethod]
        public void Auto_Registering_With_Base_Interface_With_Each_Interface_And_Implementation()
        {
            var injector = new Injector();
            var instance1 = injector.GetInstance<IMyChildInterfaceB1>();
            Assert.AreEqual(typeof(MyChildB1), instance1.GetType());

            var instance2 = injector.GetInstance<IMyChildInterfaceB2>();
            Assert.AreEqual(typeof(MyChildB2), instance2.GetType());
        }

        [TestMethod]
        public void Auto_Registering_With_Base_Interface_Resolve_Injected_Parameter_With_Interface()
        {
            var injector = new Injector();
            var instance = injector.GetInstance<IMyServiceC>();
            Assert.AreEqual(typeof(MyServiceC), instance.GetType());
            Assert.IsNotNull(((MyServiceC) instance).ChildA);
        }

        [TestMethod]
        public void GetService_With_Lifetime()
        {
            var injector = GetService();
            var instance = injector.GetInstance<IMyAwesomeService>();
            Assert.AreEqual(typeof(MyAwesomeService), instance.GetType());

            injector.LifetimeOnDiscovery = LifetimeOnDiscovery.SingletonOnlyForServices;

            var instance2 = injector.GetInstance<IMyAwesomeService>();
            Assert.AreNotEqual(instance2, instance);

            var instance3 = injector.GetInstance<IMyAwesomeService>();
            Assert.AreNotEqual(instance3, instance2);
        }
    }

    // A
    public interface IMyBaseInterface { }

    public interface IMyChildInterfaceA : IMyBaseInterface
    {

    }

    public class MyChildA : IMyChildInterfaceA
    {

    }

    // 

    public interface IMYInterfaceWithNoImplementation
    {

    }

    // B
    public interface IMyBaseInterfaceB { }

    public interface IMyChildInterfaceB1 : IMyBaseInterfaceB
    { }

    public interface IMyChildInterfaceB2 : IMyBaseInterfaceB
    { }

    public class MyChildB1 : IMyChildInterfaceB1
    {

    }

    public class MyChildB2 : IMyChildInterfaceB2
    {

    }

    // C

    public interface IMyServiceC { }

    public class MyServiceC : IMyServiceC
    {
        public IMyChildInterfaceA ChildA { get; }

        public MyServiceC(IMyChildInterfaceA childA)
        {
            ChildA = childA;
        }
    }

    public interface ILookupDataServiceType1
    {
        string Message1 { get; set; }
    }

    public interface ILookupDataServiceType2
    {
        string Message2 { get; set; }
    }

    public interface ILookupDataServiceType3
    {
        string Message3 { get; set; }
    }

    public class LookupDataService : ILookupDataServiceType1, ILookupDataServiceType2, ILookupDataServiceType3, IDisposable
    {
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }

        public LookupDataService()
        {
            Message1 = "default 1";
            Message2 = "default 2";
            Message3 = "default 3";
        }

        public void Dispose()
        {

        }
    }

    public class ItemBuildUpWithInterface
    {
        [Dependency]
        public IItem MyItem { get; set; }

        [Dependency(Name = "k2")]
        public IItem MyItem2 { get; set; }
    }


    public class ItemBuildUpWithValue
    {
        [Dependency]
        public string MyString { get; set; }
    }

    public class ItemBuildUp
    {
        [Dependency]
        public Item MyItem { get; set; }

        public Item MyItem2 { get; set; }
    }

    public class ItemBuildUpNamed
    {
        [Dependency(Name = "k2")]
        public Item MyItem { get; set; }

        public Item MyItem2 { get; set; }
    }

    public class ItemWithPropertyInjection
    {
        public string MyString { get; set; }

        public Item MyItem { get; set; }
    }

    public class CircularPropertyItem
    {
        public CircularPropertyItem Item { get; set; }
    }

    public class CircularItem
    {
        public CircularItem Item { get; set; }

        public CircularItem(CircularItem item)
        {
            this.Item = item;
        }
    }

    public class ClassWithFunc
    {
        public Item Result { get; set; }

        public ClassWithFunc(Func<Item> myFunc)
        {
            Result = myFunc();
        }
    }


    public class ItemWithFields
    {
        public string myStringField;

        public string MyString { get; set; }
    }

    public class ItemWithPrivateFields
    {
        private string myStringField;

        internal string MyString { get; set; }
    }

    public class ItemWithInjection
    {
        public bool isOk;

        public ItemWithInjection(int myValueType, string myString, Uri myUri, int? myNullable)
        {
            isOk = true;
        }
    }

    public class ItemPrivateWithInjection
    {
        public bool isOk;

        internal ItemPrivateWithInjection(int myValueType, string myString, Uri myUri, int? myNullable)
        {
            isOk = true;
        }
    }

    public class ItemWithProperties
    {
        public string MyString { get; set; }
    }

    public class ItemWithPrivateProperties
    {
        internal ItemWithPrivateProperties()
        {

        }

        internal string MyString { get; set; }
    }

    public class ClassWithPrivateCtor
    {
        private ClassWithPrivateCtor()
        {

        }
    }

    public class MultiCtorClass
    {
        public string MyString { get; set; }
        public int MyInt { get; set; }

        public MultiCtorClass()
        {

        }

        public MultiCtorClass(string myString)
        {

        }

        public MultiCtorClass(int myInt)
        {

        }

        [PreferredConstructor]
        public MultiCtorClass(string myString, int myInt)
        {
            MyString = "MyString";
            MyInt = 10;
        }

        public MultiCtorClass(string myString, string myString2)
        {

        }
    }

    public class MultiCtorClassWithEmptyCtorNotTheFirst
    {
        public string MyString { get; set; }
        public int MyInt { get; set; }
        public bool IsEmptyCtorInvoked { get; }

        public MultiCtorClassWithEmptyCtorNotTheFirst(string myString)
        {

        }

        public MultiCtorClassWithEmptyCtorNotTheFirst(int myInt)
        {

        }

        public MultiCtorClassWithEmptyCtorNotTheFirst(string myString, int myInt)
        {
            MyString = "MyString";
            MyInt = 10;
        }

        public MultiCtorClassWithEmptyCtorNotTheFirst()
        {
            this.IsEmptyCtorInvoked = true;
        }

        public MultiCtorClassWithEmptyCtorNotTheFirst(string myString, string myString2)
        {

        }
    }

    public class MultiCtorClassNonPublic
    {
        private MultiCtorClassNonPublic()
        {

        }

        private MultiCtorClassNonPublic(string myString)
        {

        }

        private MultiCtorClassNonPublic(int myInt)
        {

        }

        private MultiCtorClassNonPublic(string myString, int myInt)
        {

        }

        private MultiCtorClassNonPublic(string myString, string myString2)
        {

        }
    }
}
