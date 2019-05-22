using MvvmLib.Animation;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region Adapter for <see cref="TransitioningItemsControl"/>.
    /// </summary>
    public class TransitioningItemsControlRegionAdapter : IItemsRegionAdapter
    {
        private TransitioningItemsControl control;
        /// <summary>
        /// The control.
        /// </summary>
        public DependencyObject Control
        {
            get { return control; }
            set
            {
                if (value is TransitioningItemsControl transitioningItemsControl)
                    control = transitioningItemsControl;
                else
                    throw new InvalidOperationException("Invalid control type");
            }
        }

        /// <summary>
        /// Creates the items region adapter.
        /// </summary>
        /// <param name="control">The items control</param>
        public TransitioningItemsControlRegionAdapter(TransitioningItemsControl control)
        {
            this.control = control ?? throw new ArgumentNullException(nameof(control));
        }

        /// <summary>
        /// Allows to bind control to region.
        /// </summary>
        /// <param name="region">The items region</param>
        public void Adapt(ItemsRegion region)
        {
            if (control.ItemsSource != null || (BindingOperations.GetBinding(control, ItemsControl.ItemsSourceProperty) != null))
                throw new InvalidOperationException("The ItemsSource is set");

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
                        control.InsertItemOrEnqueue(index, ((NavigationEntry)item).Source);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        control.RemoveItemOrEnqueue(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        control.SetItemOrEnqueue(index, ((NavigationEntry)item).Source);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO: Implement Move for TransitioningItemsControl
                    throw new NotSupportedException("Move is not supported by the TransitioningItemsControl");
                case NotifyCollectionChangedAction.Reset:
                    control.ClearItemsOrEnqueue();
                    break;
            }
        }
    }
}