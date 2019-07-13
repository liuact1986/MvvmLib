using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The Async Command interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncCommand<T> : ICommand
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
        /// Executes the task.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        Task ExecuteAsync(T parameter);

        /// <summary>
        /// Allows to cancel the task.
        /// </summary>
        void Cancel();
    }
}
