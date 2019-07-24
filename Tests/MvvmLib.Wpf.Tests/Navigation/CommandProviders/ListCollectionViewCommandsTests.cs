using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Collections.Generic;

namespace MvvmLib.Wpf.Tests.Navigation.CommandProviders
{
    [TestClass]
    public class ListCollectionViewCommandsTests
    {
        [TestMethod]
        public void Navigate_With_Commands_Update_CanExecute()
        {
            var items = new List<MySharedItem>();
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            Assert.AreEqual(false, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToLastCommand.CanExecute(null));

            var itemA = new MySharedItem();
            source.AddNewItem(itemA);

            Assert.AreEqual(itemA, source.CurrentItem);
            Assert.AreEqual(false, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToLastCommand.CanExecute(null));

            var itemB = new MySharedItem();
            source.AddNewItem(itemB);

            Assert.AreEqual(itemB, source.CurrentItem);
            Assert.AreEqual(true, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToLastCommand.CanExecute(null));

            commands.MoveCurrentToPreviousCommand.Execute(null); // B => A
            Assert.AreEqual(itemA, source.CurrentItem);
            Assert.AreEqual(false, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToLastCommand.CanExecute(null));

            commands.MoveCurrentToNextCommand.Execute(null); // A => B
            Assert.AreEqual(itemB, source.CurrentItem);
            Assert.AreEqual(true, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToLastCommand.CanExecute(null));

            commands.MoveCurrentToFirstCommand.Execute(null);
            Assert.AreEqual(itemA, source.CurrentItem);
            Assert.AreEqual(false, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToNextCommand.CanExecute(null)); // not clear backstack
            Assert.AreEqual(true, commands.MoveCurrentToLastCommand.CanExecute(null));

            commands.MoveCurrentToLastCommand.Execute(null);
            Assert.AreEqual(itemB, source.CurrentItem);
            Assert.AreEqual(true, commands.MoveCurrentToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveCurrentToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveCurrentToLastCommand.CanExecute(null));
        }

        [TestMethod]
        public void AddNew()
        {
            var items = new List<MySharedItem>();
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            Assert.AreEqual(0, items.Count);

            commands.AddNewCommand.Execute(null);

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(typeof(MySharedItem), items[0].GetType());
        }


        [TestMethod]
        public void Cancel_AddNew()
        {
            var items = new List<MySharedItem>();
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            Assert.AreEqual(0, items.Count);

            commands.AddNewCommand.Execute(null);

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(typeof(MySharedItem), items[0].GetType());

            // cancel
            commands.CancelCommand.Execute(null);
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void Edit_And_CancelEdit()
        {
            var item = new MyItemEditable { MyString = "My value" };
            var items = new List<MyItemEditable> { item };
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            Assert.AreEqual(item, source.CurrentItem);
            Assert.AreEqual(null, source.CurrentEditItem);
            Assert.AreEqual("My value", ((MyItemEditable)source.CurrentItem).MyString);

            commands.EditCommand.Execute(null);

            item.MyString = "My new string value";
            Assert.AreEqual("My new string value", ((MyItemEditable)source.CurrentItem).MyString);
            Assert.AreEqual("My new string value", ((MyItemEditable)source.CurrentEditItem).MyString);

            commands.CancelCommand.Execute(null);

            Assert.AreEqual("My value", ((MyItemEditable)source.CurrentItem).MyString);
            Assert.AreEqual(null, source.CurrentEditItem);
        }

        [TestMethod]
        public void SortBy()
        {
            var items = new List<MyFilterableItem> {
                new MyFilterableItem { MyString = "A", Age = 40 },
                new MyFilterableItem { MyString = "B", Age = 20 },
                new MyFilterableItem { MyString = "C", Age = 30 },
            };
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            commands.SortByCommand.Execute("Age");

            Assert.AreEqual("B", ((MyFilterableItem)source[0]).MyString);
            Assert.AreEqual("C", ((MyFilterableItem)source[1]).MyString);
            Assert.AreEqual("A", ((MyFilterableItem)source[2]).MyString);

            commands.SortByDescendingCommand.Execute("Age");

            Assert.AreEqual("A", ((MyFilterableItem)source[0]).MyString);
            Assert.AreEqual("C", ((MyFilterableItem)source[1]).MyString);
            Assert.AreEqual("B", ((MyFilterableItem)source[2]).MyString);
        }

        [TestMethod]
        public void FilterBy()
        {
            var items = new List<MyFilterableItem> {
                new MyFilterableItem { MyString = "A", Age = 40 },
                new MyFilterableItem { MyString = "B", Age = 20 },
                new MyFilterableItem { MyString = "C", Age = 30 },
            };
            var source = new ListCollectionViewEx(items);
            var commands = new ListCollectionViewCommands(source);

            source.FilterBy<MyFilterableItem>(p => p.Age > 20);

            Assert.AreEqual("A", ((MyFilterableItem)source[0]).MyString);
            Assert.AreEqual("C", ((MyFilterableItem)source[1]).MyString);

            commands.ClearFilterCommand.Execute(null);

            Assert.AreEqual("A", ((MyFilterableItem)source[0]).MyString);
            Assert.AreEqual("B", ((MyFilterableItem)source[1]).MyString);
            Assert.AreEqual("C", ((MyFilterableItem)source[2]).MyString);
        }
    }

    public class MyFilterableItem
    {
        public string MyString { get; set; }
        public int Age { get; set; }
    }

}
