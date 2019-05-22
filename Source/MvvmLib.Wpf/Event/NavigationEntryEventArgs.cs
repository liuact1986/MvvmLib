using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The region navigation event args class.
    /// </summary>
    public class NavigationEntryEventArgs : EventArgs
    {
        private NavigationEntry entry;
        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public NavigationEntry Entry
        {
            get { return entry; }
        }

        /// <summary>
        /// Create the navigation entry event args.
        /// </summary>
        /// <param name="entry">The entry</param>
        public NavigationEntryEventArgs(NavigationEntry entry)
        {
            this.entry = entry;
        }
    }

    /// <summary>
    /// The region navigation event args class.
    /// </summary>
    public class IndexedNavigationEntryEventArgs : NavigationEntryEventArgs
    {
        private readonly int index;
        /// <summary>
        /// The index.
        /// </summary>
        public int Index
        {
            get { return index; }
        }    

        /// <summary>
        /// Create the navigation entry event args.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="entry">The entry</param>
        public IndexedNavigationEntryEventArgs(int index, NavigationEntry entry)
            :base(entry)
        {
            this.index = index;
        }
    }
}