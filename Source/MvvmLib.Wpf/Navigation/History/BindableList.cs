using System;
using System.Collections;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public class BindableListItemEventArgs : EventArgs
    {
        public object Item { get; }
        public int? Index { get; }

        public BindableListItemEventArgs(object item, int? index)
        {
            this.Item = item;
            this.Index = index;
        }
    }

    public class BindableList<T> : IList<T>
    {
        protected List<T> list = new List<T>();

        public event EventHandler<BindableListItemEventArgs> ItemAdded;

        public event EventHandler<BindableListItemEventArgs> ItemRemoved;

        public event EventHandler<BindableListItemEventArgs> ItemUpdated;

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
                if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

                if (index < 0 || index > list.Count - 1)
                {
                    throw new IndexOutOfRangeException();
                }
                list[index] = value;
                this.ItemUpdated?.Invoke(this, new BindableListItemEventArgs(list[index], index));
            }
        }

        public int Count => list.Count;

        protected bool isReadOnly;
        public bool IsReadOnly => isReadOnly;

        public BindableList(bool isReadyOnly)
        {
            this.isReadOnly = isReadOnly;
        }

        public BindableList()
            : this(false)
        { }

        public void Add(T item)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

            this.Insert(list.Count, item);
        }

        public void Clear()
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

            if (list.Count > 0)
            {
                int count = list.Count;
                for (int i = count - 1; i >= 0; i--)
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
            if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

            if (index < 0 || index > list.Count)
            {
                throw new IndexOutOfRangeException();
            }
            list.Insert(index, item);
            this.ItemAdded?.Invoke(this, new BindableListItemEventArgs(item, index));

        }

        public bool Remove(T item)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

            if (list.Remove(item))
            {
                var index = list.IndexOf(item);
                this.ItemRemoved?.Invoke(this, new BindableListItemEventArgs(item, index));
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (isReadOnly) { throw new InvalidOperationException("List is readonly"); }

            if (index < 0 || index > list.Count - 1)
            {
                throw new IndexOutOfRangeException();
            }
            list.RemoveAt(index);
            this.ItemRemoved?.Invoke(this, new BindableListItemEventArgs(null, index));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
