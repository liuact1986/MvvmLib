using System;

namespace MvvmLib.Navigation
{
    public interface INavigationHistory
    {
        BindableList<NavigationEntry> BackStack { get; }
        NavigationEntry Current { get; }
        BindableList<NavigationEntry> ForwardStack { get; }
        NavigationEntry Next { get; }
        NavigationEntry Previous { get; }
        NavigationEntry Root { get; }

        event EventHandler CanGoBackChanged;
        event EventHandler CanGoForwardChanged;

        void Clear();
        NavigationEntry GoBack();
        NavigationEntry GoForward();
        void HandleGoBackChanged();
        void HandleGoForwardChanged();
        void Navigate(NavigationEntry navigationEntry);
        void NavigateToRoot();
        void UnhandleGoBackChanged();
        void UnhandleGoForwardChanged();
    }
}