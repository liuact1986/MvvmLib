using MvvmLib.Logger;
using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Bootstrapper base class.
    /// </summary>
    public abstract class BootstrapperBase
    {
        private object shellViewModel;

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// Creates the logger used by the bootstrapper.
        /// </summary>
        /// <returns>The logger</returns>
        protected virtual ILogger CreateLogger()
        {
            return new DebugLogger();
        }

        /// <summary>
        /// Registers the required types.
        /// </summary>
        protected abstract void RegisterRequiredTypes();

        /// <summary>
        /// Registers the application dependencies.
        /// </summary>
        protected abstract void RegisterTypes();

        /// <summary>
        /// Registers modules.
        /// </summary>
        protected virtual void RegisterModules()
        {

        }

        /// <summary>
        /// Creates the main <see cref="Window"/> of the application.
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateShell();

        /// <summary>
        /// Allows to configure the service locator (Microsoft Common Service Locator).
        /// </summary>
        /// <example>ServiceLocator.SetLocatorProvider(() => new MvvmLibServiceLocatorAdapter(injector));</example>
        protected virtual void ConfigureServiceLocator()
        {

        }

        /// <summary>
        /// Sets the View Factory.
        /// </summary>
        protected virtual void SetViewFactory()
        {

        }

        /// <summary>
        /// Sets the View Model Factory.
        /// </summary>
        protected virtual void SetViewModelFactory()
        {

        }

        /// <summary>
        /// Allows to create the ViewModel and load data before creating the shell.
        /// </summary>
        /// <returns>The ViewModel</returns>
        protected virtual object CreateShellViewModel()
        {
            return null;
        }

        /// <summary>
        /// Sets the Application Current Window to the shell.
        /// </summary>
        /// <param name="shell">The Window created with <see cref="CreateShell"/></param>
        protected virtual void InitializeShell(Window shell)
        {
            if (Application.Current != null)
            {
                Application.Current.MainWindow = shell;
            }
        }

        /// <summary>
        /// Called after the bootstrapper process ends.
        /// </summary>
        protected virtual void OnInitialized()
        {
            if (Application.Current != null
                && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Show();
            }
        }

        /// <summary>
        /// Allows to preload data for application
        /// </summary>
        protected virtual void PreloadApplicationData()
        {

        }

        /// <summary>
        /// Runs the bootstrapper.
        /// </summary>
        public void Run()
        {
            this.logger = CreateLogger();
            if (this.logger == null)
                throw new InvalidOperationException("The logger cannot be null.");

            this.logger.Log("Starting bootstrapper process.", Category.Debug, Priority.Low);

            this.logger.Log("Registering required types.", Category.Debug, Priority.Low);
            RegisterRequiredTypes();

            this.logger.Log("Registering types.", Category.Debug, Priority.Low);
            RegisterTypes();

            this.logger.Log("Registering modules.", Category.Debug, Priority.Low);
            RegisterModules();

            this.logger.Log("Setting the View Factory.", Category.Debug, Priority.Low);
            SetViewFactory();

            this.logger.Log("Setting the View Model Factory.", Category.Debug, Priority.Low);
            SetViewModelFactory();

            this.logger.Log("Configuring the service locator.", Category.Debug, Priority.Low);
            ConfigureServiceLocator();

            this.logger.Log("Preloading application data.", Category.Debug, Priority.Low);
            PreloadApplicationData();

            this.logger.Log("Trying to create the ShellViewModel", Category.Debug, Priority.Low);
            var viewModel = CreateShellViewModel();
            this.shellViewModel = viewModel;

            this.logger.Log("Creating the Shell", Category.Debug, Priority.Low);
            var shell = CreateShell();

            if (viewModel != null)
            {
                shell.DataContext = viewModel;
                shell.Loaded += OnShellLoaded;
            }

            this.logger.Log("Initializing the shell.", Category.Debug, Priority.Low);
            InitializeShell(shell);

            this.logger.Log("Invoking OnInitialized method.", Category.Debug, Priority.Low);
            OnInitialized();

            this.logger.Log("Bootstrapper process completed successfully.", Category.Debug, Priority.Low);
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            var shell = sender as Window;
            shell.Loaded -= OnShellLoaded;

            if (this.shellViewModel is IIsLoaded)
                ((IIsLoaded)this.shellViewModel).OnLoaded();

        }
    }

}
