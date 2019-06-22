using MvvmLib.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to handle the <see cref="Selector.SelectionChanged"/> event for a <see cref="Selector"/> and set <see cref="IIsSelected"/> for items collection.
    /// </summary>
    public class SelectionSyncBehavior : NavigationBehavior
    {
        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="SelectionSyncBehavior"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new SelectionSyncBehavior();
        }

        /// <summary>
        /// Handles the <see cref="Selector.SelectionChanged"/> event to set <see cref="IIsSelected"/> for items that implements the interface.
        /// </summary>
        protected override void OnAttach()
        {
            if (!IsInDesignMode(this))
            {
                CheckAssociatedObjectType();
                ((Selector)associatedObject).SelectionChanged += OnSelectionChanged;
            }
        }

        /// <summary>
        /// Unhandles the <see cref="Selector.SelectionChanged"/> event for the selector.
        /// </summary>
        protected override void OnDetach()
        {
            if (associatedObject != null)
            {
                CheckAssociatedObjectType();
                ((Selector)associatedObject).SelectionChanged -= OnSelectionChanged;
            }
        }

        private void CheckAssociatedObjectType()
        {
            if (!(associatedObject is Selector))
                throw new InvalidOperationException($"Selector (ListBox, TabControl, etc.) is expected for the SelectionSyncBehavior. Current \"{associatedObject.GetType().Name}\"");
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // inactive
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

            // active
            foreach (var selectedItem in e.AddedItems)
            {
                foreach (var item in items)
                {
                    if (item == selectedItem)
                        TrySetSelected(item, true);
                }
            }
        }

        private void TrySetSelected(object item, bool isSelected)
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
