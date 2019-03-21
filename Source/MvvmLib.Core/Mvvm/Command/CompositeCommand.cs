using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// A command that can execute multiple commands.
    /// </summary>
    public class CompositeCommand : RelayCommandBase
    {
        /// <summary>
        /// The list of the commands to be executed.
        /// </summary>
        protected List<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// Adds a new command to the composite command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The composite command</returns>
        public CompositeCommand Add(ICommand command)
        {
            this.commands.Add(command);
            return this;
        }

        /// <summary>
        /// Checks if the command have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public override bool EvaluateCondition(object parameter)
        {
            foreach (var command in this.commands)
            {
                if (!command.CanExecute(parameter))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Invokes the execute method.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override void InvokeCallback(object parameter)
        {
            foreach (var command in this.commands)
            {
                command.Execute(parameter);
            }
        }
    }
}
