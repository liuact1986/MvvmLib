using MvvmLib.Logger;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Navigation
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
        /// Configures the service locator (Microsoft Common Service Locator) if used.
        /// </summary>
        protected virtual void ConfigureServiceLocator()
        {
           
        }

        /// <summary>
        /// Creates the Main Page of the application.
        /// </summary>
        /// <returns>The Main Page</returns>
        protected abstract Page CreateShell();

        /// <summary>
        /// Configures the navigation with the main frame of the application.
        /// </summary>
        /// <param name="shell">The Main Page</param>
        protected abstract void ConfigureNavigation(Page shell);

        /// <summary>
        /// Sets the View Model Factory.
        /// </summary>
        protected virtual void SetViewModelFactory()
        {

        }

        /// <summary>
        /// Sets the content of the Current Window or navigates with the frame/ navigation service.
        /// </summary>
        /// <param name="shell">The Main Page of the application</param>
        protected abstract void InitializeShell(Page shell);

        /// <summary>
        /// Activates the Main Page.
        /// </summary>
        /// <param name="shell"></param>
        protected virtual void ActivateShell(Page shell)
        {
            Window.Current.Activate();
        }


        /// <summary>
        /// Called after the bootstrapper process ends.
        /// </summary>
        protected virtual void OnComplete()
        {

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

            this.logger.Log("Registering required types.", Category.Debug, Priority.Low);
            RegisterRequiredTypes();

            this.logger.Log("Registering types.", Category.Debug, Priority.Low);
            RegisterTypes();

            this.logger.Log("Configuring the service locator.", Category.Debug, Priority.Low);
            ConfigureServiceLocator();

            this.logger.Log("Setting the View Model Factory.", Category.Debug, Priority.Low);
            SetViewModelFactory();

            this.logger.Log("Creating the shell.", Category.Debug, Priority.Low);
            var shell = CreateShell();

            this.logger.Log("Configuring the navigation.", Category.Debug, Priority.Low);
            ConfigureNavigation(shell);

            this.logger.Log("Initializing the shell.", Category.Debug, Priority.Low);
            InitializeShell(shell);

            this.logger.Log("Activating the shell.", Category.Debug, Priority.Low);
            ActivateShell(shell);

            this.logger.Log("Calling onComplete.", Category.Debug, Priority.Low);
            OnComplete();

            this.logger.Log("Bootstrapper process completed successfully.", Category.Debug, Priority.Low);
        }
    }

}
