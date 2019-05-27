using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to handle the <see cref="Selector.SelectionChanged"/> event for a <see cref="Selector"/> and set <see cref="IIsSelected"/> for items collection.
    /// </summary>
    public class SelectionChangedBehavior : NavigationBehavior
    {
        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="SelectionChangedBehavior"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new SelectionChangedBehavior();
        }

        /// <summary>
        /// Handles the <see cref="Selector.SelectionChanged"/> event to set <see cref="IIsSelected"/> for items that implements the interface.
        /// </summary>
        protected override void OnAttach()
        {
            CheckAssociatedObjectType();

            ((Selector)associatedObject).SelectionChanged += OnSelectionChanged;
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
                throw new InvalidOperationException($"Selector (ListBox, TabControl, etc.) is expected for the SelectionChangedBehavior. Current \"{associatedObject.GetType().Name}\"");
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
                        {
                            if (item is IIsSelected)
                                ((IIsSelected)item).IsSelected = false;
                        }
                    }
                }
            }

            // active
            foreach (var selectedItem in e.AddedItems)
            {
                foreach (var item in items)
                {
                    // item => view or view model
                    if (item == selectedItem)
                    {
                        if (item is IIsSelected context)
                            ((IIsSelected)item).IsSelected = true;
                    }
                }
            }
        }
    }

}
