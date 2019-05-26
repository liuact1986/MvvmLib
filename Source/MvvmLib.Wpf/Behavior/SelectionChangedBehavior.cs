using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to handle the <see cref="Selector.SelectionChanged"/> event for a <see cref="Selector"/> and set <see cref="IIsSelected"/> for items collection.
    /// </summary>
    public class SelectionChangedBehavior : NavigationBehavior, IAssociatedObject
    {
        private Selector selector;
        /// <summary>
        /// The <see cref="Selector"/> (ListBox, TabControl, etc.).
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get { return selector; }
            set { selector = value as Selector; }
        }

        /// <summary>
        /// Handle the <see cref="Selector.SelectionChanged"/> event to set <see cref="IIsSelected"/> for items that implements the interface.
        /// </summary>
        protected override void OnAttach()
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(AssociatedObject));

            selector.SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// Unhandle the <see cref="Selector.SelectionChanged"/> event for the selector.
        /// </summary>
        protected override void OnDetach()
        {
            if (selector != null)
                selector.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // inactive
            var items = selector.Items;
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
