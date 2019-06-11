﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Navigation
{

    public class MyControl : ContentControl
    {

    }


    public class SimpleView : UserControl
    {

    }

    public class Vm : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool isOkOnNavigatingTo = false;
        public static bool isOkOnNavigatedTo = false;
        public static object pOnNavigatedTo = null;
        public static object pOnNavigatingTo = null;
        public static object pCanActivate = null;

        public static bool isOkOnNavigatingFrom = false;

        public static bool isOkCanActivate = false;
        public static bool isOkCanDeactivate = false;

        public static bool canActivate = true;
        public static bool canDeactivate = true;

        public Task<bool> CanActivateAsync(object parameter)
        {
            isOkCanActivate = true;
            pCanActivate = parameter;
            return Task.FromResult(canActivate);
        }

        public Task<bool> CanDeactivateAsync()
        {
            isOkCanDeactivate = true;
            return Task.FromResult(canDeactivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            isOkOnNavigatedTo = true;
            pOnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            isOkOnNavigatingFrom = true;
        }

        public static void Reset()
        {
            isOkOnNavigatedTo = false;
            isOkOnNavigatingTo = false;
            pOnNavigatedTo = null;
            pCanActivate = null;
            pOnNavigatingTo = null;
            isOkOnNavigatingFrom = false;
            isOkCanActivate = false;
            isOkCanDeactivate = false;
            canActivate = true;
            canDeactivate = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            isOkOnNavigatingTo = true;
            pOnNavigatingTo = parameter;
        }
    }

    public class ViewWithViewModelDataContext : ContentControl, ICanActivate, ICanDeactivate
    {

        public ViewWithViewModelDataContext()
        {
            this.DataContext = new Vm();
        }

        public static object pCanActivate = null;
        public static bool isOkCanActivate = false;
        public static bool isOkCanDeactivate = false;
        public static bool canActivate = true;
        public static bool canDeactivate = true;

        public Task<bool> CanActivateAsync(object parameter)
        {
            isOkCanActivate = true;
            pCanActivate = parameter;
            return Task.FromResult(canActivate);
        }

        public Task<bool> CanDeactivateAsync()
        {
            isOkCanDeactivate = true;
            return Task.FromResult(canDeactivate);
        }

        public static void Reset()
        {
            pCanActivate = null;
            isOkCanActivate = false;
            isOkCanDeactivate = false;
            canActivate = true;
            canDeactivate = true;
        }
    }

    public class NavigatableView : UserControl, INavigationAware
    {
        public static bool isOkOnNavigatedTo = false;
        public static object p = null;

        public static bool isOkOnNavigatingFrom = false;

        public void OnNavigatedTo(object parameter)
        {
            isOkOnNavigatedTo = true;
            p = parameter;
        }

        public void OnNavigatingFrom()
        {
            isOkOnNavigatingFrom = true;
        }

        public static void Reset()
        {
            isOkOnNavigatedTo = false;
            p = null;
            isOkOnNavigatingFrom = false;
        }

        public void OnNavigatingTo(object parameter)
        {

        }
    }

    public class ActivatableView : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool isOkOnNavigatedTo = false;
        public static object p = null;
        public static object pCanActivate = null;

        public static bool isOkOnNavigatingFrom = false;

        public static bool isOkCanActivate = false;
        public static bool isOkCanDeactivate = false;

        public static bool canActivate = true;
        public static bool canDeactivate = true;

        public Task<bool> CanActivateAsync(object parameter)
        {
            isOkCanActivate = true;
            pCanActivate = parameter;
            return Task.FromResult(canActivate);
        }

        public Task<bool> CanDeactivateAsync()
        {
            isOkCanDeactivate = true;
            return Task.FromResult(canDeactivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            isOkOnNavigatedTo = true;
            p = parameter;
        }

        public void OnNavigatingFrom()
        {
            isOkOnNavigatingFrom = true;
        }

        public static void Reset()
        {
            isOkOnNavigatedTo = false;
            p = null;
            pCanActivate = null;
            isOkOnNavigatingFrom = false;
            isOkCanActivate = false;
            isOkCanDeactivate = false;
            canActivate = true;
            canDeactivate = true;
        }

        public void OnNavigatingTo(object parameter)
        {

        }
    }

    [TestClass]
    public class NavigationSourceTests
    {
        private const string defaultKey = "default";

        //[TestMethod]
        //public async Task Navigate_View()
        //{
        //    NavigatableView.Reset();

        //    var navigationSource = new NavigationSource();

        //    await navigationSource.NavigateAsync(typeof(NavigatableView), "p1");

        //    Assert.AreEqual(typeof(NavigatableView), navigationSource.Current.GetType());

        //    Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
        //    Assert.AreEqual("p1", NavigatableView.p);
        //    Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);

        //    await navigationSource.NavigateAsync(typeof(SimpleView));
        //    Assert.AreEqual(typeof(SimpleView), navigationSource.Current.GetType());

        //    Assert.AreEqual(true, NavigatableView.isOkOnNavigatingFrom);
        //}

        [TestMethod]
        public async Task Navigate_To_ViewModel()
        {
            Vm.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(Vm), "p1");

            Assert.AreEqual(typeof(Vm), navigationSource.Current.GetType());

            Assert.AreEqual(true, Vm.isOkOnNavigatedTo);
            Assert.AreEqual("p1", Vm.pOnNavigatedTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(true, Vm.isOkCanActivate);
        }

        [TestMethod]
        public async Task Activatable_View_CanActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            ActivatableView.canActivate = true;

            // ICanActivate
            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");
            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);
        }

        [TestMethod]
        public async Task Activatable_View_CannotActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            ActivatableView.canActivate = false;

            await navigationSource.NavigateAsync(typeof(ActivatableView));
            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CanDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView));

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await navigationSource.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(false, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            //Assert.AreEqual(true, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CannotDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView));

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await navigationSource.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(false, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Navigatable_GoBack()
        {
            NavigatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(NavigatableView), "p1");

            Assert.IsFalse(navigationSource.CanGoBack);

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(navigationSource.CanGoBack);

            NavigatableView.Reset();
            await navigationSource.GoBackAsync();

            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);

            // forward
            NavigatableView.Reset();
            await navigationSource.GoBackAsync();

        }

        [TestMethod]
        public async Task Navigatable_GoForward()
        {
            NavigatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.NavigateAsync(typeof(NavigatableView), "p1");

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.GoBackAsync();

            Assert.IsTrue(navigationSource.CanGoForward);

            NavigatableView.Reset();
            await navigationSource.GoForwardAsync();

            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CanActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoBack);

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(navigationSource.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canActivate = true;

            await navigationSource.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CannotActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoBack);

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(navigationSource.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await navigationSource.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CanDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoBack);

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsTrue(navigationSource.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await navigationSource.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CannotDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoBack);

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsTrue(navigationSource.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await navigationSource.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CanActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.GoBackAsync();

            Assert.IsTrue(navigationSource.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canActivate = true;

            await navigationSource.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CannotActivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.GoBackAsync();

            Assert.IsTrue(navigationSource.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await navigationSource.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CanDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.GoBackAsync();

            Assert.IsTrue(navigationSource.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await navigationSource.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CannotDeactivate()
        {
            ActivatableView.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(navigationSource.CanGoForward);

            await navigationSource.GoBackAsync();

            Assert.IsTrue(navigationSource.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await navigationSource.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Navigatable_WithView_And_Vm()
        {
            ViewWithViewModelDataContext.Reset();
            Vm.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            //Assert.AreEqual(true, ViewWithViewModelDataContext.isOkOnNavigatedTo);
            //Assert.AreEqual("p1", ViewWithViewModelDataContext.p);
            //Assert.AreEqual(false, ViewWithViewModelDataContext.isOkOnNavigatingFrom);
            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanActivate);

            Assert.AreEqual(true, Vm.isOkOnNavigatedTo);
            Assert.AreEqual("p1", Vm.pOnNavigatedTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(true, Vm.isOkCanActivate);
        }

        [TestMethod]
        public async Task Guards_WithView_And_Vm()
        {
            ViewWithViewModelDataContext.Reset();
            Vm.Reset();

            var navigationSource = new NavigationSource();

            ViewWithViewModelDataContext.canActivate = false;

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanActivate);
            Assert.AreEqual("p1", ViewWithViewModelDataContext.pCanActivate);
            Assert.AreEqual(false, Vm.isOkCanActivate);
            Assert.AreEqual(false, Vm.isOkOnNavigatingTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatedTo);
            Assert.AreEqual(null, Vm.pOnNavigatedTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);

            ViewWithViewModelDataContext.Reset();
            Vm.Reset();

            Vm.canActivate = false;

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p2");

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanActivate);
            Assert.AreEqual("p2", ViewWithViewModelDataContext.pCanActivate);
            Assert.AreEqual(true, Vm.isOkCanActivate);
            Assert.AreEqual("p2", Vm.pCanActivate);
            Assert.AreEqual(false, Vm.isOkOnNavigatingTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatedTo);
            Assert.AreEqual(null, Vm.pOnNavigatedTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);

            ViewWithViewModelDataContext.Reset();
            Vm.Reset();

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p3");

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanActivate);
            Assert.AreEqual("p3", ViewWithViewModelDataContext.pCanActivate);
            Assert.AreEqual(true, Vm.isOkCanActivate);
            Assert.AreEqual("p3", Vm.pCanActivate);
            Assert.AreEqual(true, Vm.isOkOnNavigatingTo);
            Assert.AreEqual("p3", Vm.pOnNavigatingTo);
            Assert.AreEqual(true, Vm.isOkOnNavigatedTo);
            Assert.AreEqual("p3", Vm.pOnNavigatedTo);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);

            ViewWithViewModelDataContext.Reset();
            Vm.Reset();
            Vm.canDeactivate = false;
            await navigationSource.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(true, Vm.isOkCanDeactivate);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(false, ViewWithViewModelDataContext.isOkCanDeactivate);

            ViewWithViewModelDataContext.Reset();
            Vm.Reset();
            ViewWithViewModelDataContext.canDeactivate = false;
            await navigationSource.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(true, Vm.isOkCanDeactivate);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanDeactivate);

            ViewWithViewModelDataContext.Reset();
            Vm.Reset();
            await navigationSource.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(true, Vm.isOkCanDeactivate);
            Assert.AreEqual(true, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanDeactivate);
        }

        [TestMethod]
        public async Task Navigatable_From_WithView_And_Vm()
        {
            ViewWithViewModelDataContext.Reset();

            var navigationSource = new NavigationSource();

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanDeactivate);
            Assert.AreEqual(true, Vm.isOkCanDeactivate);
            Assert.AreEqual(true, Vm.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Notify_OnNavigatedTo()
        {
            ViewWithViewModelDataContext.Reset();

            var navigationSource = new NavigationSource();

            bool isNotified = false;
            NavigatedEventArgs ev = null;
            navigationSource.Navigated += (s, e) =>
            {
                isNotified = true;
                ev = e;
                navigationSource = null;
            };

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            Assert.IsTrue(isNotified);
            Assert.AreEqual(typeof(ViewWithViewModelDataContext), ev.SourceType);
            Assert.AreEqual("p1", ev.Parameter);
            Assert.AreEqual(NavigationType.New, ev.NavigationType);
        }

        [TestMethod]
        public async Task Notify_OnNavigatingFrom()
        {
            ViewWithViewModelDataContext.Reset();

            var navigationSource = new NavigationSource();

            bool isNotified = false;
            NavigatingEventArgs ev = null;
            navigationSource.Navigating += (s, e) =>
            {
                isNotified = true;
                ev = e;
                navigationSource = null;
            };

            await navigationSource.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");
            await navigationSource.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(isNotified);
            Assert.AreEqual(typeof(ViewWithViewModelDataContext), ev.SourceType);
            Assert.AreEqual("p1", ev.Parameter);
            Assert.AreEqual(NavigationType.New, ev.NavigationType);
        }

        [TestMethod]
        public async Task Notify_OnNavigationFailed()
        {
            var navigationSource = new NavigationSource();

            bool isNotified = false;
            NavigationFailedEventArgs ev = null;
            navigationSource.NavigationFailed += (s, e) =>
            {
                isNotified = true;
                ev = e;
                navigationSource = null;
            };

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await navigationSource.NavigateAsync(typeof(ActivatableView), "p1");

            Assert.IsTrue(isNotified);
            // Assert.AreEqual(typeof(ActivatableView), ev.Exception.OriginalSource.GetType()); ? TODO: details for navigation fail ?
            //Assert.AreEqual("p1", ev.Parameter);
        }

    }
}
