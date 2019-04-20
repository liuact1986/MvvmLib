using System;
using System.Collections;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public class BindableHistoryItemEventArgs : EventArgs
    {
        private readonly object item;
        public object Item
        {
            get { return item; }
        }

        private readonly int? index;
        public int? Index
        {
            get { return index; }
        }

        public BindableHistoryItemEventArgs(object item, int? index)
        {
            this.item = item;
            this.index = index;
        }
    }

    public sealed class BindableHistory<T> : IList<T>
    {
        private List<T> items = new List<T>();

        public event EventHandler<BindableHistoryItemEventArgs> ItemAdded;
        public event EventHandler<BindableHistoryItemEventArgs> ItemRemoved;
        public event EventHandler<BindableHistoryItemEventArgs> ItemUpdated;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > items.Count - 1)
                    throw new IndexOutOfRangeException();

                return items[index];
            }
            set
            {
                if (isReadOnly)
                    throw new InvalidOperationException("List is readonly");
                if (index < 0 || index > items.Count - 1)
                    throw new IndexOutOfRangeException();

                items[index] = value;
                this.ItemUpdated?.Invoke(this, new BindableHistoryItemEventArgs(items[index], index));
            }
        }

        public int Count => items.Count;

        private bool isReadOnly;
        public bool IsReadOnly => isReadOnly;

        public BindableHistory(bool isReadyOnly)
        {
            this.isReadOnly = isReadOnly;
        }

        public BindableHistory()
            : this(false)
        { }

        public void Add(T item)
        {
            if (isReadOnly)
                throw new InvalidOperationException("List is readonly");

            this.Insert(items.Count, item);
        }

        public void Clear()
        {
            if (isReadOnly)
                throw new InvalidOperationException("List is readonly");

            if (items.Count > 0)
            {
                int count = items.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    this.RemoveAt(i);
                }
            }
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (isReadOnly)
                throw new InvalidOperationException("List is readonly");
            if (index < 0 || index > items.Count)
                throw new IndexOutOfRangeException();

            items.Insert(index, item);
            this.ItemAdded?.Invoke(this, new BindableHistoryItemEventArgs(item, index));

        }

        public bool Remove(T item)
        {
            if (isReadOnly)
                throw new InvalidOperationException("List is readonly");

            if (items.Remove(item))
            {
                var index = items.IndexOf(item);
                this.ItemRemoved?.Invoke(this, new BindableHistoryItemEventArgs(item, index));
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (isReadOnly)
                throw new InvalidOperationException("List is readonly");
            if (index < 0 || index > items.Count - 1)
                throw new IndexOutOfRangeException();

            items.RemoveAt(index);
            this.ItemRemoved?.Invoke(this, new BindableHistoryItemEventArgs(null, index));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
