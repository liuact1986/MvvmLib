using System;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    public interface ISharedSource: INotifyPropertyChanged
    {
        DeletionHandling DeletionHandling { get; set; }
        InsertionHandling InsertionHandling { get; set; }
        Type SourceType { get; }

        event PropertyChangedEventHandler PropertyChanged;
        event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;
    }
}