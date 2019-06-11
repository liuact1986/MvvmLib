using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class NavigationHelperTest
    {
        [TestMethod]
        public void IsFrameworkElementType()
        {
            Assert.AreEqual(true, NavigationHelper.IsFrameworkElementType(typeof(UserControl)));
            Assert.AreEqual(true, NavigationHelper.IsFrameworkElementType(typeof(Window)));

            Assert.AreEqual(false, NavigationHelper.IsFrameworkElementType(typeof(Vm)));
        }

        [TestMethod]
        public async Task CanActivate()
        {
            var view = new ViewCanActivate();
            var vm = new ViewModelCanActivate();

            view.Reset();
            vm.Reset();
            view.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(view, vm, "p"));

            view.Reset();
            vm.Reset();
            vm.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(view, vm, "p"));

            view.Reset();
            vm.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanActivateAsync(view, vm, "p"));
        }

        [TestMethod]
        public async Task CanActivateVm()
        {
            var vm = new ViewModelCanActivate();

            vm.Reset();
            vm.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(vm, "p"));

            vm.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanActivateAsync(vm, "p"));
        }

        [TestMethod]
        public async Task CanDeactivate()
        {
            var view = new ViewCanDeactivate();
            var vm = view.DataContext as ViewModelCanDeactivate;

            view.Reset();
            vm.CanDeactivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanDeactivateAsync(view));

            view.Reset();
            vm.Reset();
            view.CanDeactivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanDeactivateAsync(view));

            view.Reset();
            vm.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanDeactivateAsync(view));
        }
    }

    public class ViewCanActivate : UserControl, ICanActivate
    {
        public bool CanActivate { get; set; }
        public object P { get; set; }

        public void Reset()
        {
            CanActivate = true;
            P = null;
        }

        public ViewCanActivate()
        {
            this.DataContext = new ViewModelCanActivate();
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            P = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class ViewCanDeactivate : UserControl, ICanDeactivate
    {
        public bool CanDeactivate { get; set; }

        public void Reset()
        {
            CanDeactivate = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            return Task.FromResult(CanDeactivate);
        }

        public ViewCanDeactivate()
        {
            this.DataContext = new ViewModelCanDeactivate();
        }
    }

    public class ViewModelCanActivate : ICanActivate
    {
        public bool CanActivate { get; set; }
        public object P { get; set; }

        public void Reset()
        {
            CanActivate = true;
            P = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            P = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class ViewModelCanDeactivate : ICanDeactivate
    {
        public bool CanDeactivate { get; set; }

        public void Reset()
        {
            CanDeactivate = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            return Task.FromResult(CanDeactivate);
        }
    }
}
