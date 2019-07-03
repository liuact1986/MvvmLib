using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The can move to event args class.
    /// </summary>
    public class CanMoveToEventArgs : EventArgs
    {
        private readonly bool value;
        /// <summary>
        /// Checks if can move.
        /// </summary>
        public bool Value
        {
            get { return value; }
        }

        /// <summary>
        /// Creates the <see cref="CanMoveToEventArgs"/>.
        /// </summary>
        /// <param name="value">Checks if can move</param>
        public CanMoveToEventArgs(bool value)
        {
            this.value = value;
        }
    }
}
