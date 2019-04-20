using System;
using System.Linq;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// History for content regions.
    /// </summary>
    public class NavigationHistory : INavigationHistory
    {
        private readonly BindableHistory<NavigationEntry> backStack;

        /// <summary>
        /// The back stack.
        /// </summary>
        public BindableHistory<NavigationEntry> BackStack
        {
            get { return backStack; }
        }

        private readonly BindableHistory<NavigationEntry> forwardStack;
        /// <summary>
        /// The forward Stack.
        /// </summary>
        public BindableHistory<NavigationEntry> ForwardStack
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
                if (this.BackStack.Count > 0)
                    return this.BackStack[0];
                else
                    return this.Current;
            }
        }

        /// <summary>
        /// Get the previous entry.
        /// </summary>
        public NavigationEntry Previous
        {
            get { return this.BackStack.Count > 0 ? this.BackStack.ElementAt(this.BackStack.Count - 1) : null; }
        }

        /// <summary>
        /// Get the next entry.
        /// </summary>
        public NavigationEntry Next
        {
            get { return this.ForwardStack.Count > 0 ? this.ForwardStack.ElementAt(this.ForwardStack.Count - 1) : null; }
        }

        private NavigationEntry current;
        /// <summary>
        /// Get the current entry.
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
            this.backStack = new BindableHistory<NavigationEntry>();
            this.HandleGoBackChanged();

            this.forwardStack = new BindableHistory<NavigationEntry>();
            this.HandleGoForwardChanged();
        }

        /// <summary>
        /// Handles go back changed.
        /// </summary>
        public void HandleGoBackChanged()
        {
            this.BackStack.ItemAdded += OnBackStackItemAdded;
            this.BackStack.ItemRemoved += OnBackStackItemRemoved;
        }

        /// <summary>
        /// Unhandles go back changed.
        /// </summary>
        public void UnhandleGoBackChanged()
        {
            this.BackStack.ItemAdded -= OnBackStackItemAdded;
            this.BackStack.ItemRemoved -= OnBackStackItemRemoved;
        }

        /// <summary>
        /// Handles go forward changed.
        /// </summary>
        public void HandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded += OnForwardStackItemAdded;
            this.ForwardStack.ItemRemoved += OnForwardStackItemRemoved;
        }

        /// <summary>
        /// Unhandles go forward changed.
        /// </summary>
        public void UnhandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded -= OnForwardStackItemAdded;
            this.ForwardStack.ItemRemoved -= OnForwardStackItemRemoved;
        }

        private void OnBackStackItemAdded(object sender, EventArgs e)
        {
            if (BackStack.Count == 1)
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnBackStackItemRemoved(object sender, EventArgs e)
        {
            if (BackStack.Count == 0)
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardStackItemAdded(object sender, EventArgs e)
        {
            if (ForwardStack.Count == 1)
                this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardStackItemRemoved(object sender, EventArgs e)
        {
            if (ForwardStack.Count == 0)
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
            var newCurrent = this.BackStack.LastOrDefault();
            if (newCurrent == null)
                throw new InvalidOperationException("Cannot go back. Back Stack is empty");

            this.backStack.RemoveAt(this.BackStack.Count - 1); // reMoves last

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
            var newCurrent = this.ForwardStack.LastOrDefault();
            if (newCurrent == null)
                throw new InvalidOperationException("Cannot go forward. Forward Stack is empty");

            this.forwardStack.RemoveAt(this.ForwardStack.Count - 1); // reMoves last

            // push current to back stack
            if (this.current == null)
                throw new InvalidOperationException("The current entry cannot be null");

            this.backStack.Add(this.Current);

            // set new current
            this.current = newCurrent;
            return newCurrent;
        }

        /// <summary>
        /// The clear the history.
        /// </summary>
        public void Clear()
        {
            this.forwardStack.Clear();
            this.backStack.Clear();
            this.current = null;
        }
    }
}
