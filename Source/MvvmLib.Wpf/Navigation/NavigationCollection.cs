using MvvmLib.History;
using MvvmLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Collection for Models and ViewModels. Implements <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NavigationCollection<T> : IList, IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private readonly ILogger DefaultLogger = new DebugLogger();
        private SharedSource<T> sharedSource;
        private Object syncRoot;

        internal readonly NavigationEntryCollection entries;
        /// <summary>
        /// The entry collection.
        /// </summary>
        public IReadOnlyCollection<NavigationEntry> Entries
        {
            get { return entries; }
        }

        private ILogger logger;
        /// <summary>
        /// The logger used.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        private IList<T> items;

        /// <summary>
        /// Gets the item at the index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return this.items[index]; }
            set { SetItem(index, value, null); }
        }

        object IList.this[int index]
        {
            get { return items[index]; }
            set { SetItem(index, (T)value, null); }
        }

        /// <summary>
        /// The size of the collection.
        /// </summary>
        public int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Checks if collection is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        /// <summary>
        /// Checks if the collection has a  fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return ((IList)items).IsFixedSize; }
        }

        /// <summary>
        /// The syncroot object.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    ICollection c = items as ICollection;
                    if (c != null)
                        syncRoot = c.SyncRoot;
                    else
                        Interlocked.CompareExchange<Object>(ref syncRoot, new Object(), null);
                }
                return syncRoot;
            }
        }

        /// <summary>
        /// Checks if collection is synchronized (false).
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Initializes the <see cref="NavigationCollection{T}"/>.
        /// </summary>
        public NavigationCollection(SharedSource<T> sharedSource)
        {
            if (sharedSource == null)
                throw new ArgumentNullException(nameof(sharedSource));

            this.items = new List<T>();
            this.entries = new NavigationEntryCollection();

            this.PropertyChanged = null;
            this.sharedSource = sharedSource;
        }

        internal void Load(IList<T> initItems)
        {
            this.ClearFast();
            for (int i = 0; i < initItems.Count; i++)
            {
                var itemItem = initItems[i];
                var navigationContext = new NavigationContext(typeof(T), null);
                this.InsertItem(i, itemItem, navigationContext);
            }
        }

        internal void Load(Dictionary<T, object> initItems)
        {
            this.ClearFast();
            int i = 0;
            foreach (var item in initItems)
            {
                var navigationContext = new NavigationContext(typeof(T), item.Value);
                this.InsertItem(i, item.Key, navigationContext);
                i++;
            }
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object removedItem, int newIndex, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, removedItem, newIndex, oldIndex));
        }

        #endregion // Events

        private void CheckIsReadOnly()
        {
            if (items.IsReadOnly)
                throw new NotSupportedException("The collection is readonly");
        }

        /// <summary>
        /// Checks if the item is in collection.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if found</returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The index</returns>
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        private void InsertItem(int index, T item, NavigationContext navigationContext)
        {
            NavigationHelper.OnNavigatingTo(item, navigationContext);

            this.items.Insert(index, item);

            NavigationHelper.OnNavigatedTo(item, navigationContext);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);

            this.entries.Insert(index, new NavigationEntry(typeof(T), item, navigationContext.Parameter));

            // after UI notification
            sharedSource.TrySelectingItem(index);
        }

        private void SetItem(int index, T item, object parameter)
        {
            CheckIsReadOnly();

            if (index < 0 || index > this.items.Count)
                throw new IndexOutOfRangeException();

            T originalItem = items[index];

            this.items[index] = item;

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);

            this.entries[index] = new NavigationEntry(typeof(T), item, parameter);

            sharedSource.TrySelectingItem(index);
        }

        private void RemoveItem(int index, T item, NavigationContext navigationContext)
        {
            NavigationHelper.OnNavigatingFrom(item, navigationContext);

            this.items.RemoveAt(index);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);

            this.entries.RemoveAt(index);

            sharedSource.SelectItemAfterDeletion(index);
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter to pass to ViewModel</param>
        public void Insert(int index, T item, object parameter)
        {
            CheckIsReadOnly();

            if (index < 0 || index > this.items.Count)
                throw new IndexOutOfRangeException();

            if (sharedSource.FindAndSelectSelectable(item.GetType(), parameter))
            {
                return;
            }

            var navigationContext = new NavigationContext(typeof(T), parameter);
            NavigationHelper.CanActivate(item, navigationContext, canActivate =>
            {
                if (canActivate)
                    InsertItem(index, item, navigationContext);
                // log / notify fail ?
            });
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        public void Insert(int index, T item)
        {
            this.Insert(index, item, null);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter to pass to ViewModel</param>
        public void Add(T item, object parameter)
        {
            this.Insert(this.items.Count, item, parameter);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(T item)
        {
            this.Insert(this.items.Count, item, null);
        }

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        /// <param name="parameter"></param>
        public void Replace(int index, T newItem, object parameter)
        {
            CheckIsReadOnly();

            if (index < 0 || index > this.items.Count - 1)
                throw new IndexOutOfRangeException();

            var oldItem = items[index];
            var navigationContext = new NavigationContext(typeof(T), parameter);
            NavigationHelper.Replace(oldItem, newItem, navigationContext, () =>
            {
                SetItem(index, newItem, navigationContext.Parameter);
            }, success => { });
        }

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newItem"></param>
        public void Replace(int index, T newItem)
        {
            Replace(index, newItem, null);
        }

        /// <summary>
        /// Moves item  at old index to new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        public void Move(int oldIndex, int newIndex)
        {
            CheckIsReadOnly();

            var removedItem = items[oldIndex];

            items.RemoveAt(oldIndex);
            items.Insert(newIndex, removedItem);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);

            sharedSource.TrySelectingItem(newIndex);
        }

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveAt(int index)
        {
            CheckIsReadOnly();

            if (index < 0 && index > this.items.Count - 1)
                throw new IndexOutOfRangeException();

            var item = this.items[index];

            var entry = this.entries[index];
            var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
            NavigationHelper.CanDeactivate(item, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                    RemoveItem(index, item, navigationContext);
            });
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                Logger.Log($"Unable to find the index for the item \"{item}\"", Category.Warn, Priority.High);
                return false;
            }
            else
            {
                RemoveAt(index);
                return true; // caution
            }
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void Clear()
        {
            CheckIsReadOnly();

            int count = items.Count;
            // 3 ... 2 ... 1 ... 0 
            for (int index = count - 1; index >= 0; index--)
            {
                var item = this.items[index];

                var entry = this.entries[index];
                var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
                NavigationHelper.CanDeactivate(item, navigationContext, canDeactivate =>
                {
                    if (canDeactivate)
                        RemoveItem(index, item, navigationContext);
                });
            }
        }

        /// <summary>
        /// Clears all items without checking <see cref="ICanDeactivate"/> and <see cref="INavigationAware.OnNavigatingFrom"/>.
        /// </summary>
        public void ClearFast()
        {
            CheckIsReadOnly();

            this.items.Clear();
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.entries.Clear();

            sharedSource.SelectItemAfterDeletion(-1);
        }

        /// <summary>
        /// Copy the items to the array.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="index">The index</param>
        public void CopyTo(T[] array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value</param>
        public int Add(object value)
        {
            try
            {
                this.Add((T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Unable to add value '{value}'");
            }

            return this.Count - 1;
        }

        /// <summary>
        /// Checks if the value is in collection.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>True if found</returns>
        public bool Contains(object value)
        {
            return this.Contains((T)value);
        }

        /// <summary>
        /// Gets the index of the value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The index or -1</returns>
        public int IndexOf(object value)
        {
            return this.IndexOf((T)value);
        }

        /// <summary>
        /// Inserts the value at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="value">The value</param>
        public void Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="value">The value</param>
        public void Remove(object value)
        {
            this.Remove((T)value);
        }

        /// <summary>
        /// Copy the items to the array.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="index">The index</param>
        public void CopyTo(Array array, int index)
        {
            this.CopyTo((T[])array, index);
        }
    }
}
