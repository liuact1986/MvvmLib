using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// History for content regions.
    /// </summary>
    public interface INavigationHistory
    {
        /// <summary>
        /// The back stack.
        /// </summary>
        BindableHistory<NavigationEntry> BackStack { get; }

        /// <summary>
        /// Gets the current entry. 
        /// </summary>
        NavigationEntry Current { get; }

        /// <summary>
        /// The forward Stack.
        /// </summary>
        BindableHistory<NavigationEntry> ForwardStack { get; }

        /// <summary>
        /// Gets the next entry. 
        /// </summary>
        NavigationEntry Next { get; }

        /// <summary>
        /// Gets the previous entry. 
        /// </summary>
        NavigationEntry Previous { get; }

        /// <summary>
        /// Gets the root entry. 
        /// </summary>
        NavigationEntry Root { get; }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        event EventHandler CanGoBackChanged;

        /// <summary>
        /// Invoked when the can go forward value changed.
        /// </summary>
        event EventHandler CanGoForwardChanged;

        /// <summary>
        /// Clear the history.
        /// </summary>
        void Clear();

        /// <summary>
        /// Moves back the history.
        /// </summary>
        /// <returns>The previous entry</returns>
        NavigationEntry GoBack();

        /// <summary>
        /// Moves forward the history.
        /// </summary>
        /// <returns>The next entry</returns>
        NavigationEntry GoForward();

        /// <summary>
        /// Handles go back changed.
        /// </summary>
        void HandleGoBackChanged();

        /// <summary>
        /// Handles go forward changed.
        /// </summary>
        void HandleGoForwardChanged();

        /// <summary>
        /// Moves to the the entry.
        /// </summary>
        /// <param name="navigationEntry">The new entry</param>
        void Navigate(NavigationEntry navigationEntry);

        /// <summary>
        /// Handles go back changed.
        /// </summary>
        void NavigateToRoot();

        /// <summary>
        /// Unhandles go back changed.
        /// </summary>
        void UnhandleGoBackChanged();

        /// <summary>
        /// Unhandles go forward changed.
        /// </summary>
        void UnhandleGoForwardChanged();
    }
}