using System;

namespace MvvmLib.Navigation
{
    public interface IRegion
    {
        IAnimatedContentStrategy Animation { get; }
        object Control { get; }
        NavigationEntry CurrentEntry { get; }
        bool IsLoaded { get; }
        string Name { get; }
        string RegionName { get; }

        event EventHandler<RegionNavigationEventArgs> Navigated;
        event EventHandler<RegionNavigationEventArgs> Navigating;
        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}