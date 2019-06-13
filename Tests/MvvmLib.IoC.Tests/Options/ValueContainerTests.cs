using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC.Registrations;
using System;
using System.Collections.Generic;

namespace MvvmLib.IoC.Tests.Options
{
    [TestClass]
    public class ValueContainerTests
    {
        [TestMethod]
        public void Pass()
        {
            bool failed = false;
            try
            {
                var c = new ValueContainer(new Dictionary<string, object> {
                { "MyString", "My value" },
                { "MyInt", 10},
                { "MyDouble", 10.5 },
                {"MyNullable", (int?)10 },
                {"MyNullableNull", (int?)null },
                {"MyUri", new Uri("http://localhost/mysite.com") },
                {"MyList", new List<string>{"a","b" } },

            });
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void Dont_Pass()
        {
            bool failed = false;
            try
            {
                var c = new ValueContainer(new Dictionary<string, object> {
                { "MyString", new Item() }
            });
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void Set_The_Value_Container_Failed_With_Invalid_Values()
        {
            var registration = new TypeRegistration(typeof(Item), "item", typeof(Item));

            bool failed = false;
            try
            {
                registration.ValueContainer["k2"] = new Item();

            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }


        [TestMethod]
        public void CopyTo_Array()
        {
            var v = new ValueContainer { { "k1", "v1" }, { "k2", "v2" }, { "k3", "v3" } };

            var array = new KeyValuePair<string, object>[4];
            array[0] = new KeyValuePair<string, object>("a1", "av1");

            v.CopyTo(array, 1);

            Assert.AreEqual(4, array.Length);
            Assert.AreEqual("a1", array[0].Key);
            Assert.AreEqual("av1", array[0].Value);
            Assert.AreEqual("k1", array[1].Key);
            Assert.AreEqual("v1", array[1].Value);
            Assert.AreEqual("k2", array[2].Key);
            Assert.AreEqual("v2", array[2].Value);
            Assert.AreEqual("k3", array[3].Key);
            Assert.AreEqual("v3", array[3].Value);
        }
    }
}
