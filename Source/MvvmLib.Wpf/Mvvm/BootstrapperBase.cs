using MvvmLib.Logger;
using System;
using System.Windows;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// The Bootstrapper base class.
    /// </summary>
    public abstract class BootstrapperBase
    {
        /// <summary>
        /// The bootstrapper logger.
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// Creates the bootstrapper logger (DebugLogger by default)
        /// </summary>
        /// <returns>The logger to use</returns>
        protected virtual ILogger CreateLogger()
        {
            return new DebugLogger();
        }

        /// <summary>
        /// Registers library services.
        /// </summary>
        protected abstract void RegisterRequiredTypes();

        /// <summary>
        /// Registers application dependencies.
        /// </summary>
        protected abstract void RegisterTypes();

        /// <summary>
        /// Creates the Main Window of the Application.
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateShell();

        /// <summary>
        /// Configures the service locator (Microsoft Common Service Locator) if used.
        /// </summary>
        protected virtual void ConfigureServiceLocator()
        {
            // ServiceLocator.SetLocatorProvider(() => new MvvmLibServiceLocatorAdapter(injector));
        }


        /// <summary>
        /// Used to configure the IoC Container.
        /// </summary>
        protected virtual void ConfigureContainer()
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
        /// Registers custom region adapters.
        /// </summary>
        protected virtual void RegisterCustomRegionAdapters()
        {

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
        /// Runs the bootstrapper.
        /// </summary>
        public void Run()
        {
            this.logger = CreateLogger();

            if (this.logger == null)
            {
                throw new InvalidOperationException("The logger cannot be null.");
            }

            this.logger.Log("Starting bootstrapper process.", Category.Debug, Priority.Low);

            this.logger.Log("Configuring the container.", Category.Debug, Priority.Low);
            ConfigureContainer();

            this.logger.Log("Registering required types.", Category.Debug, Priority.Low);
            RegisterRequiredTypes();

            this.logger.Log("Registering types.", Category.Debug, Priority.Low);
            RegisterTypes();

            this.logger.Log("Setting the View Factory.", Category.Debug, Priority.Low);
            SetViewFactory();

            this.logger.Log("Setting the View Model Factory.", Category.Debug, Priority.Low);
            SetViewModelFactory();

            this.logger.Log("Configuring the service locator.", Category.Debug, Priority.Low);
            ConfigureServiceLocator();

            this.logger.Log("Registering custom region adapters.", Category.Debug, Priority.Low);
            RegisterCustomRegionAdapters();

            this.logger.Log("Creates the shell.", Category.Debug, Priority.Low);
            var shell = CreateShell();
            if (shell != null)
            {
                this.logger.Log("Initializes the shell.", Category.Debug, Priority.Low);
                InitializeShell(shell);
            }

            this.logger.Log("Calling onInitialized.", Category.Debug, Priority.Low);
            OnInitialized();

            this.logger.Log("Bootstrapper process completed successfully.", Category.Debug, Priority.Low);
        }
    }

}
