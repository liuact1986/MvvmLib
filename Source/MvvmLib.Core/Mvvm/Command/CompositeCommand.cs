using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// The composite can executes multiple commands.
    /// </summary>
    public class CompositeCommand : ICommand
    {
        /// <summary>
        /// The list of the commands to execute.
        /// </summary>
        protected readonly IList<ICommand> commands;
        /// <summary>
        /// The list of the commands to execute.
        /// </summary>
        public IList<ICommand> Commands
        {
            get { return commands; }
        }

        /// <summary>
        /// Can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a composite command and add commands.
        /// </summary>
        public CompositeCommand()
        {
            this.commands = new List<ICommand>();
        }

        /// <summary>
        /// Adds a new command to the composite command.
        /// </summary>
        /// <param name="command">The command</param>
        public virtual void Add(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (commands.Contains(command))
                throw new ArgumentException("Command already registered");

            commands.Add(command);
            command.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        /// <summary>
        /// Remove the command from commands list.
        /// </summary>
        /// <param name="command">The command</param>
        public virtual bool Remove(ICommand command)
        {
            if (command == null) { throw new ArgumentNullException(nameof(command)); }

            if (commands.Contains(command))
            {
                command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                var removed = commands.Remove(command);
                return removed;
            }
            return false;
        }

        /// <summary>
        /// Allows the composite command to be notified on command can execute changed.
        /// </summary>
        /// <param name="sender">The command</param>
        /// <param name="e">The event args</param>
        protected virtual void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks if commands have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if all commands can execute</returns>
        public virtual bool CanExecute(object parameter)
        {
            foreach (var command in commands)
            {
                if (!command.CanExecute(parameter))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Invokes the execute command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public virtual void Execute(object parameter)
        {
            foreach (var command in commands)
                command.Execute(parameter);
        }

    }
}
