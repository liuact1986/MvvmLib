using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmLib.Commands
{

    /// <summary>
    /// A command that can execute multiples commands simultaneously.
    /// </summary>
    public class CompositeCommand : ICompositeCommand
    {
        private readonly List<ICommand> commands;
        /// <summary>
        /// The list of the commands to execute.
        /// </summary>
        public IReadOnlyList<ICommand> Commands
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
        /// Removes the command from the commands list.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>True if removed</returns>
        public virtual bool Remove(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (commands.Contains(command))
            {
                command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                var removed = commands.Remove(command);
                return removed;
            }
            return false;
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Use the canExecuteMethod for all commands.
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
        /// Invokes the execute method for all <see cref="Commands"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public virtual void Execute(object parameter)
        {
            foreach (var command in commands)
                command.Execute(parameter);
        }

    }
}
