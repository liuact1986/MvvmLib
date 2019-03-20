using System;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Implementation of ICommand
    /// </summary>
    /// <typeparam name="T">The generic type</typeparam>
    public class RelayCommand<T> : RelayCommandBase
    {
        protected Action<T> callback;
        protected Func<T, bool> condition;

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
