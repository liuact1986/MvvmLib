using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{

    [TestClass]
    public class ClonerTests
    {
        public Cloner GetService()
        {
            return new Cloner();
        }

        [TestMethod]
        public void DeepClone_Public()
        {
            var cloner = GetService();

            var item = new FullItem();
            var result = cloner.DeepClone<FullItem>(item);

            Assert.AreEqual(100, result.MyValueType);
            Assert.AreEqual("My value", result.MyString);
            Assert.AreEqual("ViewA", result.MyUri.OriginalString);
            Assert.AreEqual(10, result.MyNullable);
            Assert.AreEqual(null, result.MyNullableNull);
            Assert.AreEqual("Sub item value", result.MySubItem.MySubString);
        }

        [TestMethod]
        public void DeepClone_Private()
        {
            var cloner = GetService();

            var item = FullItemPrivate.Create();
            var result = cloner.DeepClone<FullItemPrivate>(item);

            Assert.AreEqual(100, result.MyValueType);
            Assert.AreEqual("My value", result.MyString);
            Assert.AreEqual("ViewA", result.MyUri.OriginalString);
            Assert.AreEqual(10, result.MyNullable);
            Assert.AreEqual(null, result.MyNullableNull);
            Assert.AreEqual("Sub item value", result.MySubItem.MySubString);
        }

        [TestMethod]
        public void DeepClone_Private_And_NonPublicProperties()
        {
            var cloner = GetService();

            cloner.NonPublicProperties = false;

            var item = FullItemPrivate.Create();
            var result = cloner.DeepClone<FullItemPrivate>(item);

            Assert.IsTrue(result.isOk);
            Assert.AreEqual(100, result.MyValueType);
            Assert.AreEqual("My value", result.MyString);
            Assert.AreEqual("ViewA", result.MyUri.OriginalString);
            Assert.AreEqual(10, result.MyNullable);
            Assert.AreEqual(null, result.MyNullableNull);
            Assert.AreEqual("Sub item value", result.MySubItem.MySubString);
        }

        [TestMethod]
        public void DeepClone_WithProperty_CannotWrite()
        {
            var cloner = GetService();

            var item = new ItemWithPrivatePropertyCannotWrite("p1");
            var result = cloner.DeepClone<ItemWithPrivatePropertyCannotWrite>(item);

            Assert.AreEqual(null, result.MyString);
        }

        [TestMethod]
        public void DeepClone_WithProperty_CannotRead()
        {
            var cloner = GetService();

            var item = new ItemWithPrivatePropertyCannotRead("p1");
            var result = cloner.DeepClone<ItemWithPrivatePropertyCannotRead>(item);

            Assert.AreEqual(null, result.myString);
        }

        [TestMethod]
        public void DeepClone()
        {
            var cloner = GetService();

            var item = new SubItemToClone { MySubString = "Initial value" };
            var result = cloner.DeepClone<SubItemToClone>(item);

            Assert.AreEqual("Initial value", result.MySubString);

            item.MySubString = "new value";

            Assert.AreEqual("Initial value", result.MySubString);

            result.MySubString = "clone new value";

            Assert.AreEqual("new value", item.MySubString);
        }

        [TestMethod]
        public void DeepClone_With_Lists_Array_And_Dictionary()
        {
            var cloner = GetService();

            var item = new ItemToClone();
            var result = cloner.DeepClone<ItemToClone>(item);

            // array
            Assert.AreEqual(2, result.MyValueTypeArray.Length);
            Assert.AreEqual("1", result.MyValueTypeArray[0]);
            Assert.AreEqual("2", result.MyValueTypeArray[1]);

            Assert.AreEqual(2, result.MyArray.Length);
            Assert.AreEqual("Array v1", result.MyArray[0].MySubString);
            Assert.AreEqual("Array v2", result.MyArray[1].MySubString);

            item.MyValueTypeArray[0] = "new array value";
            Assert.AreEqual("1", result.MyValueTypeArray[0]);

            item.MyArray[0].MySubString = "new array v1 value";
            Assert.AreEqual("Array v1", result.MyArray[0].MySubString);

            // list

            Assert.AreEqual(3, result.MyList.Count);
            Assert.AreEqual("a", result.MyList[0]);
            Assert.AreEqual("b", result.MyList[1]);
            Assert.AreEqual("c", result.MyList[2]);

            Assert.AreEqual(2, result.MySubItems.Count);
            Assert.AreEqual("Sub a", result.MySubItems[0].MySubString);
            Assert.AreEqual("Sub b", result.MySubItems[1].MySubString);

            Assert.AreEqual(2, result.MyValueTypeArray.Length);
            Assert.AreEqual("1", result.MyValueTypeArray[0]);
            Assert.AreEqual("2", result.MyValueTypeArray[1]);

            Assert.AreEqual(2, result.MyDict.Count);
            Assert.AreEqual("v1", result.MyDict["k1"]);
            Assert.AreEqual("v2", result.MyDict["k2"]);

            item.MySubItems[0].MySubString = "new value";

            Assert.AreEqual("Sub a", result.MySubItems[0].MySubString);

            result.MySubItems[0].MySubString = "clone new value";

            Assert.AreEqual("new value", item.MySubItems[0].MySubString);

            item.MyList[0] = "new string";

            Assert.AreEqual("a", result.MyList[0]);


            // dictionary
            Assert.AreEqual(2, result.MyDict.Count);
            Assert.AreEqual("v1", result.MyDict["k1"]);
            Assert.AreEqual("v2", result.MyDict["k2"]);

            Assert.AreEqual(2, result.MyItemDict.Count);
            Assert.AreEqual("Dict v1", result.MyItemDict["k1"].MySubString);
            Assert.AreEqual("Dict v2", result.MyItemDict["k2"].MySubString);

            item.MyDict["k1"] = "new dict value";
            Assert.AreEqual("v1", result.MyDict["k1"]);

            item.MyItemDict["k1"].MySubString = "new dict v1";
            Assert.AreEqual("Dict v1", result.MyItemDict["k1"].MySubString);

        }

        [TestMethod]
        public void IncludeFields()
        {
            var cloner = GetService();

            var item = new ItemWithFieldAndProperties("f1", "p1");
            var r1 = cloner.DeepClone<ItemWithFieldAndProperties>(item);

            cloner.IncludeFields = true;

            var r2 = cloner.DeepClone<ItemWithFieldAndProperties>(item);

            Assert.AreEqual(null, r1.myField);
            Assert.AreEqual("p1", r1.MyString);
            Assert.AreEqual("f1", r2.myField);
            Assert.AreEqual("p1", r2.MyString);
        }

        [TestMethod]
        public void IncludePrivateFields()
        {
            var cloner = GetService();

            cloner.NonPublicFields = false;

            var item = new ItemWithPrivateFieldAndProperties("f1", "p1");
            var r1 = cloner.DeepClone<ItemWithPrivateFieldAndProperties>(item);

            cloner.IncludeFields = true;

            var r2 = cloner.DeepClone<ItemWithPrivateFieldAndProperties>(item);

            cloner.NonPublicFields = true;

            var r3 = cloner.DeepClone<ItemWithPrivateFieldAndProperties>(item);

            Assert.AreEqual(null, r1.myField);
            Assert.AreEqual("p1", r1.MyString);

            Assert.AreEqual(null, r2.myField);
            Assert.AreEqual("p1", r2.MyString);

            Assert.AreEqual("f1", r3.myField);
            Assert.AreEqual("p1", r3.MyString);
        }

        [TestMethod]
        public void CloneWithInterfaces()
        {
            var cloner = GetService();

            var item0 = new ItemWithInterfaces
            { };

            var item = new ItemWithInterfaces
            {
                MyItems = new List<IMyItem>(),
                MyArray = new MyItem[0] { },
                MyDict = new Dictionary<string, IMyItem>()
            };


            var item2 = new ItemWithInterfaces
            {
                MyItems = new List<IMyItem>
                    {
                        new MyItem { MyString ="v1"},
                        new MyItem { MyString ="v2"}
                    },
                MyArray = new MyItem[1]
                {
                        new MyItem { MyString ="Array v1"}
                },
                MyDict = new Dictionary<string, IMyItem>
                {
                    {"k1", new MyItem{MyString = "Dict v1"} }
                }
            };

            var r0 = cloner.DeepClone<ItemWithInterfaces>(item0);
            var r1 = cloner.DeepClone<ItemWithInterfaces>(item);
            var r2 = cloner.DeepClone<ItemWithInterfaces>(item2);

            Assert.AreEqual("My value", r0.MyItem.MyString);
            Assert.AreEqual(null, r0.MyItems);
            Assert.AreEqual(null, r0.MyArray);
            Assert.AreEqual(null, r0.MyDict);

            Assert.AreEqual("My value", r1.MyItem.MyString);
            Assert.AreEqual(0, r1.MyItems.Count);
            Assert.AreEqual(0, r1.MyArray.Length);
            Assert.AreEqual(0, r1.MyDict.Count);

            Assert.AreEqual("My value", r2.MyItem.MyString);
            Assert.AreEqual(2, r2.MyItems.Count);
            Assert.AreEqual(typeof(MyItem), r2.MyItems.ElementAt(0).GetType());
            Assert.AreEqual("v1", r2.MyItems.ElementAt(0).MyString);
            Assert.AreEqual("v2", r2.MyItems.ElementAt(1).MyString);
            Assert.AreEqual(typeof(MyItem), r2.MyItems.ElementAt(1).GetType());
            Assert.AreEqual(1, r2.MyArray.Length);
            Assert.AreEqual(typeof(MyItem), r2.MyArray[0].GetType());
            Assert.AreEqual("Array v1", r2.MyArray[0].MyString);
            Assert.AreEqual(1, r2.MyDict.Count);
            Assert.AreEqual(typeof(MyItem), r2.MyDict["k1"].GetType());
            Assert.AreEqual("Dict v1", r2.MyDict["k1"].MyString);
        }

        [TestMethod]
        public void Return_CircularReference()
        {

            var cloner = GetService();

            var i1 = new CircularItem("My value");

            var r1 = cloner.DeepClone<CircularItem>(i1);

            Assert.AreEqual("My value", r1.MyString);
            Assert.AreEqual("My value", r1.Item.MyString);
            Assert.AreEqual("My value", r1.Item.Item.MyString);
        }
    }

    public class CircularItem
    {
        public string MyString { get; set; }

        public CircularItem Item { get; set; }

        public CircularItem(string myString)
        {
            MyString = myString;

            this.Item = this;
        }
    }


    public interface IMyItem
    {
        string MyString { get; set; }
    }

    public class MyItem : IMyItem
    {
        public string MyString { get; set; }
    }

    public class ItemWithInterfaces
    {
        public IMyItem MyItem { get; set; }

        public ICollection<IMyItem> MyItems { get; set; }

        public IMyItem[] MyArray { get; set; }

        public IDictionary<string, IMyItem> MyDict { get; set; }

        public ItemWithInterfaces()
        {
            MyItem = new MyItem { MyString = "My value" };
            //MyItems = new List<IMyItem>
            //{
            //    new MyItem { MyString ="v1"},
            //    new MyItem { MyString ="v2"}
            //};
        }
    }

    public class ItemWithPrivatePropertyCannotWrite
    {
        internal string MyString { get; }

        public ItemWithPrivatePropertyCannotWrite(string p)
        {
            MyString = p;
        }
    }

    public class ItemWithPrivatePropertyCannotRead
    {
        internal string myString;
        public string MyString
        {
            set { myString = value; }
        }

        public ItemWithPrivatePropertyCannotRead(string p)
        {
            myString = p;
        }
    }

    public class ItemWithFieldAndProperties
    {
        public string myField;

        public string MyString { get; set; }

        public ItemWithFieldAndProperties(string p1, string p2)
        {
            myField = p1;
            MyString = p2;
        }
    }

    public class ItemWithPrivateFieldAndProperties
    {
        internal string myField;

        internal string MyString { get; set; }

        public ItemWithPrivateFieldAndProperties(string p1, string p2)
        {
            myField = p1;
            MyString = p2;
        }
    }

    public class SubItemToClone
    {
        public string MySubString { get; set; }
    }

    public class ItemToClone
    {
        // array of value types
        public string[] MyValueTypeArray { get; set; } = new string[] { "1", "2" };

        public SubItemToClone[] MyArray { get; set; } = new SubItemToClone[]
        {
            new SubItemToClone { MySubString = "Array v1" },
            new SubItemToClone { MySubString = "Array v2" }
        };

        // list

        public List<string> MyList { get; set; } = new List<string> { "a", "b", "c" };

        public List<SubItemToClone> MySubItems { get; set; } = new List<SubItemToClone>
        {
            new SubItemToClone{ MySubString = "Sub a"},
            new SubItemToClone{ MySubString = "Sub b"}
        };

        // dictionary

        public Dictionary<string, string> MyDict { get; set; } = new Dictionary<string, string>
        {
            { "k1","v1" },
            { "k2","v2" }
        };

        public Dictionary<string, SubItemToClone> MyItemDict { get; set; } = new Dictionary<string, SubItemToClone>
        {
            { "k1",new SubItemToClone{ MySubString ="Dict v1" } },
            { "k2",new SubItemToClone{ MySubString ="Dict v2" } }
        };
    }

    public class FullItem
    {
        // value type
        public int MyValueType { get; set; } = 100;

        // string
        public string MyString { get; set; } = "My value";

        // uri 
        public Uri MyUri { get; set; } = new Uri("ViewA", UriKind.Relative);

        // nullable
        public int? MyNullableNull { get; set; }

        public int? MyNullable { get; set; } = 10;

        public SubItemToClone MySubItem { get; set; } = new SubItemToClone { MySubString = "Sub item value" };
    }

    public class FullItemPrivate
    {
        public bool isOk;

        private FullItemPrivate(string myString)
        {
            isOk = true;
        }

        public static FullItemPrivate Create()
        {
            return new FullItemPrivate("");
        }

        // value type
        internal int MyValueType { get; set; } = 100;

        // string
        internal string MyString { get; set; } = "My value";

        // uri 
        internal Uri MyUri { get; set; } = new Uri("ViewA", UriKind.Relative);

        // nullable
        internal int? MyNullableNull { get; set; }

        internal int? MyNullable { get; set; } = 10;

        internal SubItemToClone MySubItem { get; set; } = new SubItemToClone { MySubString = "Sub item value" };
    }

}
