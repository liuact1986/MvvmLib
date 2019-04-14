using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests
{
    [TestClass]
    public class ItemsRegionTests
    {
        [TestMethod]
        public async Task Add_Complete()
        {
            NavigatableView.Reset();

            var i = new ItemsControl();
            i.Name = "i1";
            var service = new ItemsRegion("X1", i);

            Assert.AreEqual("X1", service.RegionName);
            Assert.AreEqual(i, service.Control);
            Assert.AreEqual("i1", service.ControlName);

            await service.AddAsync(typeof(NavigatableView), "p1");
            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);

            await service.AddAsync(typeof(SimpleView));
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Add_Activatable()
        {
            ActivatableView.Reset();

            var i = new ItemsControl();
            i.Name = "i1";
            var service = new ItemsRegion("X1", i);

            await service.AddAsync(typeof(ActivatableView), "p1");
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("p1", ActivatableView.p);
            Assert.AreEqual("p1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);

            await service.AddAsync(typeof(SimpleView));
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);
        }

        [TestMethod]
        public async Task Remove()
        {
            NavigatableView.Reset();

            var i = new ItemsControl();
            i.Name = "i1";
            var service = new ItemsRegion("X1", i);

            Assert.AreEqual("X1", service.RegionName);
            Assert.AreEqual(i, service.Control);
            Assert.AreEqual("i1", service.ControlName);

            await service.AddAsync(typeof(NavigatableView), "p1");

            NavigatableView.Reset();

            await service.RemoveAtAsync(0);

            Assert.AreEqual(false, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, NavigatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Remove_Activatable()
        {
            ActivatableView.Reset();

            var i = new ItemsControl();
            i.Name = "i1";
            var service = new ItemsRegion("X1", i);

            await service.AddAsync(typeof(ActivatableView), "p1");

            ActivatableView.Reset();

            await service.RemoveAtAsync(0);

            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(false, ActivatableView.isOkCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatingFrom);
            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
        }
    }
}
