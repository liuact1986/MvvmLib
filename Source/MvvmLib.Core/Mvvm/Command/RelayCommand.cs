using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// A command with no parameter.
    /// </summary>
    public class RelayCommand : RelayCommandBase
    {
        /// <summary>
        /// The action to execute.
        /// </summary>
        protected Action executeCommand;

        /// <summary>
        /// The function to check if command have to be executed.
        /// </summary>
        protected Func<bool> canExecuteCommand;

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeCommand">The action to execute</param>
        /// <param name="canExecuteCommand">The function to check if command have to be executed</param>
        public RelayCommand(Action executeCommand, Func<bool> canExecuteCommand)
        {
            this.executeCommand = executeCommand ?? throw new ArgumentNullException(nameof(executeCommand));
            this.canExecuteCommand = canExecuteCommand;
        }

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeCommand">The action to execute</param>
        public RelayCommand(Action executeCommand)
            : this(executeCommand, () => true)
        { }

        /// <summary>
        /// Checks if command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteCommand.Invoke();
            return canExecute;
        }

        /// <summary>
        /// Invokes the execute command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            executeCommand.Invoke();
        }

    }

}
