using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests
{
    public class ViewA : UserControl
    {

    }

    public class ViewB : UserControl
    {

    }

    public class ViewC : UserControl
    {

    }

    public class ViewD : UserControl
    {

    }

    public class MyItemsControlRegionAdapter : IItemsRegionAdapter
    {
        public DependencyObject Control { get; set; }

        public void Adapt(ItemsRegion region)
        {

        }
    }

    public class StackPanelRegionAdapter : IItemsRegionAdapter
    {
        private StackPanel control;
        /// <summary>
        /// The control.
        /// </summary>
        public DependencyObject Control
        {
            get { return control; }
            set
            {
                if (value is StackPanel stackPanel)
                    control = stackPanel;
                else
                    throw new InvalidOperationException("Invalid control type");
            }
        }

        public void Adapt(ItemsRegion region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));
            if (control.Children.Count > 0)
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
                        ////control.Children.Insert(index, ((NavigationEntry)item).ViewOrObject);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        control.Children.RemoveAt(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        ////control.Children[index] = ((NavigationEntry)item).ViewOrObject;
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    var oldIndex = e.OldStartingIndex;
                    index = e.NewStartingIndex;
                    var removedItem = control.Children[oldIndex];
                    control.Children.RemoveAt(oldIndex);
                    control.Children.Insert(index, removedItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    control.Children.Clear();
                    break;
            }
        }
    }

}
