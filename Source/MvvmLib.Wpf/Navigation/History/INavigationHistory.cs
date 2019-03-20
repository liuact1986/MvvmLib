using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public interface INavigationHistory
    {
        Stack<NavigationEntry> BackStack { get; }
        NavigationEntry Current { get; }
        Stack<NavigationEntry> ForwardStack { get; }
        NavigationEntry Next { get; }
        NavigationEntry Previous { get; }
        NavigationEntry Root { get; }

        void Clear();
        NavigationEntry GoBack();
        NavigationEntry GoForward();
        void Navigate(NavigationEntry entry);
        void NavigateToRoot();
    }
}