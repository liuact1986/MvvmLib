using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests
{

    //public class FakeContentStrategy : IAnimatedContentStrategy
    //{
    //    public FrameworkElement view;
    //    public object content;
    //    public Action cb;
    //    public EntranceTransitionType navigationTransitionType;

    //    public int DefaultAnimationDuration { get; set; }
    //    public IEasingFunction DefaultEaseFunction { get; set; }
    //    public double DefaultFromBottomValue { get; set; }
    //    public double DefaultFromLeftValue { get; set; }
    //    public double DefaultFromRightValue { get; set; }
    //    public double DefaultFromTopValue { get; set; }
    //    public double DefaultToBottomValue { get; set; }
    //    public double DefaultToLeftValue { get; set; }
    //    public double DefaultToRightValue { get; set; }
    //    public double DefaultToTopValue { get; set; }

    //    public void OnEnter(FrameworkElement view, Action setContentCallback, EntranceTransitionType entranceTransitionType, Action cb = null)
    //    {
    //        this.view = view;
    //        this.navigationTransitionType = entranceTransitionType;
    //        this.cb = cb;

    //        //setContentCallback();
    //        // cb();
    //    }

    //    public void OnLeave(FrameworkElement view, ExitTransitionType exitTransitionType, Action cb = null)
    //    {
    //        cb();
    //    }

    //}

    public class MyContentAdapter : ContentRegionAdapterBase<MyControl>
    {
        public override object GetContent(MyControl control)
        {
            return control.Content;
        }

        public override void OnGoBack(MyControl control, object previousView)
        {
            control.Content = previousView;
        }

        public override void OnGoForward(MyControl control, object nextView)
        {
            control.Content = nextView;
        }

        public override void OnNavigate(MyControl control, object view)
        {
            control.Content = view;
        }
    }

    public class MyControl : ContentControl
    {

    }


    public class SimpleView : UserControl
    {

    }

    public class Vm : INavigatable, IActivatable, IDeactivatable
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

    public class ViewWithViewModelDataContext : ContentControl, INavigatable, IActivatable, IDeactivatable
    {

        public ViewWithViewModelDataContext()
        {
            this.DataContext = new Vm();
        }

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

    public class NavigatableView : UserControl, INavigatable
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

    public class ActivatableView : UserControl, INavigatable, IActivatable, IDeactivatable
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
    public class ContentRegionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            RegionAdapterContainer.RegisterAdapter(new MyContentAdapter());
        }

        public ContentRegion GetService(ContentControl c)
        {
            RegionManager.AddContentRegion("C1", c);
            return new ContentRegion(new NavigationHistory(), "C1", c);
        }

        private const string defaultKey = "default";

        [TestMethod]
        public async Task Navigate_View()
        {
            NavigatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            Assert.AreEqual("R1", service.RegionName);
            Assert.AreEqual(c, service.Control);
            Assert.AreEqual("c1", service.ControlName);

            await service.NavigateAsync(typeof(NavigatableView), "p1");
            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);

            await service.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(true, NavigatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CanActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            ActivatableView.canActivate = true;

            // IActivatable
            await service.NavigateAsync(typeof(ActivatableView), "a1");
            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CannotActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            ActivatableView.canActivate = false;

            await service.NavigateAsync(typeof(ActivatableView));
            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(false, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CanDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView));

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await service.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(false, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_View_CannotDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView));

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await service.NavigateAsync(typeof(SimpleView));
            Assert.AreEqual(false, ActivatableView.isOkCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Navigatable_GoBack()
        {
            NavigatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(NavigatableView), "p1");

            Assert.IsFalse(service.CanGoBack);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(service.CanGoBack);

            NavigatableView.Reset();
            await service.GoBackAsync();

            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);

            // forward
            NavigatableView.Reset();
            await service.GoBackAsync();

        }

        [TestMethod]
        public async Task Navigatable_GoForward()
        {
            NavigatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoForward);

            await service.NavigateAsync(typeof(NavigatableView), "p1");

            Assert.IsFalse(service.CanGoForward);

            await service.GoBackAsync();

            Assert.IsTrue(service.CanGoForward);

            NavigatableView.Reset();
            await service.GoForwardAsync();

            Assert.AreEqual(true, NavigatableView.isOkOnNavigatedTo);
            Assert.AreEqual("p1", NavigatableView.p);
            Assert.AreEqual(false, NavigatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CanActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoBack);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(service.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canActivate = true;

            await service.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CannotActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoBack);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(service.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await service.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CanDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoBack);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsTrue(service.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await service.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoBack_CannotDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoBack);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsTrue(service.CanGoBack);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await service.GoBackAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CanActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoForward);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoForward);

            await service.GoBackAsync();

            Assert.IsTrue(service.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canActivate = true;

            await service.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CannotActivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoForward);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoForward);

            await service.GoBackAsync();

            Assert.IsTrue(service.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await service.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanActivate);
            Assert.AreEqual("a1", ActivatableView.pCanActivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatedTo);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CanDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoForward);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoForward);

            await service.GoBackAsync();

            Assert.IsTrue(service.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = true;

            await service.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(true, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Activatable_GoForward_CannotDeactivate()
        {
            ActivatableView.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ActivatableView), "a1");

            Assert.IsFalse(service.CanGoForward);

            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsFalse(service.CanGoForward);

            await service.GoBackAsync();

            Assert.IsTrue(service.CanGoForward);

            ActivatableView.Reset();
            ActivatableView.canDeactivate = false;

            await service.GoForwardAsync();

            Assert.AreEqual(true, ActivatableView.isOkCanDeactivate);
            Assert.AreEqual(false, ActivatableView.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Navigatable_WithView_And_Vm()
        {
            ViewWithViewModelDataContext.Reset();
            Vm.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkOnNavigatedTo);
            Assert.AreEqual("p1", ViewWithViewModelDataContext.p);
            Assert.AreEqual(false, ViewWithViewModelDataContext.isOkOnNavigatingFrom);
            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanActivate);

            Assert.AreEqual(true, Vm.isOkOnNavigatedTo);
            Assert.AreEqual("p1", Vm.p);
            Assert.AreEqual(false, Vm.isOkOnNavigatingFrom);
            Assert.AreEqual(true, Vm.isOkCanActivate);
        }

        [TestMethod]
        public async Task Navigatable_From_WithView_And_Vm()
        {
            ViewWithViewModelDataContext.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            await service.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            await service.NavigateAsync(typeof(SimpleView));

            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkCanDeactivate);
            Assert.AreEqual(true, ViewWithViewModelDataContext.isOkOnNavigatingFrom);

            Assert.AreEqual(true, Vm.isOkCanDeactivate);
            Assert.AreEqual(true, Vm.isOkOnNavigatingFrom);
        }

        [TestMethod]
        public async Task Notify_OnNavigatedTo()
        {
            ViewWithViewModelDataContext.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            bool isNotified = false;
            RegionNavigationEventArgs ev = null;
            service.Navigated += (s, e) =>
            {
                isNotified = true;
                ev = e;
                service = null;
            };

            await service.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");

            Assert.IsTrue(isNotified);
            Assert.AreEqual(typeof(ViewWithViewModelDataContext), ev.SourcePageType);
            Assert.AreEqual("p1", ev.Parameter);
            Assert.AreEqual(RegionNavigationType.New, ev.RegionNavigationType);       
        }

        [TestMethod]
        public async Task Notify_OnNavigatingFrom()
        {
            ViewWithViewModelDataContext.Reset();

            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            bool isNotified = false;
            RegionNavigationEventArgs ev = null;
            service.Navigating += (s, e) =>
            {
                isNotified = true;
                ev = e;
                service = null;
            };

            await service.NavigateAsync(typeof(ViewWithViewModelDataContext), "p1");
            await service.NavigateAsync(typeof(SimpleView));

            Assert.IsTrue(isNotified);
            Assert.AreEqual(typeof(ViewWithViewModelDataContext), ev.SourcePageType);
            Assert.AreEqual("p1", ev.Parameter);
            Assert.AreEqual(RegionNavigationType.New, ev.RegionNavigationType);
        }

        [TestMethod]
        public async Task Notify_OnNavigationFailed()
        {
            var c = new MyControl();
            c.Name = "c1";
            var service = new ContentRegion("R1", c);

            bool isNotified = false;
            RegionNavigationFailedEventArgs ev = null;
            service.NavigationFailed += (s, e) =>
            {
                isNotified = true;
                ev = e;
                service = null;
            };

            ActivatableView.Reset();
            ActivatableView.canActivate = false;

            await service.NavigateAsync(typeof(ActivatableView), "p1");

            Assert.IsTrue(isNotified);
            Assert.AreEqual(typeof(ActivatableView), ev.Exception.ViewOrContext.GetType());
            //Assert.AreEqual("p1", ev.Parameter);
        }
    }
}
