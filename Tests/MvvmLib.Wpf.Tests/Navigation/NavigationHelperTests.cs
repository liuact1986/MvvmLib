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
        public void IsFrameworkElementType()
        {
            Assert.AreEqual(true, NavigationHelper.IsFrameworkElementType(typeof(UserControl)));
            Assert.AreEqual(true, NavigationHelper.IsFrameworkElementType(typeof(Window)));

            Assert.AreEqual(false, NavigationHelper.IsFrameworkElementType(typeof(MyViewModelCanActivate)));
        }

        [TestMethod]
        public async Task CanActivate_View_And_ViewModel()
        {
            var view = new MyViewCanActivate();
            var viewModel = new MyViewModelCanActivate();

            view.Reset();
            viewModel.Reset();
            view.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(view, viewModel, "p")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);
            Assert.AreEqual(false, viewModel.IsCanActivateInvoked);
            Assert.AreEqual(null, viewModel.P);

            view.Reset();
            viewModel.Reset();
            viewModel.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(view, viewModel, "p2")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.P);
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);

            view.Reset();
            viewModel.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanActivateAsync(view, viewModel, "p3")); // true
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p3", view.P);
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p3", viewModel.P);
        }

        [TestMethod]
        public async Task CanActivate_ViewModel()
        {
            var viewModel = new MyViewModelCanActivate();

            viewModel.Reset();
            viewModel.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(viewModel, "p")); // false
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p", viewModel.P);

            viewModel.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanActivateAsync(viewModel, "p2")); // true
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);
        }

        [TestMethod]
        public async Task CanActivate_With_No_ViewModel_Not_Throw()
        {
            var view = new MyViewCanActivate();

            view.Reset();
            view.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanActivateAsync(view, null, "p")); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);

            view.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanActivateAsync(view, null, "p2")); // true
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.P);
        }

        [TestMethod]
        public async Task CanDeactivate_View_And_ViewModel()
        {
            var view = new MyViewCanDeactivate();
            var viewModel = view.DataContext as MyViewModelCanDeactivate;

            view.Reset();
            view.CanDeactivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanDeactivateAsync(view)); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(false, viewModel.IsCanDeactivateInvoked);

            view.Reset();
            viewModel.Reset();
            viewModel.CanDeactivate = false;
            Assert.AreEqual(false, await NavigationHelper.CanDeactivateAsync(view)); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(true, viewModel.IsCanDeactivateInvoked);

            view.Reset();
            viewModel.Reset();
            Assert.AreEqual(true, await NavigationHelper.CanDeactivateAsync(view)); // true
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(true, viewModel.IsCanDeactivateInvoked);
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

            NavigationHelper.OnNavigatingFrom(viewModel);
            Assert.AreEqual(true, viewModel.IsOnNavigatingFromInvoked);
        }

        [TestMethod]
        public void OnNavigatingFrom_With_View()
        {
            var view = new MyViewWithViewModelNavigationAware();
            var viewModel = view.DataContext as MyViewModelNavigationAware;

            viewModel.Reset();

            NavigationHelper.OnNavigatingFrom(view);
            Assert.AreEqual(true, viewModel.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, view.IsOnNavigatingFromInvoked);
        }

        [TestMethod]
        public void OnNavigatingTo_With_ViewModel()
        {
            var viewModel = new MyViewModelNavigationAware();

            viewModel.Reset();

            NavigationHelper.OnNavigatingTo(viewModel, "p");
            Assert.AreEqual(true, viewModel.IsOnNavigatingToInvoked);
            Assert.AreEqual("p", viewModel.POnNavigatingTo);
        }

        [TestMethod]
        public void OnNavigatedTo_With_ViewModel()
        {
            var viewModel = new MyViewModelNavigationAware();

            viewModel.Reset();

            NavigationHelper.OnNavigatedTo(viewModel, "p");
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
        public async Task FindSelectable_With_Navigate()
        {
            var current = new MyViewModelNavigationAware();
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               new MySelectableViewModel{ Id = 2 },
               new MySelectableViewModel{ Id = 3 }
           };
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            Assert.AreEqual(true, await NavigationHelper.NavigateAsync(current, sources, typeof(MySelectableViewModel), 2, setCurrent));

            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.IsNotNull(setCurrentObject);
            Assert.AreEqual(typeof(MySelectableViewModel), setCurrentObject.GetType());
            Assert.AreEqual(2, ((MySelectableViewModel)setCurrentObject).Id);
            Assert.AreEqual(sources[3], (MySelectableViewModel)setCurrentObject);
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

        [TestMethod]
        public async Task ProcessNavigate_With_ViewModel()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CanActivate = false;

            Assert.AreEqual(false, await NavigationHelper.EndNavigateAsync(current, typeof(MyViewModelNavigationAwareAndGuardsStatic), "p", setCurrent));
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            Assert.AreEqual(true, await NavigationHelper.EndNavigateAsync(current, typeof(MyViewModelNavigationAwareAndGuardsStatic), "p2", setCurrent));
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), setCurrentObject.GetType());
        }

        [TestMethod]
        public async Task ProcessNavigate_With_Selectable()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            var selectable = new MySelectableViewModelWithGuards();
            selectable.CanActivate = false;

            Assert.AreEqual(false, await NavigationHelper.EndNavigateWithSelectableAsync(current, selectable, "p", setCurrent));
            Assert.AreEqual(true, selectable.IsCanActivateInvoked);
            Assert.AreEqual("p", selectable.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            selectable.CanActivate = true;
            Assert.AreEqual(true, await NavigationHelper.EndNavigateWithSelectableAsync(current, selectable, "p2", setCurrent));
            Assert.AreEqual(true, selectable.IsCanActivateInvoked);
            Assert.AreEqual("p2", selectable.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, selectable.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, selectable.POnNavigatingTo);
            Assert.AreEqual(false, selectable.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, selectable.POnNavigatedTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(selectable, setCurrentObject);
        }

        [TestMethod]
        public async Task ProcessNavigate_With_Selectable_View()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            var view = new MyViewGuardedWithSelectableViewModelWithGuards();
            MySelectableViewModelWithGuardsStatic.Reset();
            view.CanActivate = false;

            Assert.AreEqual(false, await NavigationHelper.EndNavigateWithSelectableAsync(current, view, "p", setCurrent));
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.PCanActivate);
            Assert.AreEqual(false, MySelectableViewModelWithGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MySelectableViewModelWithGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            MySelectableViewModelWithGuardsStatic.Reset();
            view.CanActivate = true;
            MySelectableViewModelWithGuardsStatic.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.EndNavigateWithSelectableAsync(current, view, "p2", setCurrent));
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.PCanActivate);
            Assert.AreEqual(true, MySelectableViewModelWithGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MySelectableViewModelWithGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            MySelectableViewModelWithGuardsStatic.Reset();
            view.CanActivate = true;

            Assert.AreEqual(true, await NavigationHelper.EndNavigateWithSelectableAsync(current, view, "p3", setCurrent));
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p3", view.PCanActivate);
            Assert.AreEqual(true, MySelectableViewModelWithGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p3", MySelectableViewModelWithGuardsStatic.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, view.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, view.POnNavigatingTo);
            Assert.AreEqual(false, view.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, view.POnNavigatedTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(view, setCurrentObject);
        }

        [TestMethod]
        public async Task ProcessNavigate_With_View()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.EndNavigateAsync(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p", setCurrent));
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.EndNavigateAsync(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p2", setCurrent));
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p2", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);


            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            Assert.AreEqual(true, await NavigationHelper.EndNavigateAsync(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p3", setCurrent));
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p3", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);

            // do not navigate with view, INavigationAware is for ViewModels
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);

            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), setCurrentObject.GetType());
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), ((FrameworkElement)setCurrentObject).DataContext.GetType());
        }

        [TestMethod]
        public async Task Process_Side_Navigation_With_ViewModel()
        {
            var current = new MyViewModelNavigationAwareAndGuards();
            bool isSetCurrentInvoked = false;
            var setCurrent = new Action(() =>
            {
                isSetCurrentInvoked = true;
            });

            var toGo = new MyViewModelNavigationAwareAndGuards();
            current.CanDeactivate = false;

            Assert.AreEqual(false, await NavigationHelper.ReplaceAsync(current, toGo, "p", setCurrent));
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(false, toGo.IsCanActivateInvoked);
            Assert.AreEqual(null, toGo.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            current.Reset();
            toGo.Reset();
            current.CanDeactivate = true;
            toGo.CanActivate = false;

            Assert.AreEqual(false, await NavigationHelper.ReplaceAsync(current, toGo, "p2", setCurrent));
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(true, toGo.IsCanActivateInvoked);
            Assert.AreEqual("p2", toGo.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);

            current.Reset();
            toGo.Reset();
            current.CanDeactivate = true;
            toGo.CanActivate = true;

            Assert.AreEqual(true, await NavigationHelper.ReplaceAsync(current, toGo, "p3", setCurrent));
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(true, toGo.IsCanActivateInvoked);
            Assert.AreEqual("p3", toGo.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, toGo.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", toGo.POnNavigatingTo);
            Assert.AreEqual(true, toGo.IsOnNavigatedToInvoked);
            Assert.AreEqual("p3", toGo.POnNavigatedTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
        }
    }

    public class MySelectableViewModel : ISelectable
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }
    }

    public class MySelectableViewModelWithGuards : ISelectable, ICanActivate, INavigationAware
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }

        public bool CanActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MySelectableViewModelWithGuardsStatic : ISelectable, ICanActivate, INavigationAware
    {
        public int Id { get; set; }

        public bool IsTarget(Type sourceType, object parameter)
        {
            return (int)parameter == Id;
        }

        public static bool CanActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static void Reset()
        {
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
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

        public bool CanActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public void Reset()
        {
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            this.IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
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
        public bool CanActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object P { get; set; }

        public void Reset()
        {
            CanActivate = true;
            IsCanActivateInvoked = false;
            P = null;

        }

        public MyViewCanActivate()
        {
            this.DataContext = new MyViewModelCanActivate();
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            this.IsCanActivateInvoked = true;
            P = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class MyViewCanDeactivate : UserControl, ICanDeactivate
    {
        public bool CanDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }

        public void Reset()
        {
            CanDeactivate = true;
        }

        public Task<bool> CanDeactivateAsync()
        {
            return Task.FromResult(CanDeactivate);
        }

        public MyViewCanDeactivate()
        {
            this.IsCanDeactivateInvoked = true;
            this.DataContext = new MyViewModelCanDeactivate();
        }
    }

    public class MyViewModelCanActivate : ICanActivate
    {
        public bool CanActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object P { get; set; }

        public void Reset()
        {
            CanActivate = true;
            IsCanActivateInvoked = false;
            P = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            this.IsCanActivateInvoked = true;
            P = parameter;
            return Task.FromResult(CanActivate);
        }
    }

    public class MyViewModelCanDeactivate : ICanDeactivate
    {
        public bool CanDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }

        public void Reset()
        {
            CanDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivateAsync()
        {
            this.IsCanDeactivateInvoked = true;
            return Task.FromResult(CanDeactivate);
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

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
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

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuardsStatic : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CanActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CanDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CanDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivateAsync()
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuards : INavigationAware, ICanActivate, ICanDeactivate
    {
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }

        public bool CanActivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }

        public bool CanDeactivate { get; set; }
        public bool IsCanDeactivateInvoked { get; private set; }


        public void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CanDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivateAsync()
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MYVIEWWITHViewModelNavigationAwareAndGuards : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CanActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CanDeactivate { get; set; }
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
            CanActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CanDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public Task<bool> CanDeactivateAsync()
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CanDeactivate);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }
}
