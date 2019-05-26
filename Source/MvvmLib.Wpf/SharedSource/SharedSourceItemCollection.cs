using MvvmLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    public class SharedSourceItemCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        private readonly ILogger DefaultLogger = new DebugLogger();

        private ILogger logger;
        /// <summary>
        /// The logger used by the library.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        private IList<T> items;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.items.Count)
                    throw new IndexOutOfRangeException();

                return this.items[index];
            }
            set
            {
                SetItem(index, value);
            }
        }


        public int Count
        {
            get { return this.items.Count; }
        }

        public Func<Type, object, bool> Filter { get; internal set; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public SharedSourceItemCollection()
        {
            this.items = new List<T>();
        }

        internal void SetItems(IList<T> list)
        {
            items = list;
        }

        public SharedSourceItemCollection(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            SetItems(list);
        }

        private async Task CheckCanActivateOrThrowAsync(T item, object parameter)
        {
            if (item is ICanActivate)
            {
                if (!await ((ICanActivate)item).CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.Source, item, this);
            }
        }

        private async Task CheckCanDeactivateOrThrowAsync(T item)
        {
            if (item is ICanDeactivate)
            {
                if (!await ((ICanDeactivate)item).CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Source, item, this);
            }
        }

        private static void OnNavigatingTo(T item, object parameter)
        {
            if (item is INavigatable)
                ((INavigatable)item).OnNavigatingTo(parameter);
        }

        private static void OnNavigatedTo(T item, object parameter)
        {
            if (item is INavigatable)
                ((INavigatable)item).OnNavigatingTo(parameter);
        }

        private static void OnNavigatingFrom(T item)
        {
            if (item is INavigatable)
                ((INavigatable)item).OnNavigatingFrom();
        }


        public async Task<bool> InsertAsync(int index, T item, object parameter)
        {
            bool success = true;
            try
            {
                if (index < 0 || index > this.items.Count)
                    throw new IndexOutOfRangeException();

                if(Filter != null && Filter(item.GetType(), parameter))
                {
                    return true;
                }

                // TODO : lien entre control selector, handle selection changed event
                //  shared source collection items

                await CheckCanActivateOrThrowAsync(item, parameter);

                OnNavigatingTo(item, parameter);

                this.items.Insert(index, item);

                OnNavigatedTo(item, parameter);

                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                success = false;
            }

            return success;
        }

        public async Task<bool> InsertAsync(int index, T item)
        {
            return await InsertAsync(index, item, null);
        }

        public async Task<bool> AddAsync(T item, object parameter)
        {
            return await InsertAsync(this.items.Count, item, parameter);
        }

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

        public async Task<bool> RemoveAtAsync(int index)
        {
            bool success = true;

            try
            {
                if (index < 0 && index >= this.items.Count)
                    throw new IndexOutOfRangeException();

                var item = this.items[index];

                await CheckCanDeactivateOrThrowAsync(item);

                OnNavigatingFrom(item);

                this.items.RemoveAt(index);

                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                success = false;
            }

            return success;
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public async Task<bool> RemoveAsync(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                // throw new Exception("Unable to find the index for the item"); // log
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
            MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Moves item  at old index to new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            var removedItem = items[oldIndex];

            items.RemoveAt(oldIndex);
            items.Insert(newIndex, removedItem);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }

        public async Task<bool> ClearAsync()
        {
            bool success = true;
            try
            {
                int count = items.Count;
                // 3 ... 2 ... 1 ... 0 
                for (int index = count - 1; index >= 0; index--)
                {
                    var item = this.items[index];

                    await CheckCanDeactivateOrThrowAsync(item);

                    OnNavigatingFrom(item);

                    this.items.RemoveAt(index);
                }
                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                success = false;
            }

            return success;
        }

        public void CopyTo(T[] array, int index)
        {
            items.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
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
    }

}
