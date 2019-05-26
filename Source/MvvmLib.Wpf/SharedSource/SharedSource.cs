using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

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
                if (value < 0 || value >= items.Count)
                    throw new IndexOutOfRangeException();

                if (value != selectedIndex)
                {
                    selectedIndex = value;
                    SelectedItem = items[selectedIndex];
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
                if(index == selectedIndex)
                {
                    if(item is IIsSelected)
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
            Intialize(null);
        }

        public SharedSource(IList<T> list)
        {
            Intialize(list);
        }

        public SharedSource<T> With(IList<T> list)
        {
            Intialize(list);
            return this;
        }

        private void Intialize(IList<T> list)
        {
            if (list != null)
            {
                this.items = new SharedSourceItemCollection<T>(list);
                this.items.Filter = (type, parameter) => TryGetSelectable(type, parameter);
                if (this.items.Count > 0)
                    TrySelect(0);
            }
            else
            {
                this.items = new SharedSourceItemCollection<T>();
                this.items.Filter = (type, parameter) => TryGetSelectable(type, parameter);
            }
            this.items.CollectionChanged += OnItemsCollectionChanged;
        }


        /// <summary>
        /// Tries to get the selectable.
        /// </summary>
        /// <param name="newItemType">The type of the new item</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The selectable found or null</returns>
        protected bool TryGetSelectable(Type newItemType, object parameter)
        {
            foreach (var item in items)
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

            return false;
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TrySelect(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    TrySelectingSourceAfterDeletion(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    TrySelect(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    TrySelect(e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TrySelectingSourceAfterDeletion(-1);
                    break;
            }
        }

        private void TrySelect(int index)
        {
            if (insertionHandling == InsertionHandling.SelectInserted)
                SelectedItem = items[index];
        }

        public T CreateNew()
        {
            return Activator.CreateInstance<T>();
        }

        private void TrySelectingSourceAfterDeletion(int index)
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
        private object selectedItem;
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
