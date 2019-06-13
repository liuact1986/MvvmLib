using MvvmLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Collection for Models and ViewModels. Implements <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SharedSourceItemCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        private readonly ILogger DefaultLogger = new DebugLogger();

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
            get
            {
                if (index < 0 || index >= this.items.Count)
                    throw new IndexOutOfRangeException();

                return this.items[index];
            }
            set { SetItem(index, value); }
        }

        /// <summary>
        /// The size of the collection.
        /// </summary>
        public int Count
        {
            get { return this.items.Count; }
        }

        internal Func<Type, object, bool> findAndSelectSelectable;
        /// <summary>
        /// The function used to find items that implement <see cref="ISelectable"/>.
        /// </summary>
        public Func<Type, object, bool> FindAndSelectSelectable
        {
            get { return findAndSelectSelectable; }
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
        /// Initializes the <see cref="SharedSourceItemCollection{T}"/>.
        /// </summary>
        public SharedSourceItemCollection()
        {
            this.items = new List<T>();
        }

        internal void Load(IList<T> initItems)
        {
            this.ClearFast();
            for (int i = 0; i < initItems.Count; i++)
            {
                var itemItem = initItems[i];
                this.InsertInternal(i, itemItem, null);
            }
        }

        internal void Load(Dictionary<T, object> initItems)
        {
            this.ClearFast();
            int i = 0;
            foreach (var item in initItems)
            {
                this.InsertInternal(i, item.Key, item.Value);
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

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter to pass to ViewModel</param>
        /// <returns>True if added</returns>
        public async Task<bool> InsertAsync(int index, T item, object parameter)
        {
            if (index < 0 || index > this.items.Count)
                throw new IndexOutOfRangeException();

            if (findAndSelectSelectable != null && findAndSelectSelectable(item.GetType(), parameter))
            {
                return false;
            }

            if (item is FrameworkElement)
            {
                var frameworkElement = item as FrameworkElement;
                if (await NavigationHelper.CanActivateAsync(frameworkElement, frameworkElement.DataContext, parameter))
                {
                    InsertInternal(index, item, parameter);

                    return true;
                }
            }
            else
            {
                if (await NavigationHelper.CanActivateAsync(item, parameter))
                {
                    InsertInternal(index, item, parameter);

                    return true;
                }
            }
            return false;
        }

        private void InsertInternal(int index, T item, object parameter)
        {
            NavigationHelper.OnNavigatingTo(item, parameter);

            this.items.Insert(index, item);

            NavigationHelper.OnNavigatedTo(item, parameter);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        /// <returns>True if added</returns>
        public async Task<bool> InsertAsync(int index, T item)
        {
            return await InsertAsync(index, item, null);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter to pass to ViewModel</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(T item, object parameter)
        {
            return await InsertAsync(this.items.Count, item, parameter);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(T item)
        {
            return await InsertAsync(this.items.Count, item, null);
        }

        private void SetItem(int index, T item)
        {
            if (index < 0 || index > this.items.Count)
                throw new IndexOutOfRangeException();

            T originalItem = items[index];

            this.items[index] = item;

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAtAsync(int index)
        {
            if (index < 0 && index > this.items.Count - 1)
                throw new IndexOutOfRangeException();

            var item = this.items[index];

            if (await NavigationHelper.CanDeactivateAsync(item))
            {
                NavigationHelper.OnNavigatingFrom(item);

                this.items.RemoveAt(index);

                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAsync(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                Logger.Log($"Unable to find the index for the item \"{item}\"", Category.Warn, Priority.High);
                return false;
            }
            else
                return await RemoveAtAsync(index);
        }

        /// <summary>
        /// Moves item  at old index to new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        public void Move(int oldIndex, int newIndex)
        {
            var removedItem = items[oldIndex];

            items.RemoveAt(oldIndex);
            items.Insert(newIndex, removedItem);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        /// <returns>True if all items removed</returns>
        public async Task<bool> ClearAsync()
        {
            bool success = true;
            int count = items.Count;
            // 3 ... 2 ... 1 ... 0 
            for (int index = count - 1; index >= 0; index--)
            {
                var item = this.items[index];

                if (await NavigationHelper.CanDeactivateAsync(item))
                {
                    NavigationHelper.OnNavigatingFrom(item);
                    this.items.RemoveAt(index);
                    OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Clears all items without checking <see cref="ICanDeactivate"/> and <see cref="INavigationAware.OnNavigatingFrom"/>.
        /// </summary>
        public void ClearFast()
        {
            this.items.Clear();
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
    }
}
