using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The navigation failed event args class.
    /// </summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        private readonly NavigationFailedException exception;

        /// <summary>
        /// The exception.
        /// </summary>
        public NavigationFailedException Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Creates the region navigation failed event args class.
        /// </summary>
        /// <param name="exception">The exception</param>
        public NavigationFailedEventArgs(NavigationFailedException exception)
        {
            this.exception = exception;
        }
    }

}