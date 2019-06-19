using System;

namespace MvvmLib.Commands
{

    /// <summary>
    /// Command with parameter.
    /// </summary>
    public class RelayCommand<TParameter> : RelayCommandBase
    {
        private readonly Action<TParameter> execute;
        private readonly Func<TParameter, bool> canExecute;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="execute"/> can be invoked</param>
        public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute)
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
        public RelayCommand(Action<TParameter> executeMethod)
            : this(executeMethod, (t) => true)
        { }

        /// <summary>
        /// Use the <see cref="canExecute"/> to check if the <see cref="execute"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = this.canExecute.Invoke((TParameter)parameter);
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="execute"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            execute.Invoke((TParameter)parameter);
        }

    }

}
