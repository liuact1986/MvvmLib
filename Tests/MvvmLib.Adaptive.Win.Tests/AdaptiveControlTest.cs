﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using MvvmLib.Adaptive;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Threading;

namespace MvvmLib.Adaptive.Win.Tests
{

    public class MySizeStrategy : IAdaptiveSizeChangeStrategy
    {
        public double currentWidth = -1;
        public double CurrentWidth => currentWidth;

        public bool HasWidth => !double.IsNaN(this.CurrentWidth);

        public event EventHandler<AdaptiveSizeChangedEventArgs> SizeChanged;

        public void RaiseSizeChanged(double width)
        {
            this.currentWidth = width;
            this.SizeChanged?.Invoke(this, new AdaptiveSizeChangedEventArgs(width));
        }
    }

    /// <summary>
    /// PROBLEM : UITestMethod with async methods
    /// </summary>
    [TestClass]
    public class AdaptiveControlTest
    {

        //[UITestMethod]
        //public void TestLoadFromFile()
        //{
        //    var sizeStrategy = new MySizeStrategy();
        //    var control = new BreakpointBinder(sizeStrategy);

        //    control.LoadFromJsonFileAsync("Common/bind.json");

        //    Assert.AreEqual(3, control.BindingsByWidth.Count);

        //    var result = control.TryGetActive(100);
        //    Assert.AreEqual("20", result["TitleFontSize"]);
        //    Assert.AreEqual("red", result["TitleColor"]);

        //    result = control.TryGetActive(600);
        //    Assert.AreEqual("50", result["TitleFontSize"]);
        //    Assert.AreEqual("blue", result["TitleColor"]);

        //    result = control.TryGetActive(1200);
        //    Assert.AreEqual("100", result["TitleFontSize"]);
        //    Assert.AreEqual("green", result["TitleColor"]);

        //}

        [UITestMethod]
        public void TestWithAdd()
        {
            var sizeStrategy = new MySizeStrategy();
            var control = new BreakpointBinder(sizeStrategy);
            control.AddBreakpointWithBindings(0, AdaptiveContainer.Create().Set("TitleFontSize", "20").Set("TitleColor", "red").Get())
                .AddBreakpointWithBindings(500, AdaptiveContainer.Create().Set("TitleFontSize", "50").Set("TitleColor", "blue").Get())
                .AddBreakpointWithBindings(1000, AdaptiveContainer.Create().Set("TitleFontSize", "100").Set("TitleColor", "green").Get());

            Assert.AreEqual(3, control.BindingsByWidth.Count);

            var result = control.TryGetActive(100);
            Assert.AreEqual("20", result["TitleFontSize"]);
            Assert.AreEqual("red", result["TitleColor"]);

            result = control.TryGetActive(600);
            Assert.AreEqual("50", result["TitleFontSize"]);
            Assert.AreEqual("blue", result["TitleColor"]);

            result = control.TryGetActive(1200);
            Assert.AreEqual("100", result["TitleFontSize"]);
            Assert.AreEqual("green", result["TitleColor"]);

        }

    //    [UITestMethod]
    //    public void TestFileAndAdd()
    //    {
    //        var sizeStrategy = new MySizeStrategy();
    //        var control = new BreakpointBinder(sizeStrategy);
    //        control
    //.AddBreakpointWithBindings(1500, AdaptiveContainer.Create().Set("TitleFontSize", "120").Set("TitleColor", "pink").Get())
    //.AddBreakpointWithBindings(1800, AdaptiveContainer.Create().Set("TitleFontSize", "150").Set("TitleColor", "yellow").Get());

    //        control.LoadFromJsonFileAsync("Common/bind.json");


    //        Assert.AreEqual(5, control.BindingsByWidth.Count);

    //        var result = control.TryGetActive(100);
    //        Assert.AreEqual("20", result["TitleFontSize"]);
    //        Assert.AreEqual("red", result["TitleColor"]);

    //        result = control.TryGetActive(600);
    //        Assert.AreEqual("50", result["TitleFontSize"]);
    //        Assert.AreEqual("blue", result["TitleColor"]);

    //        result = control.TryGetActive(1200);
    //        Assert.AreEqual("100", result["TitleFontSize"]);
    //        Assert.AreEqual("green", result["TitleColor"]);

    //        result = control.TryGetActive(1600);
    //        Assert.AreEqual("120", result["TitleFontSize"]);
    //        Assert.AreEqual("pink", result["TitleColor"]);

    //        result = control.TryGetActive(1900);
    //        Assert.AreEqual("150", result["TitleFontSize"]);
    //        Assert.AreEqual("yellow", result["TitleColor"]);

    //    }

        //[UITestMethod]
        //public void TestLoad_WhenFileChange()
        //{
        //    var sizeStrategy = new MySizeStrategy();
        //    var control = new BreakpointBinder(sizeStrategy);

        //    Assert.AreEqual(0, control.BindingsByWidth.Count);

        //    control.File = "Common/bind.json";

        //    Task.Delay(1000);

        //    Assert.AreEqual(3, control.BindingsByWidth.Count);
        //}


        [UITestMethod]
        public void TestIsNotified()
        {
            var sizeStrategy = new MySizeStrategy();
            var control = new BreakpointBinder(sizeStrategy);
            control.AddBreakpointWithBindings(0, AdaptiveContainer.Create().Set("TitleFontSize", "20").Set("TitleColor", "red").Get())
            .AddBreakpointWithBindings(500, AdaptiveContainer.Create().Set("TitleFontSize", "50").Set("TitleColor", "blue").Get())
            .AddBreakpointWithBindings(1000, AdaptiveContainer.Create().Set("TitleFontSize", "100").Set("TitleColor", "green").Get());

            Assert.IsNull(control.Active);

            sizeStrategy.RaiseSizeChanged(600);

            Assert.AreEqual("50", control.Active["TitleFontSize"]);
            Assert.AreEqual("blue", control.Active["TitleColor"]);
        }

        [UITestMethod]
        public void RaiseBreakpointBinderChanged_DeferredOnSubscribe()
        {
            var sizeStrategy = new MySizeStrategy();
            var control = new BreakpointBinder(sizeStrategy);

            int count = 0;

            control.AddBreakpointWithBindings(0, AdaptiveContainer.Create().Set("TitleFontSize", "20").Set("TitleColor", "red").Get());

            sizeStrategy.RaiseSizeChanged(10);

            Task.Delay(3000);

            control.ActiveChanged += (s, e) =>
            {
                count++;
            };


            Assert.AreEqual(1, count);
        }
    }
}