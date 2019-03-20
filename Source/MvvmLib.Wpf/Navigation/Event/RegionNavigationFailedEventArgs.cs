using System;

namespace MvvmLib.Navigation
{
    public class RegionNavigationFailedEventArgs : EventArgs
    {
        public object Source { get; }
        public object Parameter { get; }

        public RegionNavigationFailedEventArgs(object source, object parameter)
        {
            this.Source = source;
            this.Parameter = parameter;
        }
    }

}