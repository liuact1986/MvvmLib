using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The can move to event args class.
    /// </summary>
    public class CanMoveToEventArgs : EventArgs
    {
        private readonly bool canMoveTo;
        /// <summary>
        /// Checks if can move.
        /// </summary>
        public bool CanMoveTo
        {
            get { return canMoveTo; }
        }

        /// <summary>
        /// Creates the <see cref="CanMoveToEventArgs"/>.
        /// </summary>
        /// <param name="canMoveTo">Checks if can move</param>
        public CanMoveToEventArgs(bool canMoveTo)
        {
            this.canMoveTo = canMoveTo;
        }
    }
}
