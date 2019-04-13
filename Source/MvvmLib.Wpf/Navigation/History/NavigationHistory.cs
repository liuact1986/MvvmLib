using System;
using System.Linq;

namespace MvvmLib.Navigation
{

    public class NavigationHistory : INavigationHistory
    {
        public BindableList<NavigationEntry> BackStack { get; }

        public BindableList<NavigationEntry> ForwardStack { get; }

        public NavigationEntry Root
        {
            get
            {
                if (this.BackStack.Count > 0)
                {
                    return this.BackStack[0];
                }
                else
                {
                    return this.Current;
                }
            }
        }

        public NavigationEntry Previous => this.BackStack.Count > 0 ? this.BackStack.ElementAt(this.BackStack.Count - 1) : null;

        public NavigationEntry Next => this.ForwardStack.Count > 0 ? this.ForwardStack.ElementAt(this.ForwardStack.Count - 1) : null;

        public NavigationEntry Current { get; private set; }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged;

        public NavigationHistory()
        {
            this.BackStack = new BindableList<NavigationEntry>();
            this.HandleGoBackChanged();

            this.ForwardStack = new BindableList<NavigationEntry>();
            this.HandleGoForwardChanged();
        }

        public void HandleGoBackChanged()
        {
            this.BackStack.ItemAdded += OnBackStackItemAdded;
            this.BackStack.ItemRemoved += OnBackStackItemRemoved;
        }

        public void UnhandleGoBackChanged()
        {
            this.BackStack.ItemAdded -= OnBackStackItemAdded;
            this.BackStack.ItemRemoved -= OnBackStackItemRemoved;
        }

        public void HandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded += OnForwardStackItemAdded;
            this.ForwardStack.ItemRemoved += OnForwardStackItemRemoved;
        }

        public void UnhandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded -= OnForwardStackItemAdded;
            this.ForwardStack.ItemRemoved -= OnForwardStackItemRemoved;
        }

        private void OnBackStackItemAdded(object sender, EventArgs e)
        {
            if(BackStack.Count == 1)
            {
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnBackStackItemRemoved(object sender, EventArgs e)
        {
            if (BackStack.Count == 0)
            {
                this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
            }       
        }

        private void OnForwardStackItemAdded(object sender, EventArgs e)
        {
            if (ForwardStack.Count == 1)
            {
                this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnForwardStackItemRemoved(object sender, EventArgs e)
        {
            if (ForwardStack.Count == 0)
            {
                this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Navigate(NavigationEntry navigationEntry)
        {
            if (this.Current != null)
            {
                this.BackStack.Add(this.Current);
            }

            this.Current = navigationEntry;

            this.ForwardStack.Clear();
        }

        public void NavigateToRoot()
        {
            this.Current = this.Root;
            this.BackStack.Clear();
            this.ForwardStack.Clear();
        }

        public NavigationEntry GoBack()
        {
            // get last backstack entry
            var newCurrent = this.BackStack.LastOrDefault();
            if (newCurrent == null)
            {
                throw new InvalidOperationException("Cannot go back. Back Stack is empty");
            }
            this.BackStack.RemoveAt(this.BackStack.Count - 1); // remove last

            this.ForwardStack.Add(this.Current);

            this.Current = newCurrent;
            return newCurrent;
        }

        public NavigationEntry GoForward()
        {
            // get last forwardstack entry
            var newCurrent = this.ForwardStack.LastOrDefault();
            if (newCurrent == null)
            {
                throw new InvalidOperationException("Cannot go forward. Forward Stack is empty");
            }
            this.ForwardStack.RemoveAt(this.ForwardStack.Count - 1); // remove last

            // push current to back stack
            if (this.Current == null)
            {
                throw new InvalidOperationException("The current entry cannot be null");
            }
            this.BackStack.Add(this.Current);

            // set new current
            this.Current = newCurrent;
            return newCurrent;
        }

        public void Clear()
        {
            this.ForwardStack.Clear();
            this.BackStack.Clear();
            this.Current = null;
        }
    }
}
