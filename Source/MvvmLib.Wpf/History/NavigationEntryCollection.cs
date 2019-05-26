using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation entry collection. Allows to notify changes. Implements <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class NavigationEntryCollection : Collection<NavigationEntry>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        /// <summary>
        /// Invoked on collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// Inserts the item and notify changes.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The navigation entry</param>
        protected override void InsertItem(int index, NavigationEntry item)
        {
            base.InsertItem(index, item);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <summary>
        /// Sets the item and notify changes.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The navigation entry</param>
        protected override void SetItem(int index, NavigationEntry item)
        {
            var oldItem = base.Items[index];
            base.SetItem(index, item);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }

        /// <summary>
        /// Inserts the item at the index and notify changes.
        /// </summary>
        /// <param name="index">The index</param>
        protected override void RemoveItem(int index)
        {
            var item = Items[index];
            base.RemoveItem(index);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
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
            var removedItem = base.Items[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);

            OnPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        protected override void ClearItems()
        {
            Items.Clear();
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
