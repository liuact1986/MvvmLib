using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Guard
{
    public class Activatable1 : IActivatable
    {
        public bool CanActivate { get; set; } = false;

        public List<object> P { get; set; } = new List<object>();

        public Task<bool> CanActivateAsync(object parameter)
        {
            P.Add(parameter);
            return Task.FromResult(CanActivate);
        }
    }

    public class Deactivatable1 : IDeactivatable
    {
        public bool CanDeactivate { get; set; } = false;

        public Task<bool> CanDeactivateAsync()
        {
            return Task.FromResult(CanDeactivate);
        }
    }

    [TestClass]
    public class NavigationGuardTests
    {
        public NavigationGuard GetService()
        {
            return new NavigationGuard();
        }

        [TestMethod]
        public async Task CanActivate()
        {
            var service = GetService();

            var a = new Activatable1();

            var r1 = await service.CheckCanActivateAsync(a, "p1");
            Assert.IsFalse(r1);

            a.CanActivate = true;

            var r2 = await service.CheckCanActivateAsync(a, "p2");
            Assert.IsTrue(r2);

            Assert.AreEqual(2, a.P.Count);
            Assert.AreEqual("p1", a.P[0]);
            Assert.AreEqual("p2", a.P[1]);
        }

        [TestMethod]
        public async Task CanDeactivate()
        {
            var service = GetService();

            var a = new Deactivatable1();

            var r1 = await service.CheckCanDeactivateAsync(a);
            Assert.IsFalse(r1);

            a.CanDeactivate = true;

            var r2 = await service.CheckCanDeactivateAsync(a);
            Assert.IsTrue(r2);
        }

        [TestMethod]
        public async Task CanActivate_Notify()
        {
            var service = GetService();

            IActivatable r = null;
            object par = null;

            service.SetCancellationCallback((ac, p) =>
            {
                r = ac;
                par = p;
            }, null);

            var a = new Activatable1();

            var r1 = await service.CheckCanActivateAsync(a, "p1");
            Assert.IsFalse(r1);
            Assert.AreEqual(a, r);
            Assert.AreEqual("p1", par);
        }

        [TestMethod]
        public async Task CanDeactivate_Notify()
        {
            var service = GetService();

            IDeactivatable r = null;
            object par = null;

            service.SetCancellationCallback(null, (ac) =>
            {
                r = ac;
            });

            var a = new Deactivatable1();

            var r1 = await service.CheckCanDeactivateAsync(a);
            Assert.IsFalse(r1);
            Assert.AreEqual(a, r);
        }
    }
}
