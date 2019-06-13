using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System.Windows;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class BootstrapperTests
    {
        [TestMethod]
        public void Run_Bootsrapper_Works()
        {
            var bootstrapper = new MyBootstrapper();
            bootstrapper.Run();

            Assert.AreEqual(true, bootstrapper.IsRegisterRequiredTypesInvoked);
            Assert.AreEqual(true, bootstrapper.IsRegisterTypesInvoked);
            Assert.AreEqual(true, bootstrapper.IsCreateShellInvoked);
            Assert.AreEqual(true, bootstrapper.IsConfigureServiceLocatorInvoked);
            Assert.AreEqual(true, bootstrapper.IsInitializeShellInvoked);
            Assert.AreEqual(true, bootstrapper.IsOnInitalizedInvoked);
            Assert.AreEqual(true, bootstrapper.IsPreloadApplicationDataInvoked);
            Assert.AreEqual(true, bootstrapper.IsSetViewFactoryInvoked);
            Assert.AreEqual(true, bootstrapper.IsSetViewModelFactoryInvoked);
            Assert.AreEqual(true, bootstrapper.IsCreateShellViewModelInvoked);
            Assert.AreEqual(true, MyWindow.IsCreated);
            Assert.AreEqual(true, MyShellViewModel.IsCreated);
        }
    }

    public class MyWindow : Window
    {
        public static bool IsCreated { get; set; }

        public MyWindow()
        {
            IsCreated = true;
        }
    }

    public class MyShellViewModel
    {
        public static bool IsCreated { get; set; }

        public MyShellViewModel()
        {
            IsCreated = true;
        }
    }

    public class MyBootstrapper : BootstrapperBase
    {
        public bool IsRegisterRequiredTypesInvoked { get; private set; }
        public bool IsRegisterTypesInvoked { get; private set; }
        public bool IsCreateShellInvoked { get; private set; }
        public bool IsConfigureServiceLocatorInvoked { get; private set; }
        public bool IsInitializeShellInvoked { get; private set; }
        public bool IsOnInitalizedInvoked { get; private set; }
        public bool IsPreloadApplicationDataInvoked { get; private set; }
        public bool IsSetViewFactoryInvoked { get; private set; }
        public bool IsSetViewModelFactoryInvoked { get; private set; }
        public bool IsCreateShellViewModelInvoked { get; private set; }

        protected override Window CreateShell()
        {
            this.IsCreateShellInvoked = true;
            return new MyWindow();
        }

        protected override void RegisterRequiredTypes()
        {
            this.IsRegisterRequiredTypesInvoked = true;
        }

        protected override void RegisterTypes()
        {
            this.IsRegisterTypesInvoked = true;
        }

        protected override void InitializeShell(Window shell)
        {
            this.IsInitializeShellInvoked = true;
            base.InitializeShell(shell);
        }

        protected override void ConfigureServiceLocator()
        {
            this.IsConfigureServiceLocatorInvoked = true;
            base.ConfigureServiceLocator();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.IsOnInitalizedInvoked = true;
        }

        protected override void PreloadApplicationData()
        {
            base.PreloadApplicationData();
            this.IsPreloadApplicationDataInvoked = true;
        }

        protected override void SetViewFactory()
        {
            base.SetViewFactory();
            this.IsSetViewFactoryInvoked = true;
        }

        protected override void SetViewModelFactory()
        {
            base.SetViewModelFactory();
            this.IsSetViewModelFactoryInvoked = true;
        }

        protected override object CreateShellViewModel()
        {
            IsCreateShellViewModelInvoked = true;
            return new MyShellViewModel();
        }
    }

}
