using System;

namespace MvvmLib.Commands
{
    /// <summary>
    /// Command with no parameter.
    /// </summary>
    public class RelayCommand : RelayCommandBase
    {
        private readonly Action executeMethod;
        private readonly Func<bool> canExecuteMethod;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method used to check if the <see cref="executeMethod"/> can be invoked</param>
        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Creates a Relay command.
        /// </summary>
        /// <param name="executeMethod">The action to execute</param>
        public RelayCommand(Action executeMethod)
            : this(executeMethod, () => true)
        { }

        /// <summary>
        /// Use the <see cref="canExecuteMethod"/> to check if the <see cref="executeMethod"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteMethod.Invoke();
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="executeMethod"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            executeMethod.Invoke();
        }

    }

}
