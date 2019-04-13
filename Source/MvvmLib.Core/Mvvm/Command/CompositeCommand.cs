using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// The composite can executes multiple commands.
    /// </summary>
    public class CompositeCommand : RelayCommandBase
    {
        /// <summary>
        /// The list of the commands to be executed.
        /// </summary>
        protected List<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// Creates a composite command and add commands.
        /// </summary>
        public CompositeCommand()
        { }

        /// <summary>
        /// Creates a composite command and add commands.
        /// </summary>
        /// <param name="commands">The commands to add</param>
        public CompositeCommand(params ICommand[] commands)
        {
            this.AddRange(commands);
        }


        /// <summary>
        /// Add a the commands to the composite command.
        /// </summary>
        /// <param name="commands">The commands to add</param>
        public void AddRange(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
            {
                this.commands.Add(command);
            }
        }

        /// <summary>
        /// Adds a new command to the composite command.
        /// </summary>
        /// <param name="command">The command</param>
        public void Add(ICommand command)
        {
            this.commands.Add(command);
        }

        /// <summary>
        /// Checks if commands have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if all commands can execute</returns>
        public override bool CanExecute(object parameter)
        {
            foreach (var command in commands)
            {
                if (!command.CanExecute(parameter))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Execute all commands of composite command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void Execute(object parameter)
        {
            foreach (var command in commands)
            {
                command.Execute(parameter);
            }
        }
    }


}
