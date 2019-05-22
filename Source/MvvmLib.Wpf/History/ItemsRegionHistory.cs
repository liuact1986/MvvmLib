using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// History for items regions.
    /// </summary>
    public sealed class ItemsRegionHistory : INotifyPropertyChanged
    {
        private readonly NavigationEntryCollection entries;
        /// <summary>
        /// The navigation entry collection.
        /// </summary>
        public NavigationEntryCollection Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public NavigationEntry Current
        {
            get
            {
                // -1, 0 ...
                if (currentIndex < 0)
                    return null;
                else
                    return this.entries[CurrentIndex];
            }
        }

        internal int currentIndex;
        /// <summary>
        /// Gets the current index.
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
        }

        /// <summary>
        /// Notifies that a the current and current index has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on current entry changed.
        /// </summary>
        public event EventHandler<IndexedNavigationEntryEventArgs> CurrentChanged;

        /// <summary>
        /// Creates the list history class.
        /// </summary>
        public ItemsRegionHistory()
        {
            this.entries = new NavigationEntryCollection();
            this.currentIndex = -1;
            HandleCollectionChanged();
        }

        /// <summary>
        /// Handles the collection changed event.
        /// </summary>
        public void HandleCollectionChanged()
        {
            this.entries.CollectionChanged += OnEntriesCollectionChanged;
        }

        /// <summary>
        /// Unhandles the collection changed event.
        /// </summary>
        public void UnhandleCollectionChanged()
        {
            this.entries.CollectionChanged -= OnEntriesCollectionChanged;
        }

        private void OnEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Select(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    int index = e.OldStartingIndex;
                    if(currentIndex == index)
                    {
                        if(entries.Count > currentIndex)
                        {
                            Select(currentIndex); 
                        }
                        else
                        {
                            Select(currentIndex - 1);
                        }
                    }
                    else if(index < currentIndex)
                    {
                        Select(currentIndex -1);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Select(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Select(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Select(-1);
                    break;
            }
        }

        /// <summary>
        /// Sets the current index.
        /// </summary>
        /// <param name="index">The index</param>
        public void Select(int index)
        {
            if (index < -1 || index > entries.Count - 1)
                throw new IndexOutOfRangeException();

            currentIndex = index;
            OnPropertyChanged(nameof(Current));
            OnPropertyChanged(nameof(CurrentIndex));
            OnCurrentChanged(index);
        }

        private void OnCurrentChanged(int index)
        {
            CurrentChanged?.Invoke(this, new IndexedNavigationEntryEventArgs(index, Current));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Adds a navigation entry at the end of the <see cref="NavigationEntryCollection"/>.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        public void Add(NavigationEntry entry)
        {
            this.entries.Add(entry);
        }

        /// <summary>
        /// Inserts a navigation entry at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="entry">The navigation entry</param>
        public void Insert(int index, NavigationEntry entry)
        {
            this.entries.Insert(index, entry);
        }

        /// <summary>
        /// Removes the navigation entry at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveAt(int index)
        {
            this.entries.RemoveAt(index);
        }

        /// <summary>
        /// Removes the entry.
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <returns>True if removed</returns>
        public bool Remove(NavigationEntry entry)
        {
            return this.entries.Remove(entry);
        }

        /// <summary>
        /// Moves the entry from old index to the new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        public void Move(int oldIndex, int newIndex)
        {
            this.entries.Move(oldIndex, newIndex);
        }

        /// <summary>
        /// Clears all entries.
        /// </summary>
        public void Clear()
        {
            this.entries.Clear();
        }
    }
}
