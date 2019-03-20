using System;

namespace MvvmLib.Mvvm
{

    public abstract class RelayCommandBase : IRelayCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.EvaluateCondition(parameter);
        }

        public void Execute(object parameter)
        {
            this.InvokeCallback(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract bool EvaluateCondition(object parameter);
        public abstract void InvokeCallback(object parameter);
    }
}
