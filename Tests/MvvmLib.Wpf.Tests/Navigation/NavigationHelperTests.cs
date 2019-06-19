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
            NavigationHelper.CanActivate(view, "p", t => { success = t; }); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);
            Assert.AreEqual(false, viewModel.IsCanActivateInvoked);
            Assert.AreEqual(null, viewModel.P);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            viewModel.CActivate = false;
            NavigationHelper.CanActivate(view, "p2", t => { success = t; }); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.P);
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            NavigationHelper.CanActivate(view, "p3", t => { success = t; }); // true
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
            NavigationHelper.CanActivate(viewModel, "p", t => { success = t; }); // false
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p", viewModel.P);
            Assert.AreEqual(false, success);

            viewModel.Reset();
            NavigationHelper.CanActivate(viewModel, "p2", t => { success = t; }); // true
            Assert.AreEqual(true, viewModel.IsCanActivateInvoked);
            Assert.AreEqual("p2", viewModel.P);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void CanActivate_With_No_ViewModel_Not_Throw()
        {
            var view = new MyViewCanActivateWithoutContext();
            bool? success = null;

            view.Reset();
            view.CActivate = false;
            NavigationHelper.CanActivate(view, "p", t => { success = t; }); // false
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.P);
            Assert.AreEqual(false, success);

            view.Reset();
            NavigationHelper.CanActivate(view, "p2", t => { success = t; }); // true
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
            NavigationHelper.CanDeactivate(view, t => { success = t; }); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(false, viewModel.IsCanDeactivateInvoked);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            viewModel.CDeactivate = false;
            NavigationHelper.CanDeactivate(view, t => { success = t; }); // false
            Assert.AreEqual(true, view.IsCanDeactivateInvoked);
            Assert.AreEqual(true, viewModel.IsCanDeactivateInvoked);
            Assert.AreEqual(false, success);

            view.Reset();
            viewModel.Reset();
            success = null;
            NavigationHelper.CanDeactivate(view, t => { success = t; }); ; // true
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
        public void FindSelectable_With_Navigate()
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

            NavigationHelper.CheckGuardsAndNavigate(current, sources, typeof(MySelectableViewModel), 2, setCurrent, (t) => { });

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
        public void ProcessNavigate_With_ViewModel()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });

            bool? success = null;

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;

            NavigationHelper.CheckCanActivateAndNavigate(current, typeof(MyViewModelNavigationAwareAndGuardsStatic), "p", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            NavigationHelper.CheckCanActivateAndNavigate(current, typeof(MyViewModelNavigationAwareAndGuardsStatic), "p2", setCurrent, (t) => { success = t; });
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
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void ProcessNavigate_With_Selectable()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });
            bool? success = null;

            var selectable = new MySelectableViewModelWithGuards();
            selectable.CActivate = false;

            NavigationHelper.CheckCanActivateAndNavigateWithSelectable(current, selectable, "p", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, selectable.IsCanActivateInvoked);
            Assert.AreEqual("p", selectable.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            selectable.CActivate = true;
            NavigationHelper.CheckCanActivateAndNavigateWithSelectable(current, selectable, "p2", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, selectable.IsCanActivateInvoked);
            Assert.AreEqual("p2", selectable.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, selectable.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, selectable.POnNavigatingTo);
            Assert.AreEqual(false, selectable.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, selectable.POnNavigatedTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(selectable, setCurrentObject);
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void ProcessNavigate_With_Selectable_View()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });
            bool? success = null;
            var view = new MyViewGuardedWithSelectableViewModelWithGuards();
            MySelectableViewModelWithGuardsStatic.Reset();
            view.CActivate = false;

            NavigationHelper.CheckCanActivateAndNavigateWithSelectable(current, view, "p", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p", view.PCanActivate);
            Assert.AreEqual(false, MySelectableViewModelWithGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MySelectableViewModelWithGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            MySelectableViewModelWithGuardsStatic.Reset();
            success = null;
            view.CActivate = true;
            MySelectableViewModelWithGuardsStatic.CActivate = false;
            NavigationHelper.CheckCanActivateAndNavigateWithSelectable(current, view, "p2", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, view.IsCanActivateInvoked);
            Assert.AreEqual("p2", view.PCanActivate);
            Assert.AreEqual(true, MySelectableViewModelWithGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MySelectableViewModelWithGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            MySelectableViewModelWithGuardsStatic.Reset();
            view.CActivate = true;
            success = null;
            NavigationHelper.CheckCanActivateAndNavigateWithSelectable(current, view, "p3", setCurrent, (t) => { success = t; });
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
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void ProcessNavigate_With_View()
        {
            var current = new MyViewModelNavigationAware();
            bool isSetCurrentInvoked = false;
            object setCurrentObject = null;
            var setCurrent = new Action<object>((source) =>
            {
                isSetCurrentInvoked = true;
                setCurrentObject = source;
            });
            bool? success = null;

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CActivate = false;
            NavigationHelper.CheckCanActivateAndNavigate(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            success = null;
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;
            NavigationHelper.CheckCanActivateAndNavigate(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p2", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p2", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            success = null;
            NavigationHelper.CheckCanActivateAndNavigate(current, typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p3", setCurrent, (t) => { success = t; });
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
            Assert.AreEqual(true, success);
        }

        [TestMethod]
        public void Process_Side_Navigation_With_ViewModel()
        {
            var current = new MyViewModelNavigationAwareAndGuards();
            bool isSetCurrentInvoked = false;
            var setCurrent = new Action(() =>
            {
                isSetCurrentInvoked = true;
            });
            bool? success = null;
            var toGo = new MyViewModelNavigationAwareAndGuards();
            current.CDeactivate = false;

            NavigationHelper.Replace(current, toGo, "p", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(false, toGo.IsCanActivateInvoked);
            Assert.AreEqual(null, toGo.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            current.Reset();
            toGo.Reset();
            current.CDeactivate = true;
            toGo.CActivate = false;
            success = null;
            NavigationHelper.Replace(current, toGo, "p2", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(true, toGo.IsCanActivateInvoked);
            Assert.AreEqual("p2", toGo.PCanActivate);
            Assert.AreEqual(false, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(false, success);

            current.Reset();
            toGo.Reset();
            current.CDeactivate = true;
            toGo.CActivate = true;
            success = null;
            NavigationHelper.Replace(current, toGo, "p3", setCurrent, (t) => { success = t; });
            Assert.AreEqual(true, current.IsCanDeactivateInvoked);
            Assert.AreEqual(true, toGo.IsCanActivateInvoked);
            Assert.AreEqual("p3", toGo.PCanActivate);
            Assert.AreEqual(true, current.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, toGo.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", toGo.POnNavigatingTo);
            Assert.AreEqual(true, toGo.IsOnNavigatedToInvoked);
            Assert.AreEqual("p3", toGo.POnNavigatedTo);
            Assert.AreEqual(true, isSetCurrentInvoked);
            Assert.AreEqual(true, success);
        }
    }

    public class MySelectableViewModel : ISelectable, IMyViewModel
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            this.IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            this.IsCanActivateInvoked = true;
            P = parameter;
            c(CActivate);
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            this.IsCanActivateInvoked = true;
            P = parameter;
            c(CActivate);
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

        public void CanDeactivate(Action<bool> c)
        {
            c(CDeactivate);
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

        public void CanActivate(object parameter, Action<bool> c)
        {
            this.IsCanActivateInvoked = true;
            P = parameter;
            c(CActivate);
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

        public void CanDeactivate(Action<bool> c)
        {
            this.IsCanDeactivateInvoked = true;
            c(CDeactivate);
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

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
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
