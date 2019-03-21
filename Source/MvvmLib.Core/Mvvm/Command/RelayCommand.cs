using System;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Parameter less command (no CommandParameter value passed to callback and condition)
    /// </summary>
    public class RelayCommand : RelayCommandBase
    {
        /// <summary>
        /// The execute method.
        /// </summary>
        protected Action callback;

        /// <summary>
        /// The predicate.
        /// </summary>
        protected Func<bool> condition;

        /// <summary>
        /// Creates the relay command.
        /// </summary>
        /// <param name="callback">The execute method</param>
        /// <param name="condition">The predicate</param>
        public RelayCommand(Action callback, Func<bool> condition = null)
        {
            this.callback = callback;
            this.condition = condition;
        }


        /// <summary>
        /// Checks if the command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if the command can be executed</returns>
        public override bool EvaluateCondition(object parameter)
        {
            return this.condition == null ? true : this.condition();
        }

        /// <summary>
        /// Invokes the execute method.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void InvokeCallback(object parameter)
        {
            this.callback();
        }
    }

}
