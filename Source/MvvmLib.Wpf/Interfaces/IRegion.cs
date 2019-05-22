using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region contract.
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        /// The control for this region.
        /// </summary>
        FrameworkElement Control { get; }

        /// <summary>
        /// The name of the control. Can be used to get a region by region name and control name.
        /// </summary>
        string ControlName { get; }

        /// <summary>
        /// The region name.
        /// </summary>
        string RegionName { get; }

        ///// <summary>
        ///// Gets the current entry of history.
        ///// </summary>
        //NavigationEntry CurrentEntry { get; }

        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        event EventHandler<RegionNavigationEventArgs> Navigated;

        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        event EventHandler<RegionNavigationEventArgs> Navigating;

        /// <summary>
        /// Invoked on navigation cancelled or on exception.
        /// </summary
        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}