using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Implementation of ICommand
    /// </summary>
    /// <typeparam name="T">The generic type</typeparam>
    public class RelayCommand<T> : RelayCommandBase
    {
        /// <summary>
        /// The execute method.
        /// </summary>
        protected Action<T> callback;

        /// <summary>
        /// The predicate.
        /// </summary>
        protected Func<T, bool> condition;

        /// <summary>
        /// Creates the relay command.
        /// </summary>
        /// <param name="callback">The execute method</param>
        /// <param name="condition">The predicate</param>
        public RelayCommand(Action<T> callback, Func<T, bool> condition = null)
        {
            this.callback = callback;
            this.condition = condition;
        }

        /// <summary>
        /// Checks if the command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public override bool EvaluateCondition(object parameter)
        {
            return this.condition == null ? true : this.condition((T)parameter);
        }

        /// <summary>
        /// Invokes the execute method.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void InvokeCallback(object parameter)
        {
            this.callback((T)parameter);
        }
    }
}
