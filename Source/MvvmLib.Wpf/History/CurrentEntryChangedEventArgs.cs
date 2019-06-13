using System;

namespace MvvmLib.History
{
    /// <summary>
    /// The current entry changed event args class.
    /// </summary>
    public class CurrentEntryChangedEventArgs : EventArgs
    {
        private readonly NavigationEntry currentEntry;
        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public NavigationEntry CurrentEntry
        {
            get { return currentEntry; }
        }

        /// <summary>
        /// Create the <see cref="CurrentEntryChangedEventArgs"/>.
        /// </summary>
        /// <param name="currentEntry">The entry</param>
        public CurrentEntryChangedEventArgs(NavigationEntry currentEntry)
        {
            this.currentEntry = currentEntry;
        }
    }
}
