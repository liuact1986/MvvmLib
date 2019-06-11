using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    public sealed class NavigationHistory : INavigationHistory
    {
        internal readonly NavigationEntryCollection entries;
        public IReadOnlyCollection<NavigationEntry> Entries
        {
            get { return entries; }
        }

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
        }

        public NavigationEntry Root
        {
            get { return this.currentIndex >= 0 ? this.entries[0] : null; }
        }

        public NavigationEntry Previous
        {
            get { return this.currentIndex > 0 ? this.entries[this.currentIndex - 1] : null; }
        }

        public NavigationEntry Current
        {
            get { return this.currentIndex >= 0 ? this.entries[this.currentIndex] : null; }
        }

        public NavigationEntry Next
        {
            get { return this.currentIndex < this.entries.Count - 1 ? this.entries[this.currentIndex + 1] : null; }
        }

        /// <summary>
        /// Chekcs if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.currentIndex > 0; }
        }

        /// <summary>
        /// Chekcs if can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this.currentIndex < this.entries.Count - 1; }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler<CanGoBackEventArgs> CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler<CanGoForwardEventArgs> CanGoForwardChanged;

        /// <summary>
        /// Invoked on current entry changed.
        /// </summary>
        public event EventHandler<CurrentEntryChangedEventArgs> CurrentChanged;

        public NavigationHistory()
        {
            this.entries = new NavigationEntryCollection();
            this.currentIndex = -1;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCanGobackChanged(bool canGoBack)
        {
            this.CanGoBackChanged?.Invoke(this, new CanGoBackEventArgs(canGoBack));
        }

        private void OnCanGoForwardChanged(bool canGoForward)
        {
            this.CanGoForwardChanged?.Invoke(this, new CanGoForwardEventArgs(canGoForward));
        }

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, new CurrentEntryChangedEventArgs(Current));
        }

        public void Navigate(NavigationEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            int oldIndex = this.currentIndex;

            // insert index + 1
            int newIndex = this.currentIndex + 1;
            this.entries.Insert(newIndex, entry);
            SetCurrent(newIndex);

            // clear forward
            if (this.entries.Count - 1 > this.currentIndex)
            {
                // remove from current index to count
                int removeIndex = this.currentIndex + 1;
                while (entries.Count > removeIndex)
                    this.entries.RemoveAt(removeIndex);
            }

            if (oldIndex >= 0)
                CheckCanGoBack(oldIndex);
            if (this.currentIndex < this.entries.Count - 1)
                OnCanGoForwardChanged(false);
        }

        /// <summary>
        /// Moves to the root entry.
        /// </summary>
        public void NavigateToRoot()
        {
            if (!CanGoBack && currentIndex == -1)
                throw new InvalidOperationException("Cannot process navigate to root. The history is empty");

            SetCurrent(0);
            if (this.entries.Count > 1)
            {
                while (this.entries.Count > 1)
                    this.entries.RemoveAt(1);
            }

            OnCanGobackChanged(false);
            OnCanGoForwardChanged(false);
        }

        internal void SetCurrent(int index)
        {
            this.currentIndex = index;
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged();
        }

        /// <summary>
        /// Moves to the previous entry.
        /// </summary>
        public void GoBack()
        {
            if (!CanGoBack)
                throw new InvalidOperationException("Cannot process go back");

            int oldIndex = this.currentIndex;
            int newIndex = this.currentIndex - 1;

            SetCurrent(newIndex);

            CheckCanGoBack(oldIndex);
            CheckCanGoForward(oldIndex);
        }

        /// <summary>
        /// Moves to the next entry.
        /// </summary>
        public void GoForward()
        {
            if (!CanGoForward)
                throw new InvalidOperationException("Cannot process go forward");

            int oldIndex = this.currentIndex;
            int newIndex = this.currentIndex + 1;

            SetCurrent(newIndex);

            CheckCanGoBack(oldIndex);
            CheckCanGoForward(oldIndex);
        }

        private void CheckCanGoBack(int oldIndex)
        {
            // index 0 => 1
            if (oldIndex == 0 && this.currentIndex == 1)
                OnCanGobackChanged(true);
            // index 1 => 0
            if (oldIndex == 1 && this.currentIndex == 0)
                OnCanGobackChanged(false);
        }

        private void CheckCanGoForward(int oldIndex)
        {
            // count - 1 => count - 2
            if (oldIndex == this.entries.Count - 1 && this.currentIndex == this.entries.Count - 2)
                OnCanGoForwardChanged(true);
            // count - 2 => count -1
            if (oldIndex == this.entries.Count - 2 && this.currentIndex == this.entries.Count - 1)
                OnCanGoForwardChanged(false);
        }

        public void MoveTo(NavigationEntry entry)
        {
            var newIndex = this.entries.IndexOf(entry);
            if (newIndex == -1)
                throw new ArgumentException("Unable to find the index of the entry");

            this.MoveTo(newIndex);
        }

        public void MoveTo(int index)
        {
            if (index < 0 || index > this.entries.Count)
                throw new IndexOutOfRangeException();

            SetCurrent(index);

            if (index > 1)
                OnCanGobackChanged(true);
            else
                OnCanGobackChanged(false);

            if (index < this.entries.Count - 1)
                OnCanGoForwardChanged(true);
            else
                OnCanGoForwardChanged(false);
        }

        public void Clear()
        {
            this.entries.Clear();
            SetCurrent(-1);

            this.OnCanGobackChanged(false);
            this.OnCanGoForwardChanged(false);
        }
    }
}
