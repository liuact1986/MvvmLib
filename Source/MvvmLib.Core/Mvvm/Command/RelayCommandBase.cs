using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Base class for commands.
    /// </summary>
    public abstract class RelayCommandBase : IRelayCommand
    {
        /// <summary>
        /// Can Execute changed event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Checks if the command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this.EvaluateCondition(parameter);
        }

        /// <summary>
        /// Invokes the execute method.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public void Execute(object parameter)
        {
            this.InvokeCallback(parameter);
        }

        /// <summary>
        /// Invokes <see cref="CanExecute(object)"/>.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks if the command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if the command can be executed</returns>
        public abstract bool EvaluateCondition(object parameter);

        /// <summary>
        /// Invokes the execute method.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void InvokeCallback(object parameter);
    }
}
