using System;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class RelayCommandBase : IRelayCommand
    {
        /// <summary>
        /// Can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Checks if commands have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if all commands can execute</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes the execute command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Notify that the can execute method have to be executed.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
