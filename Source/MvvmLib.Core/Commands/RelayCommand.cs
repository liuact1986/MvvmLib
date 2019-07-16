using System;

namespace MvvmLib.Commands
{
    /// <summary>
    /// A command without parameter.
    /// </summary>
    public class RelayCommand : CommandBase
    {
        private readonly Action executeMethod;
        private Func<bool> canExecuteMethod;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method used to check if <see cref="Execute(object)"/> can be invoked</param>
        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
            if (canExecuteMethod == null)
                throw new ArgumentNullException(nameof(canExecuteMethod));

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="executeMethod">The method to execute</param>
        public RelayCommand(Action executeMethod)
           : this(executeMethod, () => true)
        { }

        /// <summary>
        /// Checks if <see cref="Execute(object)"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter is not used</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteMethod();
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="executeMethod"/>.
        /// </summary>
        /// <param name="parameter">The parameter is not used</param>
        public override void Execute(object parameter)
        {
            executeMethod();
        }
    }
}
