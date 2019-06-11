using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Navigation
{

    public class SharedSource<T> : ISharedSource
    {
        public Type SourceType
        {
            get { return typeof(T); }
        }

        private SharedSourceItemCollection<T> items;
        public SharedSourceItemCollection<T> Items
        {
            get { return items; }
        }

        private T selectedItem;
        public T SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value == null || !value.Equals(selectedItem))
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    OnSelectedItemChanged(selectedItem);
                    SetIsSelected();
                }
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value < -1 || value > items.Count - 1)
                    throw new IndexOutOfRangeException();

                if (value != selectedIndex)
                {
                    if (value == -1)
                    {
                        selectedIndex = -1;
                        SelectedItem = default(T);
                    }
                    else
                    {
                        selectedIndex = value;
                        SelectedItem = items[selectedIndex];
                    }
                }
            }
        }

        private void SetIsSelected()
        {
            var selectedIndex = items.IndexOf(selectedItem);
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

        private InsertionHandling insertionHandling;
        public InsertionHandling InsertionHandling
        {
            get { return insertionHandling; }
            set { insertionHandling = value; }
        }

        private DeletionHandling deletionHandling;
        public DeletionHandling DeletionHandling
        {
            get { return deletionHandling; }
            set { deletionHandling = value; }
        }

        public event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public SharedSource()
        {
            Intialize(null, false);
        }

        public SharedSource(IList<T> initItems)
        {
            if (initItems == null)
                throw new ArgumentNullException(nameof(initItems));

            Intialize(initItems, true);
        }

        public SharedSource(Dictionary<T, object> initItems)
        {
            if (initItems == null)
                throw new ArgumentNullException(nameof(initItems));

            Intialize(initItems, false);
        }

        public SharedSource<T> With(IList<T> initItems)
        {
            if (initItems == null)
                throw new ArgumentNullException(nameof(initItems));

            Intialize(initItems, true);
            return this;
        }

        public SharedSource<T> With(Dictionary<T, object> initItems)
        {
            if (initItems == null)
                throw new ArgumentNullException(nameof(initItems));

            Intialize(initItems, false);
            return this;
        }

        private void Intialize(IEnumerable initItems, bool isList)
        {
            if (initItems != null)
            {
                this.items = isList ? new SharedSourceItemCollection<T>((IList<T>)initItems)
                    : new SharedSourceItemCollection<T>((Dictionary<T, object>)initItems);
                this.items.findAndSelectSelectable = (type, parameter) => FindAndSelectSelectable(type, parameter);
                if (this.items.Count > 0)
                    TrySelectingItem(0);
            }
            else
            {
                this.items = new SharedSourceItemCollection<T>();
                this.items.findAndSelectSelectable = (type, parameter) => FindAndSelectSelectable(type, parameter);
            }
            this.items.CollectionChanged += OnItemsCollectionChanged;
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
                    TrySelectingItemAfterDeletion(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    TrySelectingItem(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    TrySelectingItem(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TrySelectingItemAfterDeletion(-1);
                    break;
            }
        }

        private void TrySelectingItem(int index)
        {
            if (insertionHandling == InsertionHandling.SelectInserted)
                SelectedItem = items[index];
        }

        private void TrySelectingItemAfterDeletion(int index)
        {
            if (deletionHandling == DeletionHandling.Select)
            {
                // index:       0...1...2...3
                // count        1   2   3   4
                // delete index 
                //              0                 => select index 
                //                  1             => select index 
                //                           3    => select index -1
                //              0 (count 0 after) => select index -1
                if (this.items.Count == 0)
                    return;

                if (this.items.Count > index)
                {
                    this.SelectedItem = this.items[index];
                }
                else
                {
                    this.SelectedItem = this.items[index - 1];
                }
            }
        }

        public T CreateNewItem()
        {
            var item = SourceResolver.CreateInstance(typeof(T));
            return (T)item;
        }

        #region Events

        private void OnSelectedItemChanged(object item)
        {
            SelectedItemChanged?.Invoke(this, new SharedSourceSelectedItemChangedEventArgs(item));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Events
    }


    public class SharedSourceSelectedItemChangedEventArgs : EventArgs
    {
        private readonly object selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
        }

        public SharedSourceSelectedItemChangedEventArgs(object selectedSource)
        {
            this.selectedItem = selectedSource;
        }
    }

    public enum InsertionHandling
    {
        SelectInserted,
        None
    }

    public enum DeletionHandling
    {
        Select,
        None
    }
}
