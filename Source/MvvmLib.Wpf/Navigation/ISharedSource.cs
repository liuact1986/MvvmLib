using System;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Source for Models and ViewModels with a collection of Items and SelectedItem/SelectedIndex. 
    /// It supports Views but its not the target. 
    /// This is the source for ItemsControls, Selectors (ListBox, TabControl), etc.
    /// </summary>
    public interface ISharedSource: INotifyPropertyChanged
    {
        /// <summary>
        /// Allows to select inserted item (Select by default).
        /// </summary>
        SelectionHandling SelectionHandling { get; set; }
        /// <summary>
        /// The selected index.
        /// </summary>
        int SelectedIndex { get; set; }
        /// <summary>
        /// The type of the items.
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        /// Invoked on selected item changed.
        /// </summary>
        event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;
    }
}