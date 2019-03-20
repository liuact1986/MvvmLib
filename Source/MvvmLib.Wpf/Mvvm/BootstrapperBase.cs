using System.Windows;

namespace MvvmLib.Mvvm
{
    public abstract class BootstrapperBase
    {
        protected abstract void RegisterRequiredTypes();

        protected abstract void RegisterTypes();

        protected abstract Window CreateShell();

        protected virtual void ConfigureServiceLocator()
        {
            // ServiceLocator.SetLocatorProvider(() => new MvvmLibServiceLocatorAdapter(injector));
        }

        protected virtual void ConfigureContainer()
        {

        }

        protected virtual void SetViewFactory()
        {

        }

        protected virtual void SetViewModelFactory()
        {

        }

        protected virtual void InitializeShell(Window shell)
        {
            if (Application.Current != null)
            {
                Application.Current.MainWindow = shell;
            }
        }

        protected virtual void RegisterCustomRegionAdapters()
        {

        }

        protected virtual void OnInitialized()
        {
            if (Application.Current != null 
                && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Show();
            }
        }

        public void Run()
        {
            ConfigureServiceLocator();
            ConfigureContainer();
            SetViewFactory();
            SetViewModelFactory();

            RegisterRequiredTypes();
            RegisterTypes();
            RegisterCustomRegionAdapters();

            var shell = CreateShell();
            if (shell != null)
            {
                InitializeShell(shell);
            }
            OnInitialized();
        }
    }

}
