using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region navigation event args class.
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
}
