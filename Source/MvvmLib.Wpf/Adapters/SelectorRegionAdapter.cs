using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region Adapter for <see cref="Selector"/>.
    /// </summary>
    public class SelectorRegionAdapter : IItemsRegionAdapter
    {
        private Selector control;
        private ItemsRegion region;

        /// <summary>
        /// The control.
        /// </summary>
        public DependencyObject Control
        {
            get { return control; }
            set
            {
                if (value is Selector selector)
                    control = selector;
                else
                    throw new InvalidOperationException("Invalid control type");
            }
        }

        /// <summary>
        /// Creates the selector region adapter.
        /// </summary>
        /// <param name="control">The items control</param>
        public SelectorRegionAdapter(Selector control)
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

            this.region = region;

            region.History.Entries.CollectionChanged += OnEntriesCollectionChanged;

            control.SelectionChanged += OnControlSelectionChanged;

            region.History.CurrentChanged += OnHistoryCurrentChanged;
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

        private void OnControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // inactive
            foreach (var removedItem in e.RemovedItems)
            {
                foreach (var entry in region.History.Entries)
                {
                    if (entry.Context != null)
                    {
                        if (entry.Context == removedItem || entry.Source == removedItem)
                        {
                            if (entry.Context is IIsSelected context)
                                context.IsSelected = false;
                        }
                    }
                    else if (entry.Source == removedItem)
                    {
                        if (entry.Source is IIsSelected source)
                            source.IsSelected = false;
                    }
                }
            }

            // active
            foreach (var selectedItem in e.AddedItems)
            {
                foreach (var entry in region.History.Entries)
                {
                    // item => view or view model
                    if (entry.Context != null)
                    {
                        // view (source) + view model (context)
                        if (entry.Context == selectedItem || entry.Source == selectedItem)
                        {
                            if (entry.Context is IIsSelected context)
                                context.IsSelected = true;
                        }
                    }
                    else if (entry.Source == selectedItem)
                    {
                        if (entry.Source is IIsSelected source)
                            source.IsSelected = true;
                    }
                }
            }

            handleCurrentIndexChanged = false;
            region.History.Select(control.SelectedIndex);
        }

        private bool handleCurrentIndexChanged = true;

        private void OnHistoryCurrentChanged(object sender, IndexedNavigationEntryEventArgs e)
        {
            if (!handleCurrentIndexChanged)
                handleCurrentIndexChanged = true;
            else if (handleCurrentIndexChanged && region.SynchronizationMode == SynchronizationHandling.SynchronizeControlWithCurrentIndexOfHistory)
                control.SelectedIndex = e.Index;
        }
    }
}