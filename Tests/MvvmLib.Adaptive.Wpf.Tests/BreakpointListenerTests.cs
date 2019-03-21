using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Adaptive;

namespace MvvmLib.Adaptive.Wpf.Tests
{

    [TestClass]
    public class BreakpointListenerTests
    {

        [TestMethod]
        public void AddBreakpoint_SortOnAdd()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            service.AddBreakpoint(200);
            service.AddBreakpoint(0);
            service.AddBreakpoint(100);

            Assert.AreEqual(3, service.Breakpoints.Count);
            Assert.AreEqual(0, service.Breakpoints[0]);
            Assert.AreEqual(100, service.Breakpoints[1]);
            Assert.AreEqual(200, service.Breakpoints[2]);
        }

        [TestMethod]
        public void IsNotified_OnSizeChanged()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            Assert.AreEqual(-1, service.CurrentWidth);

            sizeStrategy.RaiseSizeChanged(100);

            Assert.AreEqual(100, service.CurrentWidth);
        }

        [TestMethod]
        public void GuessActiveWidth()
        {
            var breakpoints = new List<double> { 0, 100, 200, 300 };

            Assert.AreEqual(-1, AdaptiveHelper.GuessActiveWidth(10, new List<double>()));
            Assert.AreEqual(0, AdaptiveHelper.GuessActiveWidth(10, breakpoints));
            Assert.AreEqual(100, AdaptiveHelper.GuessActiveWidth(150, breakpoints));
            Assert.AreEqual(200, AdaptiveHelper.GuessActiveWidth(250, breakpoints));
            Assert.AreEqual(300, AdaptiveHelper.GuessActiveWidth(350, breakpoints));
            Assert.AreEqual(300, AdaptiveHelper.GuessActiveWidth(1500, breakpoints));
        }


        [TestMethod]
        public void ActiveChange()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            var breakpoints = new List<double> { 0, 100, 200, 300 };

            Assert.AreEqual(-1, service.CurrentBreakpoint);

            sizeStrategy.RaiseSizeChanged(10);

            Assert.AreEqual(-1, service.CurrentBreakpoint);
            Assert.AreEqual(10, service.CurrentWidth);

            service.AddBreakpoint(0);
            service.AddBreakpoint(100);
            service.AddBreakpoint(200);
            service.AddBreakpoint(300);

            sizeStrategy.RaiseSizeChanged(10);

            Assert.AreEqual(0, service.CurrentBreakpoint);
            Assert.AreEqual(10, service.CurrentWidth);

            sizeStrategy.RaiseSizeChanged(150);

            Assert.AreEqual(100, service.CurrentBreakpoint);
            Assert.AreEqual(150, service.CurrentWidth);

            sizeStrategy.RaiseSizeChanged(250);

            Assert.AreEqual(200, service.CurrentBreakpoint);
            Assert.AreEqual(250, service.CurrentWidth);

            sizeStrategy.RaiseSizeChanged(350);

            Assert.AreEqual(300, service.CurrentBreakpoint);
            Assert.AreEqual(350, service.CurrentWidth);
        }

        [TestMethod]
        public void RaiseBreakpointChanged()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            int count = 0;

            service.BreakpointChanged += (s, e) =>
            {
                count++;
            };

            service.AddBreakpoint(0);
            service.AddBreakpoint(100);
            service.AddBreakpoint(200);
            service.AddBreakpoint(300);

            sizeStrategy.RaiseSizeChanged(10);
            sizeStrategy.RaiseSizeChanged(20); // ignored
            sizeStrategy.RaiseSizeChanged(150);
            sizeStrategy.RaiseSizeChanged(160); // ignored
            sizeStrategy.RaiseSizeChanged(170); // ignored
            sizeStrategy.RaiseSizeChanged(180); // ignored
            sizeStrategy.RaiseSizeChanged(250);
            sizeStrategy.RaiseSizeChanged(260); // ignored
            sizeStrategy.RaiseSizeChanged(350);
            sizeStrategy.RaiseSizeChanged(1000); // ignored
            sizeStrategy.RaiseSizeChanged(1600); // ignored

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public async Task RaiseBreakpointChanged_DeferredOnSubscribe()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            int count = 0;

            service.AddBreakpoint(0);

            sizeStrategy.RaiseSizeChanged(10);

            await Task.Delay(3000);

            service.BreakpointChanged += (s, e) =>
            {
                count++;
            };


            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task RaiseBreakpointChanged_DeferredOnAdd()
        {
            var sizeStrategy = new MySizeStrategy();
            var service = new BreakpointListener(sizeStrategy);

            int count = 0;

            service.BreakpointChanged += (s, e) =>
            {
                count++;
            };

            sizeStrategy.RaiseSizeChanged(10);

            await Task.Delay(3000);

            service.AddBreakpoint(0);

            Assert.AreEqual(1, count);
        }

    }
}
