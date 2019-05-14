using System;
using System.Linq;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// History for content regions.
    /// </summary>
    public class NavigationHistory : INavigationHistory
    {
        private readonly BindableHistory backStack;
        /// <summary>
        /// The back stack.
        /// </summary>
        public BindableHistory BackStack
        {
            get { return backStack; }
        }

        private readonly BindableHistory forwardStack;
        /// <summary>
        /// The forward Stack.
        /// </summary>
        public BindableHistory ForwardStack
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
            get { return this.backStack.Count > 0 ? this.backStack.ElementAt(this.backStack.Count - 1) : null; }
        }

        /// <summary>
        /// Gets the next entry.
        /// </summary>
        public NavigationEntry Next
        {
            get { return this.forwardStack.Count > 0 ? this.forwardStack.ElementAt(this.forwardStack.Count - 1) : null; }
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
        /// Creates the history.
        /// </summary>
        public NavigationHistory()
        {
            this.backStack = new BindableHistory();
            this.HandleGoBackChanged();

            this.forwardStack = new BindableHistory();
            this.HandleGoForwardChanged();
        }

        /// <summary>
        /// Handles go back changed.
        /// </summary>
        public void HandleGoBackChanged()
        {
            this.backStack.EntryAdded += OnBackStackEntryAdded;
            this.backStack.EntryRemoved += OnBackStackEntryRemoved;
        }

        /// <summary>
        /// Unhandles go back changed.
        /// </summary>
        public void UnhandleGoBackChanged()
        {
            this.backStack.EntryAdded -= OnBackStackEntryAdded;
            this.backStack.EntryRemoved -= OnBackStackEntryRemoved;
        }

        /// <summary>
        /// Handles go forward changed.
        /// </summary>
        public void HandleGoForwardChanged()
        {
            this.forwardStack.EntryAdded += OnForwardStackEntryAdded;
            this.forwardStack.EntryRemoved += OnForwardStackEntryRemoved;
        }

        /// <summary>
        /// Unhandles go forward changed.
        /// </summary>
        public void UnhandleGoForwardChanged()
        {
            this.forwardStack.EntryAdded -= OnForwardStackEntryAdded;
            this.forwardStack.EntryRemoved -= OnForwardStackEntryRemoved;
        }

        private void OnBackStackEntryAdded(object sender, EventArgs e)
        {
            if (this.backStack.Count == 1)
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnBackStackEntryRemoved(object sender, EventArgs e)
        {
            if (this.backStack.Count == 0)
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardStackEntryAdded(object sender, EventArgs e)
        {
            if (this.forwardStack.Count == 1)
                this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardStackEntryRemoved(object sender, EventArgs e)
        {
            if (this.forwardStack.Count == 0)
                this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Moves to the the entry.
        /// </summary>
        /// <param name="navigationEntry">The new entry</param>
        public void Navigate(NavigationEntry navigationEntry)
        {
            // navigate => always new entry
            // push current if exists to backstack
            // clear forward stack
            if (this.current != null)
                this.backStack.Add(this.Current);

            this.current = navigationEntry;

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

            this.current = this.Root;
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

            this.current = newCurrent;
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

            // set new current
            this.current = newCurrent;
            return newCurrent;
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        public void Clear()
        {
            this.forwardStack.Clear();
            this.backStack.Clear();
            this.current = null;
        }
    }

}
