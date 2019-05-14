using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Generic command with parameter.
    /// </summary>
    /// <typeparam name="TParameter">The parameter type</typeparam>
    public class RelayCommand<TParameter> : RelayCommandBase
    {
        /// <summary>
        /// The action to execute.
        /// </summary>
        protected Action<TParameter> executeCommand;

        /// <summary>
        /// The function to check if command have to be executed.
        /// </summary>
        protected Func<TParameter, bool> canExecuteCommand;

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeCommand">The action to execute</param>
        /// <param name="canExecuteCommand">The function to check if command have to be executed</param>
        public RelayCommand(Action<TParameter> executeCommand, Func<TParameter, bool> canExecuteCommand)
        {
            this.executeCommand = executeCommand ?? throw new ArgumentNullException(nameof(executeCommand));
            this.canExecuteCommand = canExecuteCommand;
        }

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeCommand">The action to execute</param>
        public RelayCommand(Action<TParameter> executeCommand)
            : this(executeCommand, (p) => true)
        { }

        /// <summary>
        /// Checks if command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteCommand.Invoke((TParameter)parameter);
            return canExecute;
        }

        /// <summary>
        /// Invokes the execute command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            executeCommand.Invoke((TParameter)parameter);
        }
    }


}
