using System;

namespace MvvmLib.Commands
{
    /// <summary>
    /// Command with no parameter.
    /// </summary>
    public class RelayCommand : RelayCommandBase
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="execute"/> can be invoked</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeMethod">The action to execute</param>
        public RelayCommand(Action executeMethod)
            : this(executeMethod, () => true)
        { }

        /// <summary>
        /// Use the <see cref="canExecute"/> to check if the <see cref="execute"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = this.canExecute.Invoke();
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="execute"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            execute.Invoke();
        }

    }

}
