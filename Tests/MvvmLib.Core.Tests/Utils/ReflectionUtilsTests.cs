﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Utils;

namespace MvvmLib.Core.Tests.Utils
{
    [TestClass]
    public class ReflectionUtilsTests
    {

        [TestMethod]
        public void GetDefaultConstructor()
        {
            var c1 = ReflectionUtils.GetDefaultConstructor(typeof(MultiCtorClass));
            var p = c1.GetParameters();

            var c2 = ReflectionUtils.GetDefaultConstructor(typeof(ItemWithString));

            var c3 = ReflectionUtils.GetDefaultConstructor(typeof(ClassWithPrivateCtor));
            var c4 = ReflectionUtils.GetDefaultConstructor(typeof(ClassWithPrivateCtor), false);

            Assert.AreEqual(0, p.Length);

            Assert.IsNotNull(c2);

            Assert.IsNotNull(c3);

            Assert.IsNull(c4);
        }

        [TestMethod]
        public void GetConstructors()
        {
            var constructors = ReflectionUtils.GetConstructors(typeof(MultiCtorClass));

            Assert.AreEqual(5, constructors.Length);
        }

        [TestMethod]
        public void GetConstructors_WithPrivate()
        {
            var c1 = ReflectionUtils.GetConstructors(typeof(MultiCtorClassNonPublic));
            var c2 = ReflectionUtils.GetConstructors(typeof(MultiCtorClassNonPublic), false);

            Assert.AreEqual(5, c1.Length);
            Assert.AreEqual(0, c2.Length);
        }

        [TestMethod]
        public void GetFirstParameterizedConstructor()
        {
            var constructor = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClass));
            var p = constructor.GetParameters();

            var constructorPrivate = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClassNonPublic));
            var pPrivate = constructor.GetParameters();

            Assert.AreEqual(1, p.Length);
            Assert.AreEqual(typeof(string), p[0].ParameterType);

            Assert.AreEqual(1, pPrivate.Length);
            Assert.AreEqual(typeof(string), pPrivate[0].ParameterType);
        }

        [TestMethod]
        public void GetParameterizedConstructor()
        {
            var constructor = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClass), new Type[] { typeof(int) });
            var p = constructor.GetParameters();

            var constructorStringString = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClass), new Type[] { typeof(string), typeof(string) });
            var pStringString = constructorStringString.GetParameters();

            var constructorPrivate = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClassNonPublic), new Type[] { typeof(int) });
            var pPrivate = constructor.GetParameters();

            var constructorPrivateStringString = ReflectionUtils.GetParameterizedConstructor(typeof(MultiCtorClassNonPublic), new Type[] { typeof(string), typeof(string) });
            var pPrivateStringString = constructorPrivateStringString.GetParameters();

            Assert.AreEqual(1, p.Length);
            Assert.AreEqual(typeof(int), p[0].ParameterType);

            Assert.AreEqual(2, pStringString.Length);
            Assert.AreEqual(typeof(string), pStringString[0].ParameterType);
            Assert.AreEqual(typeof(string), pStringString[1].ParameterType);

            Assert.AreEqual(1, pPrivate.Length);
            Assert.AreEqual(typeof(int), pPrivate[0].ParameterType);

            Assert.AreEqual(2, pPrivateStringString.Length);
            Assert.AreEqual(typeof(string), pPrivateStringString[0].ParameterType);
            Assert.AreEqual(typeof(string), pPrivateStringString[1].ParameterType);
        }

        // create instance

        [TestMethod]
        public void CreateInstance()
        {
            // ctor with non parameter
            var r1 = ReflectionUtils.CreateInstance(typeof(ItemWithProperties));
            var r2 = ReflectionUtils.CreateInstance(typeof(ItemWithPrivateProperties), false); // internal
            var r3 = ReflectionUtils.CreateInstance(typeof(ItemWithPrivateProperties), true); // internal

            Assert.IsNotNull(r1);
            Assert.IsNull(r2);
            Assert.IsNotNull(r3);
        }

        [TestMethod]
        public void CreateInstance_WithDefaultParameters()
        {
            // ctor with non parameter
            var r1 = ReflectionUtils.CreateInstance(typeof(ItemWithInjection));
            var r2 = ReflectionUtils.CreateInstance(typeof(ItemPrivateWithInjection), false); // internal
            var r3 = ReflectionUtils.CreateInstance(typeof(ItemPrivateWithInjection), true); // internal

            Assert.IsNotNull(r1);
            Assert.IsTrue(((ItemWithInjection)r1).isOk);

            Assert.IsNull(r2);

            Assert.IsNotNull(r3);
            Assert.IsTrue(((ItemPrivateWithInjection)r3).isOk);
        }

        // properties

        [TestMethod]
        public void GetProperties()
        {
            var properties = ReflectionUtils.GetProperties(typeof(ItemWithProperties));

            Assert.AreEqual(1, properties.Length);
            Assert.AreEqual("MyString", properties[0].Name);
        }

        [TestMethod]
        public void GetPrivateProperties()
        {
            var p1 = ReflectionUtils.GetProperties(typeof(ItemWithPrivateProperties), false);
            var p2 = ReflectionUtils.GetProperties(typeof(ItemWithPrivateProperties));

            Assert.AreEqual(0, p1.Length);

            Assert.AreEqual(1, p2.Length);
            Assert.AreEqual("MyString", p2[0].Name);
        }

        // fields
        [TestMethod]
        public void GetFields()
        {
            var fields = ReflectionUtils.GetFields(typeof(ItemWithFields));

            Assert.AreEqual(2, fields.Length);
            Assert.AreEqual("myStringField", fields[0].Name);
        }

        [TestMethod]
        public void GetPrivateFields()
        {
            var fields = ReflectionUtils.GetFields(typeof(ItemWithPrivateFields));
            var fields2 = ReflectionUtils.GetFields(typeof(ItemWithPrivateFields), false);

            Assert.AreEqual(2, fields.Length);
            Assert.AreEqual("myStringField", fields[0].Name);

            Assert.AreEqual(0, fields2.Length);
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

        //[PreferredConstructor]
        public MultiCtorClass(string myString, int myInt)
        {
            MyString = "MyString";
            MyInt = 10;
        }

        public MultiCtorClass(string myString, string myString2)
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
