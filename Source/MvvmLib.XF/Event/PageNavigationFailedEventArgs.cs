using System;

namespace MvvmLib.Navigation
{
    public class PageNavigationFailedEventArgs : EventArgs
    {
        public object Source { get; }
        public object Parameter { get; }

        public PageNavigationFailedEventArgs(object source, object parameter)
        {
            this.Source = source;
            this.Parameter = parameter;
        }
    }
}
