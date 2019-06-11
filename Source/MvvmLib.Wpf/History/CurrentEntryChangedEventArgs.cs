using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The current navigation event args class.
    /// </summary>
    public class CurrentEntryChangedEventArgs : EventArgs
    {
        private NavigationEntry currentEntry;
        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public NavigationEntry CurrentEntry
        {
            get { return currentEntry; }
        }

        /// <summary>
        /// Create the navigation entry event args.
        /// </summary>
        /// <param name="currentEntry">The entry</param>
        public CurrentEntryChangedEventArgs(NavigationEntry currentEntry)
        {
            this.currentEntry = currentEntry;
        }
    }

    /// <summary>
    /// The event class used on current changed for navigation sources.
    /// </summary>
    public class CurrentSourceChangedEventArgs : EventArgs
    {
        private object current;
        /// <summary>
        /// Gets the current source.
        /// </summary>
        public object Current
        {
            get { return current; }
        }

        /// <summary>
        /// Create the current source changed event args class.
        /// </summary>
        /// <param name="current">The entry</param>
        public CurrentSourceChangedEventArgs(object current)
        {
            this.current = current;
        }
    }
}
