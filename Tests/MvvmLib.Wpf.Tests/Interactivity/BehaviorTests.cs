using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Interactivity;
using MvvmLib.Navigation;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Wpf.Tests.Interactivity
{
  
    [TestClass]
    public class BehaviorTests
    {
        [TestMethod]
        public void EventToCommandBehavior()
        {
            var c = new TestEventToCommandBehavior();
            var item = new ActionItem();

            c.Command = item.MyCommand1;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Parameter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new ActionItem();

            c.Command = item.MyCommand2;
            c.CommandParameter = "My parameter";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod2Invoked);
            Assert.AreEqual("My parameter", item.Method2Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Int_Parameter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<int>();

            c.Command = item.MyCommand1;
            c.CommandParameter = 10;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(10, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Int_Nullable_Parameter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<int>();

            int? n = 10;
            c.Command = item.MyCommand1;
            c.CommandParameter = n;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(n, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_DateTime_Parameter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);

            c.Command = item.MyCommand1;
            c.CommandParameter = date;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Converter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<bool>();

            c.Command = item.MyCommand1;
            c.CommandParameter = "true";
            c.Converter = new StringToBoolConverter();
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(true, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_DateTime_Conversion_And_Invariant_Culture()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);
            var dateAsString = date.ToString(CultureInfo.InvariantCulture); // 18/07/2019 00:00:00 => invariant 07/18/2019 00:00:00

            c.Command = item.MyCommand1;
            c.CommandParameter = dateAsString;
            c.Converter = new GenericConverter<string, DateTime>();
            c.ConverterCulture = CultureInfo.InvariantCulture;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Converter_And_Culture()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00

            c.Command = item.MyCommand1;
            c.CommandParameter = dateAsStringFr;
            c.Converter = new GenericConverter<string, DateTime>();
            c.ConverterCulture = CultureInfo.GetCultureInfo("fr");
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
        }

        [TestMethod]
        public void EventToCommandBehavior_With_Converter_Pass_Command_Parameter()
        {
            var c = new TestEventToCommandBehavior();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00

            c.Command = item.MyCommand1;
            c.CommandParameter = dateAsStringFr;
            var converter = new GenericConverter<string, DateTime>();
            c.Converter = converter;
            c.ConverterCulture = CultureInfo.GetCultureInfo("fr");
            c.ConverterParameter = "My parameter";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
            Assert.AreEqual(CultureInfo.GetCultureInfo("fr"), converter.Culture);
            Assert.AreEqual("My parameter", converter.Parameter);
        }

        [TestMethod]
        public void SelectionSyncBehavior()
        {
            var c = new TestSelectionSyncBehavior();

            var listBox = new ListBox { SelectionMode = SelectionMode.Multiple };
            var i1 = new SelectItem { Name = "A" };
            var i2 = new SelectItem { Name = "B" };
            var i3 = new SelectItem { Name = "C" };
            listBox.Items.Add(i1);
            listBox.Items.Add(i2);
            listBox.Items.Add(i3);

            c.Attach(listBox);

            Assert.AreEqual(false, i1.IsSelected);
            Assert.AreEqual(false, i2.IsSelected);
            Assert.AreEqual(false, i3.IsSelected);

            listBox.SelectedItem = i2;

            Assert.AreEqual(false, i1.IsSelected);
            Assert.AreEqual(true, i2.IsSelected);
            Assert.AreEqual(false, i3.IsSelected);

            listBox.SelectedItems.Add(i3);

            Assert.AreEqual(false, i1.IsSelected);
            Assert.AreEqual(true, i2.IsSelected);
            Assert.AreEqual(true, i3.IsSelected);

            listBox.SelectedItems.Clear();

            Assert.AreEqual(false, i1.IsSelected);
            Assert.AreEqual(false, i2.IsSelected);
            Assert.AreEqual(false, i3.IsSelected);
        }

        [TestMethod]
        public void SelectionSyncBehavior_On_Attach_With_Multiple_SelectedItems_ListBox()
        {
            var c = new TestSelectionSyncBehavior();

            var listBox = new ListBox { SelectionMode = SelectionMode.Multiple };
            var i1 = new SelectItem { Name = "A" };
            var i2 = new SelectItem { Name = "B" };
            var i3 = new SelectItem { Name = "C" };
            listBox.Items.Add(i1);
            listBox.Items.Add(i2);
            listBox.Items.Add(i3);

            listBox.SelectedItems.Add(i1);
            listBox.SelectedItems.Add(i2);

            c.Attach(listBox);

            Assert.AreEqual(true, i1.IsSelected);
            Assert.AreEqual(true, i2.IsSelected);
            Assert.AreEqual(false, i3.IsSelected);
        }

        [TestMethod]
        public void SelectionSyncBehavior_On_Attach_With_SelectedItem()
        {
            var c = new TestSelectionSyncBehavior();

            var tabControl = new TabControl { };
            var i1 = new SelectItem { Name = "A" };
            var i2 = new SelectItem { Name = "B" };
            var i3 = new SelectItem { Name = "C" };
            tabControl.Items.Add(i1);
            tabControl.Items.Add(i2);
            tabControl.Items.Add(i3);

            tabControl.SelectedItem = i2;

            c.Attach(tabControl);

            Assert.AreEqual(false, i1.IsSelected);
            Assert.AreEqual(true, i2.IsSelected);
            Assert.AreEqual(false, i3.IsSelected);
        }

        [TestMethod]
        public void SelectionSyncBehavior_Fails_With_Non_Selector()
        {
            var c = new TestSelectionSyncBehavior();

            var itemsControl = new ItemsControl { };
            var i1 = new SelectItem { Name = "A" };
            var i2 = new SelectItem { Name = "B" };
            var i3 = new SelectItem { Name = "C" };
            itemsControl.Items.Add(i1);
            itemsControl.Items.Add(i2);
            itemsControl.Items.Add(i3);

            bool failed = false;
            try
            {
                c.Attach(itemsControl);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.AreEqual(true, failed);
        }
    }

    public class SelectItem : IIsSelected
    {
        public string Name { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }
    }

    public class TestEventToCommandBehavior : EventToCommandBehavior
    {
        public void TestInvoke()
        {
            this.OnEvent(null, null);
        }
    }

    public class TestSelectionSyncBehavior : SelectionSyncBehavior
    {
        public void TestInvoke(SelectionChangedEventArgs e)
        {
            this.OnSelectionChanged(null, e);
        }
    }
}
