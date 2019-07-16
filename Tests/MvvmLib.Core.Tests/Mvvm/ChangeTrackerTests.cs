using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class ChangeTrackerTests
    {
        // value not supported
        //[TestMethod]
        //public void TrackValue()
        //{
        //    var value = "My value";

        //    var tracker = new Tracker<string>(value);
        //    Assert.IsFalse(tracker.CheckChanges());

        //    value = "New Value";
        //    Assert.IsTrue(tracker.CheckChanges());

        //    value = "My value";
        //    Assert.IsFalse(tracker.CheckChanges());
        //}

        [TestMethod]
        public void TrackObjectPropertyValues()
        {
            var item = new MyTrackedItem
            {
                MyInt = 10,
                MyString = "My string value"
            };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyString = "New Value";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyString = "My string value";
            Assert.IsFalse(tracker.CheckChanges());
        }


        [TestMethod]
        public void Ignore_Property()
        {
            var item = new MyTrackedItem
            {
                MyInt = 10,
                MyString = "My string value"
            };

            var tracker = new ChangeTracker(new List<string> { "MyInt" });
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyInt = 100;
            Assert.IsFalse(tracker.CheckChanges());

            // back
            item.MyString = "My new value";
            Assert.IsTrue(tracker.CheckChanges());
        }

        [TestMethod]
        public void Ignore_Command()
        {
            var item = new MyChangeTrackedItem
            {
                MyString = "My string value"
            };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            item.MyCommand = new RelayCommand(() => { });
            Assert.IsFalse(tracker.CheckChanges());

            item.MyString = "My new value";
            Assert.IsTrue(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerObjects()
        {
            var item = new MyTrackedItemWithInnerObject
            {
                MyInnerItem = new MyTrackedItem
                {
                    MyInt = 10,
                    MyString = "My string value"
                }
            };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyInnerItem.MyString = "New Value";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyInnerItem.MyString = "My string value";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerObjects_Null()
        {
            var item = new MyTrackedItemWithInnerObject();

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyInnerItem = new MyTrackedItem
            {
                MyInt = 10,
                MyString = "My string value"
            };
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyInnerItem = null;
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerObjects_That_Change_To_Null()
        {
            var item = new MyTrackedItemWithInnerObject
            {
                MyInnerItem = new MyTrackedItem
                {
                    MyInt = 10,
                    MyString = "My string value"
                }
            };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyInnerItem = null;
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyInnerItem = new MyTrackedItem
            {
                MyInt = 10,
                MyString = "My string value"
            };
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerList_Null()
        {
            var item = new MyTrackedItemWithList();

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems = new List<string> { "A", "B" };
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems = null;
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerList_That_Change_To_Null()
        {
            var item = new MyTrackedItemWithList { MyItems = new List<string> { "A", "B" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems = null;
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems = new List<string> { "A", "B" };
            Assert.IsFalse(tracker.CheckChanges());
        }

        //[TestMethod]
        //public void Track_List_That_Change_To_Null()
        //{
        //    var items = new List<string> { "A", "B" } ;

        //    var tracker = new ChangeTracker(items, false);
        //    Assert.IsFalse(tracker.CheckChanges());

        //    // change
        //    items = null;

        //    Assert.IsTrue(tracker.CheckChanges());

        //    // back
        //    items = new List<string> { "A", "B" };
        //    Assert.IsFalse(tracker.CheckChanges());
        //}

        [TestMethod]
        public void TrackInnerList_With_Count_Change()
        {
            var item = new MyTrackedItemWithList { MyItems = new List<string> { "A", "B" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            item.MyItems.Add("C");
            Assert.IsTrue(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_List_With_Count_Change()
        {
            var items = new List<string> { "A", "B" };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            items.Add("C");
            Assert.IsTrue(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerList_That_Value_Changed()
        {
            var item = new MyTrackedItemWithList { MyItems = new List<string> { "A", "B" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_List_That_Value_Changed()
        {
            var items = new List<string> { "A", "B" };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Class_That_Inherits_From_Collection()
        {
            var items = new MyCollectionOfString { "A", "B" };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Class_That_Inherits_From_GenericList()
        {
            var items = new MyIListOfString { "A", "B" };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerArray_With_Null()
        {
            var item = new MyTrackedItemWithArray { MyItems = new string[4] };
            item.MyItems[0] = "A";
            item.MyItems[1] = "B";

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            //// "redimensionnement" du tableau
            //string[] newItems = new string[4];
            //Array.Copy(item.MyItems, newItems, 2);
            //item.MyItems = newItems;

            // change
            item.MyItems[2] = "C";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems[2] = null;
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerArray_That_Value_Changed()
        {
            var item = new MyTrackedItemWithArray { MyItems = new string[] { "A", "B" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Array_With_Null()
        {
            var items = new string[4];
            items[0] = "A";
            items[1] = "B";

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items[2] = "C";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items[2] = null;
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Array_That_Value_Changed()
        {
            var items = new string[] { "A", "B" };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items[1] = "B!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items[1] = "B";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerDictionary()
        {
            var item = new MyTrackedItemWithDictionary { MyItems = new Dictionary<string, string> { { "k1", "v1" }, { "k2", "v2" } } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems["k2"] = "v2!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems["k2"] = "v2";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Dictionary()
        {
            var items = new Dictionary<string, string> { { "k1", "v1" }, { "k2", "v2" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items["k2"] = "v2!";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items["k2"] = "v2";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerDictionary_With_Count()
        {
            var item = new MyTrackedItemWithDictionary { MyItems = new Dictionary<string, string> { { "k1", "v1" }, { "k2", "v2" } } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems.Remove("k2");
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems["k2"] = "v2";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Dictionary_With_Count()
        {
            var items = new Dictionary<string, string> { { "k1", "v1" }, { "k2", "v2" } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items.Remove("k2");
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items["k2"] = "v2";
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void TrackInnerDictionary_With_Null()
        {
            var item = new MyTrackedItemWithDictionary { MyItems = new Dictionary<string, string> { { "k1", "v1" }, { "k2", null } } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(item);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            item.MyItems["k2"] = "v2";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            item.MyItems["k2"] = null;
            Assert.IsFalse(tracker.CheckChanges());
        }

        [TestMethod]
        public void Track_Dictionary_With_Null()
        {
            var items = new Dictionary<string, string> { { "k1", "v1" }, { "k2", null } };

            var tracker = new ChangeTracker();
            tracker.TrackChanges(items);
            Assert.IsFalse(tracker.CheckChanges());

            // change
            items["k2"] = "v2";
            Assert.IsTrue(tracker.CheckChanges());

            // back
            items["k2"] = null;
            Assert.IsFalse(tracker.CheckChanges());
        }
    }

    public class MyTrackedItem
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }

    public class MyTrackedItemWithInnerObject
    {
        public MyTrackedItem MyInnerItem { get; set; }
    }

    public class MyTrackedItemWithList
    {
        public List<string> MyItems { get; set; }
    }

    public class MyTrackedItemWithArray
    {
        public string[] MyItems { get; set; }
    }

    public class MyTrackedItemWithDictionary
    {
        public Dictionary<string, string> MyItems { get; set; }
    }


    public class MyChangeTrackedItem
    {
        public string MyString { get; set; }
        public ICommand MyCommand { get; set; }

        public MyChangeTrackedItem()
        {
            MyCommand = new RelayCommand(DoWork);
        }

        private void DoWork()
        {
            
        }
    }
}
