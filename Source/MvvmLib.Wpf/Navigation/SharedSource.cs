using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Source for Models and ViewModels with a collection of Items and SelectedItem/SelectedIndex. 
    /// It supports Views but its not the target. 
    /// This is the source for ItemsControls, Selectors (ListBox, TabControl), etc.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SharedSource<T> : ISharedSource
    {
        private bool handleSelectionChanged;

        /// <summary>
        /// The type of the items.
        /// </summary>
        public Type SourceType
        {
            get { return typeof(T); }
        }

        private SharedSourceItemCollection<T> items;
        /// <summary>
        /// The items collection.
        /// </summary>
        public SharedSourceItemCollection<T> Items
        {
            get { return items; }
        }

        private T selectedItem;
        /// <summary>
        /// The selected item.
        /// </summary>
        public T SelectedItem
        {
            get { return selectedItem; }
            set { SetSelectedItem(value); }
        }

        private int selectedIndex;
        /// <summary>
        /// The selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetSelectedIndex(value); }
        }

        private SelectionHandling selectionHandling;
        /// <summary>
        /// Allows to select inserted item (Select by default).
        /// </summary>
        public SelectionHandling SelectionHandling
        {
            get { return selectionHandling; }
            set { selectionHandling = value; }
        }

        /// <summary>
        /// Invoked on selected item changed.
        /// </summary>
        public event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates the <see cref="SharedSource{T}"/>.
        /// </summary>
        public SharedSource()
        {
            this.items = new SharedSourceItemCollection<T>(this);
            this.selectionHandling = SelectionHandling.Select;
            this.handleSelectionChanged = true;
            this.selectedIndex = -1;
        }

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the list.
        /// </summary>
        /// <param name="list">The list</param>
        public SharedSource<T> Load(IList<T> list)
        {
            this.handleSelectionChanged = false;
            this.items.Load(list);
            this.handleSelectionChanged = true;
            if (this.items.Count > 0)
                TrySelectingItem(0);
            return this;
        }

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        public SharedSource<T> Load(Dictionary<T, object> dictionary)
        {
            this.handleSelectionChanged = false;
            this.items.Load(dictionary);
            this.handleSelectionChanged = true;
            if (this.items.Count > 0)
                TrySelectingItem(0);
            return this;
        }

        internal bool FindAndSelectSelectable(Type newItemType, object parameter)
        {
            var selectable = NavigationHelper.FindSelectable(this.items, newItemType, parameter);
            if (selectable != null)
            {
                SelectedItem = (T)selectable;
                return true;
            }
            return false;
        }

        private void SetSelectedItem(T value)
        {
            this.selectedIndex = this.items.IndexOf(value);
            selectedItem = value;

            OnPropertyChanged(nameof(SelectedIndex));
            OnPropertyChanged(nameof(SelectedItem));
            OnSelectedItemChanged(selectedIndex, selectedItem);
            UpdateIsSelected();
        }

        private void SetSelectedIndex(int value)
        {
            if (value < -1 || value > items.Count - 1)
                throw new IndexOutOfRangeException();

            if (value == -1)
            {
                selectedIndex = -1;
                selectedItem = default(T);
            }
            else
            {
                selectedIndex = value;
                selectedItem = items[selectedIndex];
            }

            OnPropertyChanged(nameof(SelectedIndex));
            OnPropertyChanged(nameof(SelectedItem));
            OnSelectedItemChanged(selectedIndex, selectedItem);
            UpdateIsSelected();
        }

        internal void TrySelectingItem(int index)
        {
            if (handleSelectionChanged && selectionHandling == SelectionHandling.Select)
                SetSelectedIndex(index);
        }

        private void UpdateIsSelected()
        {
            if (selectedIndex == -1)
                return;

            int index = 0;
            foreach (var item in items)
            {
                if (index == selectedIndex)
                {
                    if (item is IIsSelected)
                        ((IIsSelected)item).IsSelected = true;
                }
                else
                {
                    if (item is IIsSelected)
                        ((IIsSelected)item).IsSelected = false;
                }
                index++;
            }
        }

        internal void SelectItemAfterDeletion(int index)
        {
            if (this.items.Count == 0)
            {
                this.SetSelectedIndex(-1);
                return;
            }

            // remove selected
            // [A] B [C] => [A] C (old 1, new 1)
            // [A] B => A         (old 1, new 0)
            //  A => -1
            var oldIndex = this.selectedIndex;
            if (oldIndex == index)
            {
                if (this.items.Count > index)
                    this.SetSelectedIndex(oldIndex);
                else
                    this.SetSelectedIndex(oldIndex - 1);
            }
            else
            {
                // [x B] C [D] => selectedindex - 1
                // [A B] C [x] => selected index
                if (index < oldIndex)
                    this.SetSelectedIndex(oldIndex - 1);
                else
                    this.SetSelectedIndex(oldIndex);
            }
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <returns>The instance created</returns>
        public T CreateNew()
        {
            var item = SourceResolver.CreateInstance(typeof(T));
            return (T)item;
        }


        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <returns>The instance created</returns>
        public T CreateNew<TImplementation>() where TImplementation : T
        {
            var item = SourceResolver.CreateInstance(typeof(T));
            return (T)item;
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <param name="implementationType">The implementation type</param>
        /// <returns></returns>
        public T CreateNew(Type implementationType)
        {
            var item = SourceResolver.CreateInstance(implementationType);
            return (T)item;
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and inserts the item created at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The item added</returns>
        public T InsertNew(int index, object parameter)
        {
            var item = CreateNew();
            this.items.Insert(index, (T)item, parameter);
            return item;
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and inserts the item created at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item added</returns>
        public T InsertNew(int index)
        {
            return this.InsertNew(index, null);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and adds the item created.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>The item inserted</returns>
        public T AddNew(object parameter)
        {
            return this.InsertNew(this.items.Count, parameter);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and adds the item created.
        /// </summary>
        /// <returns>The item added</returns>
        public T AddNew()
        {
            return this.InsertNew(this.items.Count, null);
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        internal void Clear()
        {
            this.items.ClearFast(); 
            this.SetSelectedIndex(-1);
        }

        #region Events

        private void OnSelectedItemChanged(int index, object item)
        {
            SelectedItemChanged?.Invoke(this, new SharedSourceSelectedItemChangedEventArgs(index, item));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Events
    }

    /// <summary>
    /// The shared source selected item changed event args.
    /// </summary>
    public class SharedSourceSelectedItemChangedEventArgs : EventArgs
    {
        private readonly int selectedIndex;
        /// <summary>
        /// The new selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        private readonly object selectedItem;
        /// <summary>
        /// The new selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return selectedItem; }
        }

        /// <summary>
        /// Creates the <see cref="SharedSourceSelectedItemChangedEventArgs"/>.
        /// </summary>
        /// <param name="selectedIndex">The new selectedIndex</param>
        /// <param name="selectedItem">The new selected item.</param>
        public SharedSourceSelectedItemChangedEventArgs(int selectedIndex, object selectedItem)
        {
            this.selectedIndex = selectedIndex;
            this.selectedItem = selectedItem;
        }
    }

    /// <summary>
    /// The selection handling
    /// </summary>
    public enum SelectionHandling
    {
        /// <summary>
        /// Select.
        /// </summary>
        Select,
        /// <summary>
        /// None.
        /// </summary>
        None
    }

}
