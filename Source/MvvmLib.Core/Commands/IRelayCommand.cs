using System;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// Contract for relay commands.
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// Notify that the can execute method have to be executed.
        /// </summary>
        void RaiseCanExecuteChanged();
    }

}
