using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    public interface IRegion
    {
        FrameworkElement Control { get; }
        string ControlName { get; }
        string RegionName { get; }
        NavigationEntry CurrentEntry { get; }

        event EventHandler<RegionNavigationEventArgs> Navigated;
        event EventHandler<RegionNavigationEventArgs> Navigating;
        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}