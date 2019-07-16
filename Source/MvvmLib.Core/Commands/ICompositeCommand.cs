using System.Collections.Generic;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The <see cref="CompositeCommand"/> interface.
    /// </summary>
    public interface ICompositeCommand : ICommand
    {
        /// <summary>
        /// The list of the commands to execute.
        /// </summary>
        IReadOnlyList<ICommand> Commands { get; }

        /// <summary>
        /// Adds a new command to the composite command.
        /// </summary>
        /// <param name="command">The command</param>
        void Add(ICommand command);

        /// <summary>
        /// Removes the command from the commands list.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>True if removed</returns>
        bool Remove(ICommand command);
    }
}
