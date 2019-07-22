using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;
using MvvmLib.Interactivity;
using MvvmLib.Wpf.Tests.Mvvm;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace MvvmLib.Wpf.Tests.Interactivity
{
    [TestClass]
    public class TriggerActionTests
    {
        [TestMethod]
        public void CallMethodAction()
        {
            var c = new TestCallMethodAction();
            var item = new ActionItem();

            c.TargetObject = item;
            c.MethodName = "Method1";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
        }

        [TestMethod]
        public void CallMethodAction_With_String_Parameter()
        {
            var c = new TestCallMethodAction();
            var item = new ActionItem();

            c.TargetObject = item;
            c.MethodName = "Method2";
            c.Parameter = "My parameter";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod2Invoked);
            Assert.AreEqual("My parameter", item.Method2Parameter);
        }


        [TestMethod]
        public void Change_CallMethodAction_Properties()
        {
            var c = new TestCallMethodAction();
            var item = new ActionItem();

            c.TargetObject = item;
            c.MethodName = "Method1";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);

            c.MethodName = "Method2";
            c.Parameter = "My parameter";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod2Invoked);
            Assert.AreEqual("My parameter", item.Method2Parameter);
        }

        [TestMethod]
        public void CallMethodAction_With_Int_Parameter()
        {
            var c = new TestCallMethodAction();
            var item = new GenericActionItem<int>();

            c.TargetObject = item;
            c.MethodName = "Method1";
            c.Parameter = 10;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(10, item.Method1Parameter);
        }

        [TestMethod]
        public void CallMethodAction_With_Int_Nullable_Parameter()
        {
            var c = new TestCallMethodAction();
            var item = new GenericActionItem<int>();

            int? n = 10;
            c.TargetObject = item;
            c.MethodName = "Method1";
            c.Parameter = n;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(n, item.Method1Parameter);
        }

        [TestMethod]
        public void CallMethodAction_With_DateTime_Parameter()
        {
            var c = new TestCallMethodAction();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);

            c.TargetObject = item;
            c.MethodName = "Method1";
            c.Parameter = date;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction()
        {
            var c = new TestInvokeCommandAction();
            var item = new ActionItem();

            c.Command = item.MyCommand1;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
        }

        [TestMethod]
        public void InvokeCommandAction_With_Parameter()
        {
            var c = new TestInvokeCommandAction();
            var item = new ActionItem();

            c.Command = item.MyCommand2;
            c.CommandParameter = "My parameter";
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod2Invoked);
            Assert.AreEqual("My parameter", item.Method2Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction_With_Int_Parameter()
        {
            var c = new TestInvokeCommandAction();
            var item = new GenericActionItem<int>();

            c.Command = item.MyCommand1;
            c.CommandParameter = 10;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(10, item.Method1Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction_With_Int_Nullable_Parameter()
        {
            var c = new TestInvokeCommandAction();
            var item = new GenericActionItem<int>();

            int? n = 10;
            c.Command = item.MyCommand1;
            c.CommandParameter = n;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(n, item.Method1Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction_With_DateTime_Parameter()
        {
            var c = new TestInvokeCommandAction();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);

            c.Command = item.MyCommand1;
            c.CommandParameter = date;
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(date, item.Method1Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction_With_Converter()
        {
            var c = new TestInvokeCommandAction();
            var item = new GenericActionItem<bool>();

            c.Command = item.MyCommand1;
            c.CommandParameter = "true";
            c.Converter = new StringToBoolConverter();
            c.TestInvoke();

            Assert.AreEqual(true, item.IsMethod1Invoked);
            Assert.AreEqual(true, item.Method1Parameter);
        }

        [TestMethod]
        public void InvokeCommandAction_With_DateTime_Conversion_And_Invariant_Culture()
        {
            var c = new TestInvokeCommandAction();
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
        public void InvokeCommandAction_With_Converter_And_Culture()
        {
            var c = new TestInvokeCommandAction();
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
        public void InvokeCommandAction_With_Converter_Pass_Command_Parameter()
        {
            var c = new TestInvokeCommandAction();
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
        public void ChangePropertyAction()
        {
            var c = new TestChangePropertyAction();
            var item = new ActionItem();

            c.TargetObject = item;
            c.PropertyPath = "MyString";
            c.Value = "My value";
            c.TestInvoke();

            Assert.AreEqual("My value", item.MyString);
        }

        [TestMethod]
        public void ChangePropertyAction_With_Sub_Item()
        {
            var c = new TestChangePropertyAction();
            var item = new ActionItem { MyInner = new SubFilteredItem { } };

            c.TargetObject = item;
            c.PropertyPath = "MyInner.MyInnerString";
            c.Value = "My value";
            c.TestInvoke();

            Assert.AreEqual("My value", item.MyInner.MyInnerString);
        }

        [TestMethod]
        public void ChangePropertyAction_With_Sub_Sub_Item()
        {
            var c = new TestChangePropertyAction();
            var item = new ActionItem { MyInner = new SubFilteredItem { MySubSubItem = new SubSubFilteredItem { } } };

            c.TargetObject = item;
            c.PropertyPath = "MyInner.MySubSubItem.MySubSubString";
            c.Value = "My value";
            c.TestInvoke();

            Assert.AreEqual("My value", item.MyInner.MySubSubItem.MySubSubString);
        }

        [TestMethod]
        public void ChangePropertyAction_Not_Set_Sub_Item_If_Null()
        {
            var c = new TestChangePropertyAction();
            var item = new ActionItem { };

            c.TargetObject = item;
            c.PropertyPath = "MyInner.MyInnerString";
            c.Value = "My value";
            c.TestInvoke();

            Assert.AreEqual(null, item.MyInner);
        }

        [TestMethod]
        public void ChangePropertyAction_Converts_Int()
        {
            var c = new TestChangePropertyAction();
            var item = new GenericActionItem<int> { };

            c.TargetObject = item;
            c.PropertyPath = "MyProperty";
            c.Value = "10";
            c.TestInvoke();

            Assert.AreEqual(10, item.MyProperty);
        }

        [TestMethod]
        public void ChangePropertyAction_Converts_Value()
        {
            var c = new TestChangePropertyAction();
            var item = new GenericActionItem<Brush> { };

            c.TargetObject = item;
            c.PropertyPath = "MyProperty";
            c.Value = "Red";
            c.TestInvoke();

            Assert.AreEqual(new SolidColorBrush(Colors.Red).ToString(), item.MyProperty.ToString());
        }

        [TestMethod]
        public void ChangePropertyAction_With_Culture()
        {
            var c = new TestChangePropertyAction();
            var item = new GenericActionItem<DateTime>();

            var date = new DateTime(2019, 07, 18);
            var dateAsStringFr = date.ToString(CultureInfo.GetCultureInfo("fr")); // 18/07/2019 00:00:00

            c.TargetObject = item;
            c.PropertyPath = "MyProperty";
            c.Value = dateAsStringFr;
            c.Culture = CultureInfo.GetCultureInfo("fr");
            c.TestInvoke();

            Assert.AreEqual(date, item.MyProperty);
        }

        [TestMethod]
        public void GoToStateAction()
        {
            var c = new TestGoToStateAction();

            c.StateName = "State1";
            var t = new MyControl();
            c.Target = t;
            c.TestInvoke();

            Assert.AreEqual(t, c.Element);
            Assert.AreEqual(true, t is Control);
            Assert.AreEqual("State1", c.ReceiveStateName);
            Assert.AreEqual(false, c.Success);
        }

        [TestMethod]
        public void GoToStateAction_Success()
        {
            var c = new TestGoToStateAction();

            c.StateName = "State1";
            var g = new Grid();
            var v = new VisualStateGroup { Name = "States" };
            var s1 = new VisualState { Name = "State1" };
            //var s1 = (VisualState)XamlReader.Parse("<VisualState x:Name=\"State1\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />");
            v.States.Add(s1);
            VisualStateManager.GetVisualStateGroups(g).Add(v);

            c.Target = g;
            c.TestInvoke();

            Assert.AreEqual(g, c.Element);
            Assert.AreEqual("State1", c.ReceiveStateName);
            Assert.AreEqual(true, c.Success);
        }
    }

    public class MyControl : Control
    {

    }

    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsString = (string)value;
            if (bool.TryParse(valueAsString, out bool result))
            {
                return result;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericConverter<TFrom, TTo> : IValueConverter
    {
        public CultureInfo Culture { get; private set; }
        public object Parameter { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.Culture = culture;
            this.Parameter = parameter;
            if (value == null)
            {
                return value;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TTo));
            if (converter.CanConvertFrom(typeof(TFrom)))
            {
                var convertedValue = culture != null ? converter.ConvertFrom(null, culture, value) : converter.ConvertFrom(value);
                return convertedValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestGoToStateAction : GoToStateAction
    {
        public FrameworkElement Element { get; set; }
        public string ReceiveStateName { get; set; }

        public void TestInvoke()
        {
            this.Invoke();
        }

        protected override bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
        {
            this.Element = element;
            this.ReceiveStateName = stateName;
            return base.GoToState(element, stateName, useTransitions);
        }
    }

    public class TestChangePropertyAction : ChangePropertyAction
    {
        public void TestInvoke()
        {
            this.Invoke();
        }
    }

    public class TestCallMethodAction : CallMethodAction
    {
        public void TestInvoke()
        {
            this.Invoke();
        }
    }

    public class TestInvokeCommandAction : InvokeCommandAction
    {
        public void TestInvoke()
        {
            this.Invoke();
        }
    }

    public class GenericActionItem<T>
    {
        public bool IsMethod1Invoked { get; set; }
        public T Method1Parameter { get; set; }

        public ICommand MyCommand1 { get; set; }

        public T MyProperty { get; set; }

        public GenericActionItem()
        {
            MyCommand1 = new DelegateCommand<T>(Method1);
        }

        public void Method1(T p)
        {
            this.IsMethod1Invoked = true;
            this.Method1Parameter = p;
        }
    }

    public class ActionItem
    {
        public bool IsMethod3Invoked { get; set; }
        public DateTime Method3Parameter { get; set; }
        public bool IsMethod1Invoked { get; set; }
        public bool IsMethod2Invoked { get; set; }
        public string Method2Parameter { get; set; }

        public ICommand MyCommand1 { get; set; }
        public ICommand MyCommand2 { get; set; }

        public string MyString { get; set; }
        public SubFilteredItem MyInner { get; set; }

        public ActionItem()
        {
            MyCommand1 = new DelegateCommand(Method1);
            MyCommand2 = new DelegateCommand<string>(Method2);
        }

        public void Method1()
        {
            this.IsMethod1Invoked = true;
        }

        public void Method2(string p)
        {
            this.IsMethod2Invoked = true;
            Method2Parameter = p;
        }

        public void Method3(DateTime p)
        {
            this.IsMethod3Invoked = true;
            Method3Parameter = p;
        }
    }
}
