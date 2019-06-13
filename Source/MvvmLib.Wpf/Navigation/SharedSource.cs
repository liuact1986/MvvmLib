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
            set
            {
                this.selectedIndex = this.items.IndexOf(value);
                selectedItem = value;

                OnPropertyChanged(nameof(SelectedIndex));
                OnPropertyChanged(nameof(SelectedItem));
                OnSelectedItemChanged(selectedIndex, selectedItem);
                UpdateIsSelected();
            }
        }

        private int selectedIndex;
        /// <summary>
        /// The selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
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
            this.items = new SharedSourceItemCollection<T>();
            this.selectionHandling = SelectionHandling.Select;
            this.items.findAndSelectSelectable = new Func<Type, object, bool>((type, parameter) => FindAndSelectSelectable(type, parameter));
            this.handleSelectionChanged = true;
            this.items.CollectionChanged += OnItemsCollectionChanged;
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

        private bool FindAndSelectSelectable(Type newItemType, object parameter)
        {
            foreach (var item in items)
            {
                if (item is FrameworkElement)
                {
                    var frameworkElement = item as FrameworkElement;
                    if (frameworkElement.DataContext is ISelectable)
                    {
                        if (((ISelectable)frameworkElement.DataContext).IsTarget(newItemType, parameter))
                        {
                            SelectedItem = item;
                            return true;
                        }
                    }
                }
                else
                {
                    if (item is ISelectable)
                    {
                        if (((ISelectable)item).IsTarget(newItemType, parameter))
                        {
                            SelectedItem = item;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TrySelectingItem(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    SelectItemAfterDeletion(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    TrySelectingItem(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    TrySelectingItem(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    SelectItemAfterDeletion(-1);
                    break;
            }
        }

        private void TrySelectingItem(int index)
        {
            if (handleSelectionChanged && selectionHandling == SelectionHandling.Select)
                SelectedItem = items[index];
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
                    {
                        ((IIsSelected)item).IsSelected = true;
                    }
                }
                else
                {
                    if (item is IIsSelected)
                    {
                        ((IIsSelected)item).IsSelected = false;
                    }
                }
                index++;
            }
        }

        private void SelectItemAfterDeletion(int index)
        {
            // index:       0...1...2...3
            // count        1   2   3   4
            // delete index 
            //              0                 => select index 
            //                  1             => select index 
            //                           3    => select index -1
            //              0 (count 0 after) => select index -1
            if (this.items.Count == 0)
            {
                this.SelectedIndex = -1;
                return;
            }

            if (this.items.Count > index)
            {
                this.SelectedItem = this.items[index];
            }
            else
            {
                this.SelectedItem = this.items[index - 1];
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
        public T CreateNew<TImplementation>() where TImplementation: T
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
        public async Task<T> InsertNewAsync(int index, object parameter)
        {
            var item = CreateNew();
            if (await this.items.InsertAsync(index, (T)item, parameter))
                return item;

            return default(T);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and inserts the item created at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item added</returns>
        public async Task<T> InsertNewAsync(int index)
        {
            return await this.InsertNewAsync(index, null);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and adds the item created.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>The item inserted</returns>
        public async Task<T> AddNewAsync(object parameter)
        {
            return await this.InsertNewAsync(this.items.Count, parameter);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory and adds the item created.
        /// </summary>
        /// <returns>The item added</returns>
        public async Task<T> AddNewAsync()
        {
            return await this.InsertNewAsync(this.items.Count, null);
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        internal void Clear()
        {
            //this.items.ClearWithoutCheckingCanDeactivate();
            this.items.ClearFast(); // ??
            this.SelectedIndex = -1;
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
