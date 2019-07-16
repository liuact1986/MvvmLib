﻿using System;

namespace MvvmLib.Commands
{

    /// <summary>
    /// A command with a generic parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> executeMethod;
        private Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Creates the <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method used to check if <see cref="Execute(object)"/> can be invoked</param>
        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
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
        public RelayCommand(Action<T> executeMethod)
           : this(executeMethod, p => true)
        { }

        /// <summary>
        /// Checks if <see cref="Execute(object)"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            var canExecute = canExecuteMethod((T)parameter);
            return canExecute;
        }

        /// <summary>
        /// Invokes the <see cref="executeMethod"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            executeMethod((T)parameter);
        }
    }
}
