using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.History
{
    /// <summary>
    /// The history for NavigationSource.
    /// </summary>
    public interface INavigationHistory : INotifyPropertyChanged
    {
        /// <summary>
        /// The entry collection.
        /// </summary>
        IReadOnlyCollection<NavigationEntry> Entries { get; }

        /// <summary>
        /// The current index.
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// The first entry.
        /// </summary>
        NavigationEntry Root { get; }

        /// <summary>
        /// The previous entry.
        /// </summary>
        NavigationEntry Previous { get; }

        /// <summary>
        /// The current entry.
        /// </summary>
        NavigationEntry Current { get; }

        /// <summary>
        /// The next entry.
        /// </summary>
        NavigationEntry Next { get; }

        /// <summary>
        /// Chekcs if can go back.
        /// </summary>
        bool CanGoBack { get; }
        /// <summary>
        /// Chekcs if can go forward.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        event EventHandler<CanGoBackEventArgs> CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        event EventHandler<CanGoForwardEventArgs> CanGoForwardChanged;

        /// <summary>
        /// Invoked on current entry changed.
        /// </summary>
        event EventHandler<CurrentEntryChangedEventArgs> CurrentChanged;
    }
}