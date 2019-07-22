using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Interactivity;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MvvmLib.Wpf.Tests.Mvvm
{

    [TestClass]
    public class PropertyFilterTests
    {
        [TestMethod]
        public void Creates_Filter_On_Creation()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsEqual, "Value B");

            Assert.IsNotNull(filter.Filter);
            Assert.IsNotNull(filter.Expression);
            Assert.AreEqual(CultureInfo.InvariantCulture, filter.Culture);
            Assert.AreEqual(false, filter.IsCaseSensitive);
        }

        [TestMethod]
        public void IsEqual_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsEqual, "Value B");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void IsEqual_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsEqual, 10);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 10 }));
        }

        [TestMethod]
        public void IsEqual_With_Double()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyDouble", PredicateOperator.IsEqual, 10.5);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDouble = 20.9 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDouble = 10.5 }));
        }

        [TestMethod]
        public void Converts_Value()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyDouble", PredicateOperator.IsEqual, "10.5");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDouble = 20.9 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDouble = 10.5 }));
        }

        [TestMethod]
        public void Converts_Value_With_Culture()
        {
            // double with comma + culture
            var filter = new PropertyFilter<MyFilteredItem>("MyDouble", PredicateOperator.IsEqual, "10,5", CultureInfo.GetCultureInfo("fr"));

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDouble = 20.9 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDouble = 10.5 }));
        }

        [TestMethod]
        public void IsEqual_With_Date()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyDate", PredicateOperator.IsEqual, new DateTime(2019, 07, 18));

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDate = DateTime.Now }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDate = new DateTime(2019, 07, 18) }));
        }

        [TestMethod]
        public void IsEqual_With_Date_As_String_Invariant_Culture()
        {
            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.InvariantCulture); // 18/07/2019 00:00:00 => invariant 07/18/2019 00:00:00

            var filter = new PropertyFilter<MyFilteredItem>("MyDate", PredicateOperator.IsEqual, dateAsString);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDate = DateTime.Now }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDate = new DateTime(2019, 07, 18) }));
        }

        [TestMethod]
        public void IsEqual_With_Date_As_String_Culture()
        {
            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00

            var filter = new PropertyFilter<MyFilteredItem>("MyDate", PredicateOperator.IsEqual, dateAsString, CultureInfo.GetCultureInfo("fr"));

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyDate = DateTime.Now }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyDate = new DateTime(2019, 07, 18) }));
        }

        [TestMethod]
        public void IsEqual_With_Inner_Object()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInner.MyInnerString", PredicateOperator.IsEqual, "B");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MyInnerString = "A" } }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MyInnerString = "B" } }));
        }

        [TestMethod]
        public void IsEqual_With_Inner_Objects()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInner.MySubSubItem.MySubSubString", PredicateOperator.IsEqual, "B");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MySubSubItem = new SubSubFilteredItem { MySubSubString = "A" } } }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MySubSubItem = new SubSubFilteredItem { MySubSubString = "B" } } }));
        }

        [TestMethod]
        public void Converts_Inner_Object_Value()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInner.MyInnerInt", PredicateOperator.IsEqual, "10");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MyInnerInt = 20 } }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInner = new SubFilteredItem { MyInnerInt = 10 } }));
        }

        [TestMethod]
        public void IsEqual_With_List()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyStrings.Count", PredicateOperator.IsEqual, 2);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyStrings = new List<string> { "A" } }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyStrings = new List<string> { "A", "B" } }));
        }

        [TestMethod]
        public void IsNotEqual_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsNotEqual, "B");

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "A" }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem())); // passes null
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "B" }));
        }

        [TestMethod]
        public void IsNotEqual_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsNotEqual, 10);

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 10 }));
        }

        // numbers
        // PredicateOperator.IsLessThan:
        // PredicateOperator.IsGreaterThan:
        // PredicateOperator.IsGreaterThanOrEqualTo:
        // PredicateOperator.IsLessThanOrEqualTo:

        [TestMethod]
        public void IsLess_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsLessThan, 20);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 10 }));
        }

        [TestMethod]
        public void IsLessOrEqual_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsLessThanOrEqualTo, 20);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 30 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 10 }));
        }

        [TestMethod]
        public void IsGreater_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsGreaterThan, 10);

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 10 }));
        }

        [TestMethod]
        public void IsGreaterOrEqual_With_Int()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyInt", PredicateOperator.IsGreaterThanOrEqualTo, 10);

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 20 }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyInt = 10 }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyInt = 5 }));
        }

        // only for string
        // PredicateOperator.StartsWith
        // PredicateOperator.EndsWith
        // PredicateOperator.Contains
        // PredicateOperator.DoesNotContain

        [TestMethod]
        public void Contains_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.Contains, "b");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null TODO: handle nulls 
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void Contains_Case_Sensitive_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.Contains, "b", isCaseSensitive: true);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void DoesNotContains_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.DoesNotContain, "b");

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void DoesNotContains_Case_Sensitive_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.DoesNotContain, "b", isCaseSensitive: true);

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void StartsWith_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.StartsWith, "item");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null TODO: handle nulls 
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Item B" }));
        }

        [TestMethod]
        public void StartsWith_With_String_With_Case_Sensitive()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.StartsWith, "item", isCaseSensitive: true);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null TODO: handle nulls 
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Item B" }));
        }

        [TestMethod]
        public void EndsWith_With_String()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.EndsWith, "b");

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null TODO: handle nulls 
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Item B" }));
        }

        [TestMethod]
        public void EndsWith_With_String_With_Case_Sensitive()
        {
            var filter = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.EndsWith, "b", isCaseSensitive: true);

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            // Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null TODO: handle nulls 
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Item B" }));
        }

        ///// <summary>
        ///// Gets the property for the property path.
        ///// </summary>
        ///// <param name="type">The type</param>
        ///// <param name="propertyPath">The property path</param>
        ///// <returns>The property</returns>
        //public PropertyInfo GetProperty(Type type, string propertyPath)
        //{
        //    var propertyNames = propertyPath.Split('.');
        //    if (propertyNames.Length == 1)
        //    {
        //        var property = type.GetProperty(propertyPath);
        //        if (property == null)
        //            throw new ArgumentException($"Failed to find the property for the property path '{propertyPath}' and type '{type.Name}'");
        //        return property;
        //    }
        //    else
        //    {
        //        Type currentType = type;
        //        PropertyInfo property = null;
        //        foreach (string propertyName in propertyNames)
        //        {
        //            property = currentType.GetProperty(propertyName);
        //            if (property == null)
        //                throw new ArgumentException($"Failed to find the property for the property path '{propertyPath}' and type '{type.Name}'");
        //            currentType = property.PropertyType;
        //        }
        //        return property;
        //    }
        //}

        //public static object GetValueFromPropertyPath(object value, string path)
        //{
        //    Type currentType = value.GetType();

        //    foreach (string propertyName in path.Split('.'))
        //    {
        //        PropertyInfo property = currentType.GetProperty(propertyName);
        //        value = property.GetValue(value, null);
        //        currentType = property.PropertyType;
        //    }
        //    return value;
        //}
    }

    public class MyFilteredItem
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
        public double MyDouble { get; set; }
        public DateTime MyDate { get; set; }
        public SubFilteredItem MyInner { get; set; }
        public List<string> MyStrings { get; set; }
    }

    public class SubFilteredItem
    {
        public string MyInnerString { get; set; }
        public int MyInnerInt { get; set; }
        public SubSubFilteredItem MySubSubItem { get; set; }
    }

    public class SubSubFilteredItem
    {
        public string MySubSubString { get; set; }
    }

    [TestClass]
    public class CompositeFilterTests
    {
        [TestMethod]
        public void With_Logical_Or()
        {
            var filter = new CompositeFilter<MyFilteredItem>(LogicalOperator.Or);
            var filterA = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsEqual, "Value A");
            var filterB = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsEqual, "Value B");
            filter.AddFilter(filterA);
            filter.AddFilter(filterB);
            filter.Refresh();

            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Value B" }));
        }

        [TestMethod]
        public void Not_Null_And_StartsWith()
        {
            var filter = new CompositeFilter<MyFilteredItem>();
            var filterA = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.IsNotEqual, null);
            var filterB = new PropertyFilter<MyFilteredItem>("MyString", PredicateOperator.StartsWith, "item");
            filter.AddFilter(filterA);
            filter.AddFilter(filterB);
            filter.Refresh();

            Assert.AreEqual(false, filter.Filter(new MyFilteredItem { MyString = "Value A" }));
            Assert.AreEqual(false, filter.Filter(new MyFilteredItem())); // passes null
            Assert.AreEqual(true, filter.Filter(new MyFilteredItem { MyString = "Item B" }));
        }
    }
}
