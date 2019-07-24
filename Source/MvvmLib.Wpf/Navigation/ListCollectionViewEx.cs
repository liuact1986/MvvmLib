using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// ListCollectionView with commands and shortcuts.
    /// </summary>
    public class ListCollectionViewEx : ListCollectionView
    {
        /// <summary>
        /// Allows to get the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item</returns>
        public object this[int index]
        {
            get { return this.InternalList[index]; }
        }

        private int rank;
        /// <summary>
        /// The rank (index + 1).
        /// </summary>
        public int Rank
        {
            get { return rank; }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.CurrentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.CurrentPosition < this.Count - 1; }
        }

       
        /// <summary>
        /// Creates the <see cref="ListCollectionViewEx"/>.
        /// </summary>
        /// <param name="list">The source collection</param>
        public ListCollectionViewEx(IList list)
            : base(list)
        {
            this.CurrentChanged += OnCollectionViewCurrentChanged;
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            rank = this.CurrentPosition + 1;
            OnPropertyChanged(nameof(Rank));
        }

        #region Events

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Events
  
        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <param name="itemType">The item type</param>
        /// <returns>The item created</returns>
        public virtual object CreateNew(Type itemType)
        {
            var item = SourceResolver.CreateInstance(itemType);
            return item;
        }

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The item created</returns>
        public T CreateNew<T>()
        {
            return (T)CreateNew(typeof(T));
        }

        /// <summary>
        /// Allows to define a filter with generics.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter</param>
        public void FilterBy<T>(Predicate<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            this.Filter = new Predicate<object>(p => filter((T)p));
        }

        private void SortBy(string propertyName, ListSortDirection sortDirection, bool clearSortDescriptions)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (clearSortDescriptions)
                this.SortDescriptions.Clear();

            this.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortByDescending(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Descending, clearSortDescriptions);
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortBy(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Ascending, clearSortDescriptions);
        }

        /// <summary>
        /// Clears the filter.
        /// </summary>
        public void ClearFilter()
        {
            this.Filter = null;
        }
    }
}
