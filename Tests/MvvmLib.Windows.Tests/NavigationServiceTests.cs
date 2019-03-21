using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using MvvmLib.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Windows.Tests
{
    public class NavigationEntry
    {
        public Type SourceType { get; }
        public object View { get; }
        public object Parameter { get; }
        public object Context { get; }

        public NavigationEntry(Type viewType, object view, object parameter, object context)
        {
            this.SourceType = viewType;
            this.View = view;
            this.Parameter = parameter;
            this.Context = context;
        }
    }

    public class NavigationHistory
    {
        public Stack<NavigationEntry> BackStack { get; }
        public Stack<NavigationEntry> ForwardStack { get; }

        public NavigationEntry Root => this.BackStack.Count > 0 ? this.BackStack.LastOrDefault() : null;

        public NavigationEntry Previous => this.BackStack.Count > 0 ? this.BackStack.Peek() : null;

        public NavigationEntry Next => this.ForwardStack.Count > 0 ? this.ForwardStack.Peek() : null;

        public NavigationEntry Current { get; protected set; }

        public NavigationHistory()
        {
            this.BackStack = new Stack<NavigationEntry>();
            this.ForwardStack = new Stack<NavigationEntry>();
        }

        public void Clear()
        {
            this.ForwardStack.Clear();
            this.BackStack.Clear();
            this.Current = null;
        }

        public void Navigate(NavigationEntry entry)
        {
            if (this.Current != null)
            {
                this.BackStack.Push(this.Current);
            }

            this.Current = entry;

            this.ForwardStack.Clear();
        }

        public NavigationEntry GoBack()
        {
            this.ForwardStack.Push(this.Current);
            this.Current = this.BackStack.Pop();
            return this.Current;
        }

        public NavigationEntry GoForward()
        {
            this.BackStack.Push(this.Current);
            this.Current = this.ForwardStack.Pop();
            return this.Current;
        }
    }

    public class FakeFacade : IFrameFacade
    {
        public NavigationHistory History { get; set; } = new NavigationHistory();

        public bool CanGoBack => History.BackStack.Count > 0;

        public bool CanGoForward => History.ForwardStack.Count > 0;

        private object content;
        public object Content => content;

        public event EventHandler CanGoBackChanged;
        public event EventHandler CanGoForwardChanged;
        public event EventHandler<FrameNavigatedEventArgs> Navigated;
        public event EventHandler<FrameNavigatingEventArgs> Navigating;

        public string GetNavigationState()
        {
            return "navigationState";
        }

        public bool IsGoBackInvoked { get; set; }
        public bool IsGoFowardInvoked { get; set; }
        public bool IsNavInvoked { get; set; }
        public object NavParam { get; set; }
        public bool IsSetNavState { get; set; }

        public void GoBack()
        {
            IsGoBackInvoked = true;
        }

        public void GoBack(NavigationTransitionInfo infoOverride)
        {
            IsGoBackInvoked = true;
        }

        public void GoForward()
        {
            IsGoFowardInvoked = true;
        }

        public void Navigate(Type sourcePageType)
        {
            content = Activator.CreateInstance(sourcePageType);
            IsNavInvoked = true;
        }

        public void Navigate(Type sourcePageType, object parameter)
        {
            content = Activator.CreateInstance(sourcePageType);
            IsNavInvoked = true;
            NavParam = parameter;
        }

        public void Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            content = Activator.CreateInstance(sourcePageType);
            IsNavInvoked = true;
            NavParam = parameter;
        }

        public void SetNavigationState(string navigationState)
        {
            IsSetNavState = true;
        }

        public void Reset()
        {
            IsGoBackInvoked = false;
            IsGoFowardInvoked = false;
            IsNavInvoked = false;
            IsSetNavState = false;
            NavParam = null;
            content = null;
        }
    }

    [TestClass]
    public class NavigationServiceTests
    {
        public FrameNavigationService GetService(IFrameFacade frameFacade)
        {
            return new FrameNavigationService(frameFacade);
        }

        [TestMethod]
        public async Task Navigate()
        {
            var facade = new FakeFacade();
            var service = GetService(facade);

            await service.NavigateAsync(typeof(FakePage));
            Assert.IsTrue(facade.IsNavInvoked);
            Assert.IsNull(facade.NavParam);

            facade.Reset();

            await service.NavigateAsync(typeof(FakePage), "p1");
            Assert.IsTrue(facade.IsNavInvoked);
            Assert.AreEqual("p1", facade.NavParam);

            facade.Reset();

            await service.NavigateAsync(typeof(FakePage), "p2", null);
            Assert.IsTrue(facade.IsNavInvoked);
            Assert.AreEqual("p2", facade.NavParam);
        }

        [TestMethod]
        public async Task GoBack()
        {
            var facade = new FakeFacade();
            var service = GetService(facade);

            await service.GoBackAsync();
            Assert.IsTrue(facade.IsGoBackInvoked);

            facade.Reset();

            await service.GoBackAsync(null);
            Assert.IsTrue(facade.IsGoBackInvoked);
        }

        [TestMethod]
        public async Task GoForward()
        {
            var facade = new FakeFacade();
            var service = GetService(facade);

            await service.GoForwardAsync();
            Assert.IsTrue(facade.IsGoFowardInvoked);
        }

        //[UITestMethod]
        //public void CanActivateIsCalled()
        //{
        //    var facade = new FakeFacade();
        //    var service = GetService(facade);

        //    service.NavigateAsync(typeof(FakePageWithDataContext));
        //    Assert.IsTrue(FakeViewModel.IsCanNavigateInvoked);
        //    FakeViewModel.Reset();

        //    service.GoBackAsync();
        //    Assert.IsTrue(FakeViewModel.IsCanNavigateInvoked);
        //    FakeViewModel.Reset();

        //    service.GoForwardAsync();
        //    Assert.IsTrue(FakeViewModel.IsCanNavigateInvoked);
        //}
    }

    public class FakePage 
    {

    }

    public class FakePageWithDataContext : FrameworkElement
    {
        public FakePageWithDataContext()
        {
            this.DataContext = new FakeViewModel();
        }
    }

    public class FakeViewModel : INavigatable, IDeactivatable
    {
        public static bool IsNavigatingToInvoked { get; set; }
        public static bool IsNavigatedToInvoked { get; set; }
        public static object NavParam { get; set; }
        public static bool IsSuspending { get; set; }

        public static bool IsCanNavigateInvoked { get; set; }
        public static bool CanDeactivate { get; set; }

        public Task<bool> CanDeactivateAsync()
        {
            return Task.FromResult(CanDeactivate);
        }

        public void OnNavigatedTo(object parameter, NavigationMode navigationMode)
        {
            IsNavigatingToInvoked = true;
            NavParam = parameter;
        }

        public void OnNavigatingFrom(bool isSuspending)
        {
            IsNavigatingToInvoked = true;
            IsSuspending = isSuspending;
        }

        public static void Reset()
        {
            IsNavigatingToInvoked = false;
            IsNavigatedToInvoked = false;
            NavParam = null;
            IsSuspending = false;
            IsCanNavigateInvoked = false;
            CanDeactivate = false;
        }
    }

}
