using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using MvvmLib.Wpf.Tests.View;
using MvvmLib.Wpf.Tests.ViewModel;
using MvvmLib.Wpf.Tests.ViewModels;
using MvvmLib.Wpf.Tests.Views;
using MyTestLibrary.View;
using MyTestLibrary.ViewModel;
using MyTestLibrary.ViewModels;
using MyTestLibrary.Views;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.Wpf.Tests.ViewModelResolution
{
    [TestClass]
    public class ViewModelLocationProviderTests
    {
        [TestMethod]
        public void Resolve_ViewModelType_With_Default_Convention()
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(typeof(MyViewA));

            Assert.AreEqual(typeof(MyViewAViewModel), viewModelType);
        }

        [TestMethod]
        public void Change_The_Convention_And_Resolve()
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(typeof(MyViewB));
            Assert.AreEqual(null, viewModelType);

            ViewModelLocationProvider.ChangeConvention((viewType) =>
            {
                // mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                // var viewAssemblyName = viewType.GetType().Assembly.FullName; X ERROR

                // MvvmLib.Wpf.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                // MvvmLib.Wpf.Tests.View.MyViewB
                var viewName = viewType.FullName;

                // MvvmLib.Wpf.Tests.ViewModel.MyViewB
                viewName = viewName.Replace(".View.", ".ViewModel.");

                // ViewModel
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";

                // MvvmLib.Wpf.Tests.ViewModel.MyViewBViewModel, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

                var viewModelTypeFound = Type.GetType(viewModelName);
                return viewModelTypeFound;
            });

            var viewModelType2 = ViewModelLocationProvider.ResolveViewModelType(typeof(MyViewB));
            Assert.AreEqual(typeof(MyViewBViewModel), viewModelType2);

            ViewModelLocationProvider.ResetConvention();
        }

        [TestMethod]
        public void Resolve_ViewModelType_With_Default_Convention_From_Another_Assembly()
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(typeof(MyLibViewA));

            Assert.AreEqual(typeof(MyLibViewAViewModel), viewModelType);
        }

        [TestMethod]
        public void Resolve_ViewModelType_With_Default_Convention_From_Another_Assembly_With_Custom_Convention()
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(typeof(MyLibViewB));
            Assert.AreEqual(null, viewModelType);

            ViewModelLocationProvider.ChangeConvention((viewType) =>
            {
                // MvvmLib.Wpf.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                // MvvmLib.Wpf.Tests.View.MyViewB
                var viewName = viewType.FullName;

                // MvvmLib.Wpf.Tests.ViewModel.MyViewB
                viewName = viewName.Replace(".View.", ".ViewModel.");

                // ViewModel
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";

                // MvvmLib.Wpf.Tests.ViewModel.MyViewBViewModel, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

                var viewModelTypeFound = Type.GetType(viewModelName);
                return viewModelTypeFound;
            });

            var viewModelType2 = ViewModelLocationProvider.ResolveViewModelType(typeof(MyLibViewB));
            Assert.AreEqual(typeof(MyLibViewBViewModel), viewModelType2);

            ViewModelLocationProvider.ResetConvention();
        }

        [TestMethod]
        public void Creates_The_ViewModel_Instance_With_ViewModelType()
        {
            var viewModel = ViewModelLocationProvider.CreateViewModelInstance(typeof(MyViewAViewModel));

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(typeof(MyViewAViewModel), viewModel.GetType());
        }

        [TestMethod]
        public void Creates_The_ViewModel_Instance_With_A_Custom_Factory()
        {
            ViewModelLocationProvider.SetViewModelFactory((viewModelType) => ReflectionDelegateFactory.CreateConstructor<object>(viewModelType)());

            var viewModel = ViewModelLocationProvider.CreateViewModelInstance(typeof(MyViewAViewModel));

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(typeof(MyViewAViewModel), viewModel.GetType());

            ViewModelLocationProvider.ResetViewModelFactory();
        }
    }

    public class ReflectionDelegateFactory
    {
        public static Func<T> CreateConstructor<T>(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            return () => (T)constructor.Invoke(null);
        }
    }
}
