using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region Adapter for <see cref="ItemsControl"/>.
    /// </summary>
    public class ItemsControlRegionAdapter : IItemsRegionAdapter
    {
        private ItemsControl control;
        /// <summary>
        /// The control.
        /// </summary>
        public DependencyObject Control
        {
            get { return control; }
            set
            {
                if (value is ItemsControl itemsControl)
                    control = itemsControl;
                else
                    throw new InvalidOperationException("Invalid control type");
            }
        }

        /// <summary>
        /// Creates the items region adapter.
        /// </summary>
        /// <param name="control">The items control</param>
        public ItemsControlRegionAdapter(ItemsControl control)
        {
            this.control = control ?? throw new ArgumentNullException(nameof(control));
        }

        /// <summary>
        /// Allows to bind control to region.
        /// </summary>
        /// <param name="region">The items region</param>
        public void Adapt(ItemsRegion region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));
            if (control.ItemsSource != null || (BindingOperations.GetBinding(control, ItemsControl.ItemsSourceProperty) != null))
                throw new InvalidOperationException("The ItemsSource is set");
            if (control.Items.Count > 0)
                throw new InvalidOperationException("The ItemCollection is not empty");

            region.History.Entries.CollectionChanged += OnEntriesCollectionChanged;
        }

        private void OnEntriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = -1;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        control.Items.Insert(index, ((NavigationEntry)item).Source);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        control.Items.RemoveAt(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        control.Items[index] = ((NavigationEntry)item).Source;
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    var oldIndex = e.OldStartingIndex;
                    index = e.NewStartingIndex;
                    var removedItem = control.Items[oldIndex];
                    control.Items.RemoveAt(oldIndex);
                    control.Items.Insert(index, removedItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    control.Items.Clear();
                    break;
            }
        }
    }
}