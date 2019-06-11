using System;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Command with parameter.
    /// </summary>
    public class RelayCommand<TParameter> : RelayCommandBase
    {
        private readonly Action<TParameter> executeMethod;
        private readonly Func<TParameter, bool> canExecuteMethod;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method used to check if the <see cref="executeMethod"/> can be invoked</param>
        public RelayCommand(Action<TParameter> executeMethod, Func<TParameter, bool> canExecuteMethod)
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
        public RelayCommand(Action<TParameter> executeMethod)
            : this(executeMethod, (t) => true)
        { }

        /// <summary>
        /// Use the <see cref="canExecuteMethod"/> to check if the <see cref="executeMethod"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteMethod.Invoke((TParameter)parameter);
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="executeMethod"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            executeMethod.Invoke((TParameter)parameter);
        }

    }

}
