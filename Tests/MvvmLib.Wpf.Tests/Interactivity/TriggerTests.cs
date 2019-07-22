using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Wpf.Tests.Interactivity
{
    [TestClass]
    public class EventTriggerTests
    {
        [TestMethod]
        public void Test_Call()
        {
            var c = new TestEventTrigger();
            var button = new Button();
            c.EventName = "Click";
            c.Attach(button);

            Assert.AreEqual(false, c.IsInvoked);

            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            Assert.AreEqual(true, c.IsInvoked);
        }
    }

    [TestClass]
    public class DataTriggerTests
    {
        [TestMethod]
        public void Test_Call()
        {
            var c = new TestDataTrigger();
            var item = new MyDataTriggerItem { MyString = "OK" };
            c.Binding = item.MyString;
            c.Value = "OK";

            c.Attach(item);

            Assert.AreEqual(true, c.IsInvoked);
            Assert.AreEqual(true, c.Result);
        }

        [TestMethod]
        public void Test_Call_False()
        {
            var c = new TestDataTrigger();
            var item = new MyDataTriggerItem { MyString = "NotOK" };
            c.Binding = item.MyString;
            c.Value = "OK";

            c.Attach(item);

            Assert.AreEqual(true, c.IsInvoked);
            Assert.AreEqual(false, c.Result);
        }
    }

    public class MyDataTriggerItem : DependencyObject
    {

        public string MyString
        {
            get { return (string)GetValue(MyStringProperty); }
            set { SetValue(MyStringProperty, value); }
        }

        public static readonly DependencyProperty MyStringProperty =
            DependencyProperty.Register("MyString", typeof(string), typeof(MyDataTriggerItem), new PropertyMetadata(null));

    }

    public class TestEventTrigger : MvvmLib.Interactivity.EventTrigger
    {
        public bool IsInvoked { get; set; }

        protected override void OnEvent(object sender, object eventArgs)
        {
            this.IsInvoked = true;
            base.OnEvent(sender, eventArgs);
        }
    }

    public class TestDataTrigger : MvvmLib.Interactivity.DataTrigger
    {
        public bool IsInvoked { get; set; }
        public bool Result { get; set; }

        protected override void CompareAndInvokeActions()
        {
            this.IsInvoked = true;
            this.Result = this.Compare();


            base.CompareAndInvokeActions();
        }
    }
}
