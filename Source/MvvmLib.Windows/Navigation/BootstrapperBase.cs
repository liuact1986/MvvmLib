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
        protected ILogger logger;

        /// <summary>
        /// Creates the bootstrapper base class.
        /// </summary>
        /// <param name="logger">The logger</param>
        public BootstrapperBase(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Creates the bootstrapper base class.
        /// </summary>
        public BootstrapperBase()
            : this(new DebugLogger())
        { }

        /// <summary>
        /// Registers library services.
        /// </summary>
        protected abstract void RegisterRequiredTypes();

        /// <summary>
        /// Registers application dependencies.
        /// </summary>
        protected abstract void RegisterTypes();


        /// <summary>
        /// Creates the Main Page of the application.
        /// </summary>
        /// <returns></returns>
        protected abstract Page CreateShell();

        protected abstract void ConfigureNavigation(Page shell);

        protected virtual void SetViewModelFactory()
        {

        }

        /// <summary>
        /// Sets the content of the Current Window or navigates with the frame/ navigation service.
        /// </summary>
        /// <param name="shell">The Main Page of the application</param>
        protected abstract void InitializeShell(Page shell);

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
            if(this.logger == null)
            {
                throw new InvalidOperationException("The logger cannot be null.");
            }

            this.logger.Log("Starting bootstrapper process.", Category.Debug, Priority.Low);

            this.logger.Log("Registering required types.", Category.Debug, Priority.Low);
            RegisterRequiredTypes();

            this.logger.Log("Registering types.", Category.Debug, Priority.Low);
            RegisterTypes();

            this.logger.Log("Setting ViewModelFactory.", Category.Debug, Priority.Low);
            SetViewModelFactory();

            this.logger.Log("Creating the shell.", Category.Debug, Priority.Low);
            var shell = CreateShell();

            this.logger.Log("Configuring the navigation.", Category.Debug, Priority.Low);
            ConfigureNavigation(shell);

            this.logger.Log("Initalizing the shell.", Category.Debug, Priority.Low);
            InitializeShell(shell);


            this.logger.Log("Activating the shell.", Category.Debug, Priority.Low);
            ActivateShell(shell);

            this.logger.Log("Calling onComplete.", Category.Debug, Priority.Low);
            OnComplete();

            this.logger.Log("Bootstrapper process completed succesfully.", Category.Debug, Priority.Low);
        }
    }

}
