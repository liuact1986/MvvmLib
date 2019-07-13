using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// An Async command with parameter.
    /// </summary>
    /// <typeparam name="T">The parameter type</typeparam>
    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        private Func<T, Task> executeAsync;
        private Func<T, bool> canExecute;
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
        /// The cancellation toekn source.
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
        /// Creates the <see cref="AsyncCommand{T}"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="executeAsync"/> can be invoked</param>
        /// <param name="onException">The action invoked on error</param>
        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute, Action<Exception> onException)
        {
            if (executeAsync == null)
                throw new ArgumentNullException(nameof(executeAsync));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));

            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
            this.onException = onException;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancelCommand = new RelayCommand(Cancel);
        }


        /// <summary>
        /// Creates the <see cref="AsyncCommand{T}"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="canExecute">The method used to check if the <see cref="executeAsync"/> can be invoked</param>
        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute)
          : this(executeAsync, canExecute, null)
        { }

        /// <summary>
        /// Creates the <see cref="AsyncCommand{T}"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        /// <param name="onException">The action invoked on error</param>
        public AsyncCommand(Func<T, Task> executeAsync, Action<Exception> onException)
            : this(executeAsync, (t) => true, onException)
        { }

        /// <summary>
        /// Creates the <see cref="AsyncCommand{T}"/>.
        /// </summary>
        /// <param name="executeAsync">The method to execute</param>
        public AsyncCommand(Func<T, Task> executeAsync)
            : this(executeAsync, (t) => true, null)
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
            var result = !isExecuting && canExecute((T)parameter);
            return result;
        }

        /// <summary>
        /// Invokes the <see cref="executeAsync"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public void Execute(object parameter)
        {
            this.executeTask = ExecuteAsync((T)parameter);
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                this.isExecuting = true;
                await executeAsync(parameter);
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
