using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// History for content regions.
    /// </summary>
    public sealed class ContentRegionHistory : INotifyPropertyChanged
    {
        private readonly NavigationEntryCollection backStack;
        /// <summary>
        /// The back stack.
        /// </summary>
        public NavigationEntryCollection BackStack
        {
            get { return backStack; }
        }

        private readonly NavigationEntryCollection forwardStack;
        /// <summary>
        /// The forward stack.
        /// </summary>
        public NavigationEntryCollection ForwardStack
        {
            get { return forwardStack; }
        }

        /// <summary>
        /// Gets the root entry. 
        /// </summary>
        public NavigationEntry Root
        {
            get
            {
                if (this.backStack.Count > 0)
                    return this.backStack[0];
                else
                    return this.Current;
            }
        }

        /// <summary>
        /// Gets the previous entry.
        /// </summary>
        public NavigationEntry Previous
        {
            get
            {
                if (this.backStack.Count > 0)
                    return this.backStack[this.backStack.Count - 1];

                return null;
            }
        }

        /// <summary>
        /// Gets the next entry.
        /// </summary>
        public NavigationEntry Next
        {
            get
            {
                if (this.forwardStack.Count > 0)
                    return this.forwardStack[this.forwardStack.Count - 1];

                return null;
            }
        }

        private NavigationEntry current;
        /// <summary>
        /// Gets the current entry.
        /// </summary>
        public NavigationEntry Current
        {
            get { return current; }
        }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on current entry changed.
        /// </summary>
        public event EventHandler<NavigationEntryEventArgs> CurrentChanged;

        /// <summary>
        /// Creates the history.
        /// </summary>
        public ContentRegionHistory()
        {
            this.backStack = new NavigationEntryCollection();
            this.forwardStack = new NavigationEntryCollection();

            HandleGoBackChanged();
            HandleGoForwardChanged();
        }

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, new NavigationEntryEventArgs(Current));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles go back changed.
        /// </summary>
        public void HandleGoBackChanged()
        {
            this.backStack.CollectionChanged += OnBackStackCollectionChanged;
        }

        /// <summary>
        /// Unhandles go back changed.
        /// </summary>
        public void UnhandleGoBackChanged()
        {
            this.backStack.CollectionChanged -= OnBackStackCollectionChanged;
        }

        private void OnBackStackCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (this.backStack.Count == 1)
                        OnCanGobackChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.backStack.Count == 0)
                        OnCanGobackChanged();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCanGobackChanged();
                    break;
            }
        }

        /// <summary>
        /// Handles go forward changed.
        /// </summary>
        public void HandleGoForwardChanged()
        {
            this.forwardStack.CollectionChanged += OnForwardStackCollectionChanged;
        }

        /// <summary>
        /// Unhandles go forward changed.
        /// </summary>
        public void UnhandleGoForwardChanged()
        {
            this.forwardStack.CollectionChanged -= OnForwardStackCollectionChanged;
        }

        private void OnForwardStackCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (this.forwardStack.Count == 1)
                        OnCanGoForwardChanged();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.forwardStack.Count == 0)
                        OnCanGoForwardChanged();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCanGoForwardChanged();
                    break;
            }
        }

        private void OnCanGobackChanged()
        {
            this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCanGoForwardChanged()
        {
            this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetCurrent(NavigationEntry entry)
        {
            this.current = entry;
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged();
        }

        /// <summary>
        /// Moves to the the entry.
        /// </summary>
        /// <param name="entry">The new entry</param>
        public void Navigate(NavigationEntry entry)
        {
            // navigate => always new entry
            // push current if exists to backstack
            // clear forward stack
            if (this.current != null)
                this.backStack.Add(this.Current);

            this.SetCurrent(entry);

            this.forwardStack.Clear();
        }

        /// <summary>
        /// Moves to root entry.
        /// </summary>
        public void NavigateToRoot()
        {
            // set current to root entry
            // clear back stack
            // clear forward stack

            this.SetCurrent(this.Root);

            this.backStack.Clear();
            this.forwardStack.Clear();
        }

        /// <summary>
        /// Moves back the history.
        /// </summary>
        /// <returns>The previous entry</returns>
        public NavigationEntry GoBack()
        {
            // get last backstack entry
            var newCurrent = this.backStack.LastOrDefault();
            if (newCurrent == null)
                throw new InvalidOperationException("Cannot go back. Back Stack is empty");

            this.backStack.RemoveAt(this.backStack.Count - 1); // remove last

            this.forwardStack.Add(this.Current);

            this.SetCurrent(newCurrent);

            return newCurrent;
        }

        /// <summary>
        /// Moves forward the history.
        /// </summary>
        /// <returns>The next entry</returns>
        public NavigationEntry GoForward()
        {
            // get last forwardstack entry
            var newCurrent = this.forwardStack.LastOrDefault();
            if (newCurrent == null)
                throw new InvalidOperationException("Cannot go forward. Forward Stack is empty");

            this.forwardStack.RemoveAt(this.forwardStack.Count - 1); // remove last

            // push current to back stack
            if (this.current == null)
                throw new InvalidOperationException("The current entry cannot be null");

            this.backStack.Add(this.Current);

            this.SetCurrent(newCurrent);

            return newCurrent;
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        public void Clear()
        {
            this.forwardStack.Clear();
            this.backStack.Clear();
            this.SetCurrent(null);
        }
    }

}
