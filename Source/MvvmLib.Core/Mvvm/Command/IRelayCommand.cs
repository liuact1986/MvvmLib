using System.Windows.Input;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// The relay command contract.
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// Invokes when the condition have to be checked.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
