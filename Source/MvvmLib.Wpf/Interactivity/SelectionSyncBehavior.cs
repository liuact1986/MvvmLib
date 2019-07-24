using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to handle the <see cref="Selector.SelectionChanged"/> event for a <see cref="Selector"/> and set <see cref="IIsSelected"/> for items collection.
    /// </summary>
    public class SelectionSyncBehavior : Behavior
    {
        /// <summary>
        /// Creates the <see cref="SelectionSyncBehavior"/>.
        /// </summary>
        /// <returns>The Freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new SelectionSyncBehavior();
        }

        /// <summary>
        /// Handles the <see cref="Selector.SelectionChanged"/> event to set <see cref="IIsSelected"/> for items that implements the interface.
        /// </summary>
        protected override void OnAttach()
        {
            if (!IsSelector())
                throw new InvalidOperationException($"Selector (ListBox, TabControl, etc.) is expected for the SelectionSyncBehavior. Current \"{associatedObject.GetType().Name}\"");

            EvaluateAllItems();
            ((Selector)associatedObject).SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// Unhandles the <see cref="Selector.SelectionChanged"/> event for the selector.
        /// </summary>
        protected override void OnDetach()
        {
            if (associatedObject != null)
            {
                ((Selector)associatedObject).SelectionChanged -= OnSelectionChanged;
            }
        }

        /// <summary>
        /// Checks if the AssociatedObject is Selector.
        /// </summary>
        /// <returns></returns>
        protected bool IsSelector()
        {
            return associatedObject is Selector;
        }

        /// <summary>
        /// Sets IsSelected for changes.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The selection changed event args</param>
        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // non selected items
            var items = ((Selector)associatedObject).Items;
            foreach (var removedItem in e.RemovedItems)
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        if (item == removedItem)
                            TrySetSelected(item, false);
                    }
                }
            }

            // selected items
            foreach (var addedItem in e.AddedItems)
            {
                foreach (var item in items)
                {
                    if (item == addedItem)
                        TrySetSelected(item, true);
                }
            }
        }

        /// <summary>
        /// Checks all items for AssociatedObject.
        /// </summary>
        protected virtual void EvaluateAllItems()
        {
            var items = ((Selector)associatedObject).Items;
            var selectedItems = associatedObject is ListBox ?
                ((ListBox)associatedObject).SelectedItems 
                : new List<object> { ((Selector)associatedObject).SelectedItem };

            foreach (var item in items)
            {
                if (item != null)
                {
                    if (selectedItems.Contains(item))
                    {
                        TrySetSelected(item, true);
                    }
                    else
                    {
                        TrySetSelected(item, false);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to set selected.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="isSelected">Is Selected</param>
        protected void TrySetSelected(object item, bool isSelected)
        {
            var view = item as FrameworkElement;
            if (view != null)
            {
                if (view.DataContext is IIsSelected)
                    ((IIsSelected)view.DataContext).IsSelected = isSelected;
            }
            else
            {
                if (item is IIsSelected)
                    ((IIsSelected)item).IsSelected = isSelected;
            }
        }
    }
}
