using System;

namespace MvvmLib.Navigation
{
    public interface IRegion
    {
        object Control { get; }
        string Name { get; }
        string RegionName { get; }

        event EventHandler<RegionNavigationEventArgs> Navigated;
        event EventHandler<RegionNavigationEventArgs> Navigating;
        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}