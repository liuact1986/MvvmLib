using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Navigation
{
    public class BindableItemEventArgs : EventArgs
    {
        public object Item { get; }
        public int? Index { get; }

        public BindableItemEventArgs(object item, int? index)
        {
            this.Item = item;
            this.Index = index;
        }
    }

    public class BindableList<T> : IList<T>
    {
        protected List<T> list = new List<T>();

        public event EventHandler<BindableItemEventArgs> ItemAdded;

        public event EventHandler<BindableItemEventArgs> ItemRemoved;

        public event EventHandler<BindableItemEventArgs> ItemUpdated;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > list.Count - 1)
                {
                    throw new IndexOutOfRangeException();
                }
                return list[index];
            }
            set
            {
                if (isReadOnly) { throw new InvalidOperationException("List is readony"); }

                if (index < 0 || index > list.Count - 1)
                {
                    throw new IndexOutOfRangeException();
                }
                list[index] = value;
                this.ItemUpdated?.Invoke(this, new BindableItemEventArgs(list[index], index));
            }
        }

        public int Count => list.Count;

        protected bool isReadOnly;
        public bool IsReadOnly => isReadOnly;

        public void SetReadOnly(bool isReadOnly = true)
        {
            this.isReadOnly = isReadOnly;
        }

        public void Add(T item)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readony"); }
            this.Insert(list.Count, item);

        }

        public void Clear()
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readony"); }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    this.RemoveAt(i);
                }
            }

        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readony"); }
            if (index < 0 || index > list.Count)
            {
                throw new IndexOutOfRangeException();
            }
            list.Insert(index, item);
            this.ItemAdded?.Invoke(this, new BindableItemEventArgs(item, index));

        }

        public bool Remove(T item)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readony"); }
            if (list.Remove(item))
            {
                var index = list.IndexOf(item);
                this.ItemRemoved?.Invoke(this, new BindableItemEventArgs(item, index));
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readony"); }
            if (index < 0 || index > list.Count - 1)
            {
                throw new IndexOutOfRangeException();
            }
            list.RemoveAt(index);
            this.ItemRemoved?.Invoke(this, new BindableItemEventArgs(null, index));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }

    public class NavigationHistory : INavigationHistory
    {
        // current 

        // - 0 : Home
        // - 1: Page A
        // - 2: Page B
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
            this.BackStack.ItemAdded += OnBackStackChanged;
            this.BackStack.ItemRemoved += OnBackStackChanged;
        }

        public void UnhandleGoBackChanged()
        {
            this.BackStack.ItemAdded -= OnBackStackChanged;
            this.BackStack.ItemRemoved -= OnBackStackChanged;
        }

        public void HandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded += OnForwardStackChanged;
            this.ForwardStack.ItemRemoved += OnForwardStackChanged;
        }

        public void UnhandleGoForwardChanged()
        {
            this.ForwardStack.ItemAdded -= OnForwardStackChanged;
            this.ForwardStack.ItemRemoved -= OnForwardStackChanged;
        }

        private void OnBackStackChanged(object sender, EventArgs e)
        {
            this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardStackChanged(object sender, EventArgs e)
        {
            this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
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
