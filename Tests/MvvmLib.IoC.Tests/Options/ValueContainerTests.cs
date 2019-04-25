using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
