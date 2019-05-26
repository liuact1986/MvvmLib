using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The can go forward event args class.
    /// </summary>
    public class CanGoForwardEventArgs : EventArgs
    {
        private bool canGoForward;
        /// <summary>
        /// Checks if can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return canGoForward; }
        }

        /// <summary>
        /// Creates the can go forward event args.
        /// </summary>
        /// <param name="canGoForward">Checks if can go forward</param>
        public CanGoForwardEventArgs(bool canGoForward)
        {
            this.canGoForward = canGoForward;
        }
    }
}
