using System;

namespace MvvmLib.Navigation
{
    public class RegionNavigationFailedEventArgs : EventArgs
    {
        public NavigationFailedException Exception { get; }

        public RegionNavigationFailedEventArgs(NavigationFailedException exception)
        {
            this.Exception = exception;
        }
    }

}