using System;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    public interface ISharedSource: INotifyPropertyChanged
    {
        DeletionHandling DeletionHandling { get; set; }
        InsertionHandling InsertionHandling { get; set; }
        int SelectedIndex { get; set; }
        Type SourceType { get; }

        event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;
    }
}