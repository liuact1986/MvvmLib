﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using MvvmLib.Wpf.Tests.View;
using MvvmLib.Wpf.Tests.Views;
using System;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class SourceResolverTests
    {
        [TestMethod]
        public void Create_Instance_With_Default_Factory_Works()
        {
            var instance = SourceResolver.CreateInstance(typeof(MyViewA));

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(MyViewA), instance.GetType());
        }

        [TestMethod]
        public void Change_And_Reset_The_Factory()
        {
            var mySourceFactory = new MySourceFactory();

            Assert.AreEqual(false, mySourceFactory.IsCalled);

            SourceResolver.SetFactory(mySourceFactory.CreateInstance);
            var instance = SourceResolver.CreateInstance(typeof(MyViewB));
            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(MyViewB), instance.GetType());
            Assert.AreEqual(true, mySourceFactory.IsCalled);


            SourceResolver.SetFactoryToDefault();
            mySourceFactory.IsCalled = false;
            var instance2 = SourceResolver.CreateInstance(typeof(MyViewB));
            Assert.IsNotNull(instance2);
            Assert.AreEqual(typeof(MyViewB), instance2.GetType());
            Assert.AreEqual(false, mySourceFactory.IsCalled);

        }

        [TestMethod]
        public void Registers_Type_For_Navigation()
        {
            Assert.AreEqual(0, SourceResolver.TypesForNavigation.Count);

            SourceResolver.RegisterTypeForNavigation<MyNavViewA>("A");

            Assert.AreEqual(1, SourceResolver.TypesForNavigation.Count);
            Assert.AreEqual(typeof(MyNavViewA), SourceResolver.TypesForNavigation["A"]);

            SourceResolver.RegisterTypeForNavigation<MyNavViewB>("B");

            Assert.AreEqual(2, SourceResolver.TypesForNavigation.Count);
            Assert.AreEqual(typeof(MyNavViewA), SourceResolver.TypesForNavigation["A"]);
            Assert.AreEqual(typeof(MyNavViewB), SourceResolver.TypesForNavigation["B"]);

            SourceResolver.ClearTypesForNavigation();
        }
    }

    public class MySourceFactory
    {
        public bool IsCalled { get; set; }

        public object CreateInstance(Type sourceType)
        {
            IsCalled = true;
            return Activator.CreateInstance(sourceType);
        }
    }
}
