using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyIoCTestLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvvmLib.IoC.Tests.Scanned
{
    [TestClass]
    public class ScannedTypeManagerTests
    {
        [TestMethod]
        public void Resolve_ImplementationType()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyAwesomeService));

            Assert.AreEqual(typeof(MyAwesomeService), type);
        }

        [TestMethod]
        public void Resolve_ImplementationType_With_PreferredImplementation_Attribute()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyAwesomeServiceB));

            Assert.AreEqual(typeof(MyAwesomeServiceB2), type);
        }

        [TestMethod]
        public void Cannot_Resolve_Multiple_Implementation_WIthout_Attribute()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyAwesomeServiceC));

            Assert.AreEqual(null, type);
        }

        [TestMethod]
        public void Resolve_ImplementationType_From_Another_Assembly()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyExternalService));

            Assert.AreEqual(typeof(MyExternalService), type);
        }

        [TestMethod]
        public void Resolve_ImplementationType_With_PreferredImplementation_Attribute_From_Another_Assembly()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyExternalServiceB));

            Assert.AreEqual(typeof(MyExternalServiceB2), type);
        }

        [TestMethod]
        public void Cannot_Resolve_Multiple_Implementation_WIthout_Attribute_From_Another_Assembly()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyExternalServiceC));

            Assert.AreEqual(null, type);
        }

        [TestMethod]
        public void Cache_The_Assembly()
        {
            var manager = new ScannedTypeManager();
            Assert.AreEqual(0, manager.Assemblies.Count);

            var type = manager.FindImplementationType(typeof(IMyAwesomeService));
            Assert.AreEqual(1, manager.Assemblies.Count);
            var a1 = manager.Assemblies.First();

            manager.FindImplementationType(typeof(IMyAwesomeService));
            Assert.AreEqual(1, manager.Assemblies.Count);

            manager.FindImplementationType(typeof(IMyExternalService));
            Assert.AreEqual(2, manager.Assemblies.Count);
        }

        [TestMethod]
        public void Cache_The_ImplementationType()
        {
            var manager = new ScannedTypeManager();
            var type = manager.FindImplementationType(typeof(IMyAwesomeService));
            Assert.AreEqual(typeof(MyAwesomeService), type);

            Assert.AreEqual(1, manager.ResolvedTypes.Count);
            Assert.AreEqual("MvvmLib.IoC.Tests.Scanned.IMyAwesomeService, MvvmLib.IoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeof(IMyAwesomeService).AssemblyQualifiedName);
            Assert.AreEqual(type, manager.ResolvedTypes[typeof(IMyAwesomeService).AssemblyQualifiedName]);

            var type2 = manager.FindImplementationType(typeof(IMyAwesomeService));
            Assert.AreEqual(type2, type);
        }
    }

    // A
    public interface IMyAwesomeService
    { }

    public class MyAwesomeService : IMyAwesomeService
    { }

    // B
    public interface IMyAwesomeServiceB
    { }

    public class MyAwesomeServiceB1 : IMyAwesomeServiceB
    { }

    [PreferredImplementation]
    public class MyAwesomeServiceB2 : IMyAwesomeServiceB
    { }

    public class MyAwesomeServiceB3 : IMyAwesomeServiceB
    { }

    // C

    public interface IMyAwesomeServiceC
    { }

    public class MyAwesomeServiceC1 : IMyAwesomeServiceC
    { }

    public class MyAwesomeServiceC2 : IMyAwesomeServiceC
    { }
}
