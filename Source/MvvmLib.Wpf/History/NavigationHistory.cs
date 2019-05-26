using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The Navigation History.
    /// </summary>
    public sealed class NavigationHistory : INotifyPropertyChanged
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
        /// Chekcs if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.backStack.Count > 0; }
        }

        /// <summary>
        /// Chekcs if can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this.forwardStack.Count > 0; }
        }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler<CanGoBackEventArgs> CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler<CanGoForwardEventArgs> CanGoForwardChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on current entry changed.
        /// </summary>
        public event EventHandler<CurrentEntryChangedEventArgs> CurrentChanged;

        /// <summary>
        /// Creates the history.
        /// </summary>
        public NavigationHistory()
        {
            this.backStack = new NavigationEntryCollection();
            this.forwardStack = new NavigationEntryCollection();

            HandleGoBackChanged();
            HandleGoForwardChanged();
        }

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, new CurrentEntryChangedEventArgs(Current));
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
                        OnCanGobackChanged(true);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.backStack.Count == 0)
                        OnCanGobackChanged(false);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCanGobackChanged(false);
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
                        OnCanGoForwardChanged(true);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.forwardStack.Count == 0)
                        OnCanGoForwardChanged(false);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCanGoForwardChanged(false);
                    break;
            }
        }

        private void OnCanGobackChanged(bool canGoBack)
        {
            this.CanGoBackChanged?.Invoke(this, new CanGoBackEventArgs(canGoBack));
        }

        private void OnCanGoForwardChanged(bool canGoForward)
        {
            this.CanGoForwardChanged?.Invoke(this, new CanGoForwardEventArgs(canGoForward));
        }

        private void SetCurrent(NavigationEntry entry)
        {
            this.current = entry;
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged();
        }

        /// <summary>
        /// Moves to the entry.
        /// </summary>
        /// <param name="entry">The new entry</param>
        public void Navigate(NavigationEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            // navigate => always new entry
            // push current if exists to backstack
            // clear forward stack
            if (this.current != null)
                this.backStack.Add(this.current);

            this.SetCurrent(entry);

            if (this.forwardStack.Count > 0)
                this.forwardStack.Clear();
        }

        /// <summary>
        /// Moves to the root entry.
        /// </summary>
        public void NavigateToRoot()
        {
            if (!CanGoBack && current == null)
                throw new InvalidOperationException("Cannot process navigate to root. The history is empty");

            // set current to root entry
            // clear back stack
            // clear forward stack

            this.SetCurrent(this.Root);

            this.backStack.Clear();
            this.forwardStack.Clear();
        }

        /// <summary>
        /// Moves to the previous entry.
        /// </summary>
        /// <returns>The previous entry</returns>
        public NavigationEntry GoBack()
        {
            if (!CanGoBack)
                throw new InvalidOperationException("Cannot process go back. The back stack is empty");

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
        /// Moves to the next entry.
        /// </summary>
        /// <returns>The next entry</returns>
        public NavigationEntry GoForward()
        {
            if (!CanGoForward)
                throw new InvalidOperationException("Cannot process go forward. The forward stack is empty");

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
