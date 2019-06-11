using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    public interface INavigationHistory : INotifyPropertyChanged
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        NavigationEntry Current { get; }
        int CurrentIndex { get; }
        IReadOnlyCollection<NavigationEntry> Entries { get; }
        NavigationEntry Next { get; }
        NavigationEntry Previous { get; }
        NavigationEntry Root { get; }

        event EventHandler<CanGoBackEventArgs> CanGoBackChanged;
        event EventHandler<CanGoForwardEventArgs> CanGoForwardChanged;
        event EventHandler<CurrentEntryChangedEventArgs> CurrentChanged;
        event PropertyChangedEventHandler PropertyChanged;
    }
}