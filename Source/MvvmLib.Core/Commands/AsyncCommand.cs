using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// An Async Command.
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        private Func<Task> executeAsync;
        private Func<bool> canExecute;
        private Action<Exception> onException;
        private Task executeTask;

        private bool isExecuting;
        /// <summary>
        /// Checks if is executing.
        /// </summary>
        public bool IsExecuting
        {
            get { return isExecuting; }
        }

        private CancellationTokenSource cancellationTokenSource;
        /// <summary>
        /// The cancellation token source.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
        {
            get { return cancellationTokenSource; }
            set { cancellationTokenSource = value; }
        }

        /// <summary>
        /// Checks if cancel has been requested.
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                if (cancellationTokenSource == null)
                    return false;

                return cancellationTokenSource.IsCancellationRequested;
            }
        }

        private ICommand cancelCommand;
        /// <summary>
        /// The command used to cancel the task.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
        }

        /// <summary>
        /// Creates the <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="executeAsync"/> can be invoked</param>
        /// <param name="onException">The action invoked on error</param>
        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute, Action<Exception> onException)
        {
            if (executeAsync == null)
                throw new ArgumentNullException(nameof(executeAsync));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));

            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
            this.onException = onException;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancelCommand = new DelegateCommand(Cancel);
        }

        /// <summary>
        /// Creates the <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="executeAsync"/> can be invoked</param>
        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute)
         : this(executeAsync, canExecute, null)
        { }

        /// <summary>
        /// Creates the <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="onException">The action invoked on error</param>
        public AsyncCommand(Func<Task> executeAsync, Action<Exception> onException)
            : this(executeAsync, () => true, onException)
        { }

        /// <summary>
        /// Creates the <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        public AsyncCommand(Func<Task> executeAsync)
            : this(executeAsync, () => true, null)
        { }

        /// <summary>
        /// Invoked on can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Use the <see cref="canExecute"/> to check if the <see cref="executeAsync"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if can execute</returns>
        public bool CanExecute(object parameter)
        {
            var result = !isExecuting && canExecute();
            return result;
        }

        /// <summary>
        /// Invokes the <see cref="executeAsync"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public void Execute(object parameter)
        {
            this.executeTask = ExecuteAsync();
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            if (!CanExecute(null))
                return;

            try
            {
                this.isExecuting = true;
                await executeAsync();
            }
            catch (Exception ex)
            {
                if (onException != null)
                    onException(ex);
            }
            finally
            {
                this.isExecuting = false;
            }
        }

        /// <summary>
        /// Allows to cancel the task.
        /// </summary>
        public void Cancel()
        {
            if (!isExecuting)
                return;

            CancellationTokenSource.Cancel();
        }
    }

}
