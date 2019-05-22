using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The region navigation failed event args class.
    /// </summary>
    public class RegionNavigationFailedEventArgs : EventArgs
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
        public RegionNavigationFailedEventArgs(NavigationFailedException exception)
        {
            this.exception = exception;
        }
    }

}