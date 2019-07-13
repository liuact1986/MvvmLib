using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The Async Command interface.
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Checks if is executing.
        /// </summary>
        bool IsExecuting { get; }

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Checks if cancel has been requested.
        /// </summary>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// The command used to cancel the task.
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Raises can execute changed.
        /// </summary>
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();

        /// <summary>
        /// Allows to cancel the task.
        /// </summary>
        void Cancel();
    }

}