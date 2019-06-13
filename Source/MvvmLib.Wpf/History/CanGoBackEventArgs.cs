using System;

namespace MvvmLib.History
{
    /// <summary>
    /// The can go back event args class.
    /// </summary>
    public class CanGoBackEventArgs : EventArgs
    {
        private readonly bool canGoBack;
        /// <summary>
        /// Checks if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return canGoBack; }
        }

        /// <summary>
        /// Creates the can go back event args.
        /// </summary>
        /// <param name="canGoBack">Checks if can go back</param>
        public CanGoBackEventArgs(bool canGoBack)
        {
            this.canGoBack = canGoBack;
        }
    }
}
