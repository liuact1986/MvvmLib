using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using MvvmLib.Wpf.Tests.ViewModels;
using MvvmLib.Wpf.Tests.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Navigation
{

    [TestClass]
    public class NavigationHelperTests
    {
        [TestMethod]
        public void CanActivate_View_And_ViewModel()
        {
            var view = new MyViewCanActivate();
            var viewModel = view.DataContext as MyViewModelCanActivate;
            bool? success = null;

            view.Reset();
            viewModel.Reset();
            view.CActivate = false;
            success = NavigationHelper.CanActivate(view, new NavigationContext(typeof(MyViewCanActivate), "p")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);
            Assert.AreEqual(false, viewModel.IsCanActivateInvoked);
            Assert.AreEqual(null, viewModel.P);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            viewModel.CActivate = false;
            success = NavigationHelper.CanActivate(view, new NavigationContext(typeof(MyViewCanActivate), "p2")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.P);
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            success = NavigationHelper.CanActivate(view, new NavigationContext(typeof(MyViewCanActivate), "p3")); // true
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p3", view.P);
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p3", viewModel.P);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void CanActivate_ViewModel()
        {
            var viewModel = new MyViewModelCanActivate();
            bool? success = null;

            viewModel.Reset();
            viewModel.CActivate = false;
            success = NavigationHelper.CanActivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p")); // false
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p", viewModel.P);
            Assert.AreEqual(false, success);

            viewModel.Reset();
            success = NavigationHelper.CanActivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p2")); // true
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void Delayed_CanActivate()
        {
            var viewModel = new DelayedViewModel();
            bool? success = null;

            viewModel.CActivate = false;
            success = NavigationHelper.CanActivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p")); // false
            Assert.AreEqual(false, success);

            viewModel.CActivate = true;
            success = NavigationHelper.CanActivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p2")); // true
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void Delayed_CanDeactivate()
        {
            var viewModel = new DelayedViewModel();
            bool? success = null;

            viewModel.CDeactivate = false;
            success = NavigationHelper.CanDeactivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p")); // false
            Assert.AreEqual(false, success);

            viewModel.CDeactivate = true;
            success = NavigationHelper.CanDeactivate(viewModel, new NavigationContext(typeof(MyViewModelCanActivate), "p2")); // true
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void CanActivate_With_No_ViewModel_Not_Throw()
        {
            var view = new MyViewCanActivateWithoutContext();
            bool? success = null;

            view.Reset();
            view.CActivate = false;
            success = NavigationHelper.CanActivate(view, new NavigationContext(typeof(MyViewCanActivateWithoutContext), "p")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);
            Assert.AreEqual(false, success);

            view.Reset();
            success = NavigationHelper.CanActivate(view, new NavigationContext(typeof(MyViewCanActivateWithoutContext), "p2")); // true
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.P);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void CanDeactivate_View_And_ViewModel()
        {
            var view = new MyViewCanDeactivate();
            var viewModel = view.DataContext as MyViewModelCanDeactivate;
            bool? success = null;

            view.Reset();
            view.CDeactivate = false;
            success = NavigationHelper.CanDeactivate(view, new NavigationContext(typeof(MyViewCanDeactivate), "p")); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(false, viewModel.IsCanDeactivateInvoked);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            viewModel.CDeactivate = false;
            success = NavigationHelper.CanDeactivate(view, new NavigationContext(typeof(MyViewCanDeactivate), "p2")); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(true, viewModel.IsCanDeactivateInvoked);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            success = NavigationHelper.CanDeactivate(view, new NavigationContext(typeof(MyViewCanDeactivate), "p3")); ; // true
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(true, viewModel.IsCanDeactivateInvoked);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void Resolve_ViewModel_With_Default_Convention()
        {
            var viewModel = NavigationHelper.ResolveViewModelWithViewModelLocator(typeof(MyViewA));
            Assert.AreEqual(typeof(MyViewAViewModel), viewModel.GetType());
        }

        [TestMethod]
        public void Not_Resolve_Not_Existing_ViewModel_With_Default_Convention()
        {
            var viewModel = NavigationHelper.ResolveViewModelWithViewModelLocator(typeof(MyViewCanActivate));
            Assert.AreEqual(null, viewModel);
        }

        [TestMethod]
        public void OnNavigatingFrom_With_ViewModel()
        {
            var viewModel = new MyViewModelNavigationAware();

            viewModel.Reset();

            NavigationHelper.OnNavigatingFrom(viewModel, new NavigationContext(typeof(MyViewModelNavigationAware), null));
            Assert.AreEqual(true, viewModel.IsOnNavigatingFromInvoked);
        }

        [TestMethod]
        public void OnNavigatingFrom_With_View()
        {
            var view = new MyViewWithViewModelNavigationAware();
            var viewModel = view.DataContext as MyViewModelNavigationAware;

            viewModel.Reset();

            NavigationHelper.OnNavigatingFrom(view, new NavigationContext(typeof(MyViewWithViewModelNavigationAware), null));
            Assert.AreEqual(true, viewModel.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, view.IsOnNavigatingFromInvoked);
        }

        [TestMethod]
        public void OnNavigatingTo_With_ViewModel()
        {
            var viewModel = new MyViewModelNavigationAware();

            viewModel.Reset();

            NavigationHelper.OnNavigatingTo(viewModel, new NavigationContext(typeof(MyViewModelNavigationAware), "p"));
            Assert.AreEqual(true, viewModel.IsOnNavigatingToInvoked);
            Assert.AreEqual("p", viewModel.POnNavigatingTo);
        }

        [TestMethod]
        public void OnNavigatedTo_With_ViewModel()
        {
            var viewModel = new MyViewModelNavigationAware();

            viewModel.Reset();

            NavigationHelper.OnNavigatedTo(viewModel, new NavigationContext(typeof(MyViewModelNavigationAware), "p"));
            Assert.AreEqual(true, viewModel.IsOnNavigatedToInvoked);
            Assert.AreEqual("p", viewModel.POnNavigatedTo);
        }

        [TestMethod]
        public void FindSelectable()
        {
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               new MySelectableViewModel{ Id = 2 },
               new MySelectableViewModel{ Id = 3 }
           };

            var selectable = NavigationHelper.FindSelectable(sources, typeof(MySelectableViewModel), 2);

            Assert.IsNotNull(selectable);
            Assert.AreEqual(typeof(MySelectableViewModel), selectable.GetType());
            Assert.AreEqual(2, ((MySelectableViewModel)selectable).Id);
        }

        [TestMethod]
        public void Not_FindSelectable_For_Criteria()
        {
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               new MySelectableViewModel{ Id = 2 },
               new MySelectableViewModel{ Id = 3 }
           };

            var selectable = NavigationHelper.FindSelectable(sources, typeof(MySelectableViewModel), 4);

            Assert.IsNull(selectable);
        }

        [TestMethod]
        public void Check_Selectable_Only_For_The_SourceType()
        {
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               new MySelectableViewModel{ Id = 2 },
               new MySelectableViewModel{ Id = 3 }
           };

            var selectable = NavigationHelper.FindSelectable(sources, typeof(MyViewModelCanDeactivate), 2);

            Assert.IsNull(selectable);
        }

        [TestMethod]
        public void Find_ViewModel_Selectable_For_The_Source_As_View()
        {
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MyViewWithSelectableViewModel (1),
               new MyViewModelCanActivate(),
               new MyViewWithSelectableViewModel (2),
               new MyViewWithSelectableViewModel (3),
           };

            var selectable = NavigationHelper.FindSelectable(sources, typeof(MyViewWithSelectableViewModel), 2);

            Assert.IsNotNull(selectable);
            Assert.AreEqual(typeof(MyViewWithSelectableViewModel), selectable.GetType());
            Assert.AreEqual(2, ((MySelectableViewModel)((MyViewWithSelectableViewModel)selectable).DataContext).Id);
        }

    }

    public class MySimpleViewModel
    {

    }

    public class MySelectableViewModel : ISelectable, IMyViewModel
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }
    }

    public class MyViewThatChangeParameter : UserControl, ICanActivate, ICanDeactivate
    {
        public MyViewThatChangeParameter()
        {
            DataContext = new MyViewModelThatChangeParameter();
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-canactivateview-";
            return Task.FromResult(true);
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-candeactivateview-";
            return Task.FromResult(true);
        }
    }

    public class MyViewModelThatChangeParameter : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static string Parameter = null;

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-canactivateviewmodel-";
            Parameter = (string)navigationContext.Parameter;
            return Task.FromResult(true);
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-candeactivateviewmodel-";
            Parameter = (string)navigationContext.Parameter;
            return Task.FromResult(true);
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-onavigatingfromviewmodel-";
            Parameter = (string)navigationContext.Parameter;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-onavigatingtoviewmodel-";
            Parameter = (string)navigationContext.Parameter;
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            navigationContext.Parameter = navigationContext.Parameter + "-navigatedtoviewmodel-";
            Parameter = (string)navigationContext.Parameter;
        }
    }


    public class DelayedViewModel : ICanActivate, ICanDeactivate, INavigationAware
    {
        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public bool IsCanDeactivateInvoked { get; private set; }
        public object PCanDeactivate { get; private set; }
        public object PCanActivate { get; private set; }
        public bool CDeactivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsCanDeactivateInvoked = false;
            PCanDeactivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
        }

        public async Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            await Task.Delay(1000).ConfigureAwait(false);
            return CActivate;
        }

        public async Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            IsCanDeactivateInvoked = true;
            PCanDeactivate = navigationContext.Parameter;
            await Task.Delay(1000).ConfigureAwait(false);
            return CDeactivate;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }


    public class DelayedViewModelStatic : ICanActivate, ICanDeactivate, INavigationAware
    {
       public static bool CActivate { get; set; }
       public static bool IsCanActivateInvoked { get; private set; }
       public static bool IsCanDeactivateInvoked { get; private set; }
       public static object PCanDeactivate { get; private set; }
       public static object PCanActivate { get; private set; }
       public static bool CDeactivate { get; set; }
       public static bool IsOnNavigatedToInvoked { get; private set; }
       public static object POnNavigatedTo { get; private set; }
       public static bool IsOnNavigatingFromInvoked { get; private set; }
       public static bool IsOnNavigatingToInvoked { get; private set; }
       public static object POnNavigatingTo { get; private set; }

        public static void Reset()
        {
            CActivate = true;
            CDeactivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsCanDeactivateInvoked = false;
            PCanDeactivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
        }

        public async Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            await Task.Delay(1000).ConfigureAwait(false);
            return CActivate;
        }

        public async Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            IsCanDeactivateInvoked = true;
            PCanDeactivate = navigationContext.Parameter;
            await Task.Delay(1000).ConfigureAwait(false);
            return CDeactivate;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MySelectableViewModelWithGuards : ISelectable, ICanActivate, INavigationAware
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }

        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MySelectableViewModelWithGuardsStatic : ISelectable, ICanActivate, INavigationAware
    {
        public int Id { get; set; }

        public MySelectableViewModelWithGuardsStatic()
        {
            Id = 1;
        }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyViewGuardedWithSelectableViewModelWithGuards : UserControl, ISelectable, ICanActivate, INavigationAware
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }

        public MyViewGuardedWithSelectableViewModelWithGuards()
        {
            DataContext = new MySelectableViewModelWithGuardsStatic();
        }

        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            this.IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyViewWithSelectableViewModel : UserControl
    {
        public MyViewWithSelectableViewModel(int id)
        {
            this.DataContext = new MySelectableViewModel { Id = id };
        }
    }

    public class MyViewCanActivate : UserControl, ICanActivate
    {
        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object P { get; set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            P = null;

        }

        public MyViewCanActivate()
        {
            this.DataContext = new MyViewModelCanActivate();
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            this.IsCanActivateInvoked = true;
            P = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }
    }

    public class MyViewCanActivateWithoutContext : UserControl, ICanActivate
    {
        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object P { get; set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            P = null;

        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            this.IsCanActivateInvoked = true;
            P = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }
    }

    public class MyViewCanDeactivate : UserControl, ICanDeactivate
    {
        public bool CDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }

        public void Reset()
        {
            CDeactivate = true;
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            return Task.FromResult(CDeactivate);
        }

        public MyViewCanDeactivate()
        {
            this.IsCanDeactivateInvoked = true;
            this.DataContext = new MyViewModelCanDeactivate();
        }
    }

    public class MyViewModelCanActivate : ICanActivate, IMyViewModel
    {
        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object P { get; set; }

        public void Reset()
        {
            CActivate = true;
            IsCanActivateInvoked = false;
            P = null;
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            this.IsCanActivateInvoked = true;
            P = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }
    }

    public interface IMyViewModel
    {

    }

    public class MyViewModelCanDeactivate : ICanDeactivate, IMyViewModel
    {
        public bool CDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }

        public void Reset()
        {
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            this.IsCanDeactivateInvoked = true;
            return Task.FromResult(CDeactivate);
        }
    }

    public class MyViewWithViewModelNavigationAware : UserControl, INavigationAware
    {
        public MyViewWithViewModelNavigationAware()
        {
            this.DataContext = new MyViewModelNavigationAware();
        }

        public bool IsOnNavigatedToInvoked { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
        }
    }

    public class MyViewModelNavigationAware : INavigationAware
    {
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuardsStatic : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CDeactivate);
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuards : INavigationAware, ICanActivate, ICanDeactivate
    {
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public bool CActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }

        public bool CDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }


        public void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CDeactivate);
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MYVIEWWITHViewModelNavigationAwareAndGuards : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MYVIEWWITHViewModelNavigationAwareAndGuards()
        {
            DataContext = new MyViewModelNavigationAwareAndGuardsStatic();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CDeactivate);
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            return Task.FromResult(CActivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }
}
