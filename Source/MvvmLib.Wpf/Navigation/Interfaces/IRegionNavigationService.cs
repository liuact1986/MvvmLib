using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to get content and items regions and navigate.
    /// </summary>
    public interface IRegionNavigationService
    {
        /// <summary>
        /// Gets the last content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName);

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName, string controlName);

        /// <summary>
        /// Gets the content regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of content regions</returns>
        IReadOnlyList<ContentRegion> GetContentRegions(string regionName);

        /// <summary>
        /// Get the last items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        ItemsRegion GetItemsRegion(string regionName);

        /// <summary>
        /// Gets the items regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of items regions</returns>
         IReadOnlyList<ItemsRegion> GetItemsRegions(string regionName);

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>The items region</returns>
        ItemsRegion GetItemsRegion(string regionName, string controlName);

        /// <summary>
        /// Checks if the content region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>True if discovered</returns>
        bool IsContentRegionDiscovered(string regionName);

        /// <summary>
        /// Checks if the content region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>True if discovered</returns>
        bool IsContentRegionDiscovered(string regionName, string controlName);

        /// <summary>
        /// Checks if the items region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>True if discovered</returns>
        bool IsItemsRegionDiscovered(string regionName);

        /// <summary>
        /// Checks if the items region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>True if discovered</returns>
        bool IsItemsRegionDiscovered(string regionName, string controlName);

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="controlName">The control name</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        Task<bool> NavigateWhenAvailableAsync(string regionName, Type sourceType, string controlName, object parameter, Action onCompleted = null);

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        Task<bool> NavigateWhenAvailable(string regionName, string controlName, Type sourceType, Action onCompleted = null);

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        Task<bool> NavigateWhenAvailable(string regionName, Type sourceType, object parameter, Action onCompleted = null);

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        Task<bool> NavigateWhenAvailable(string regionName, Type sourceType, Action onCompleted = null);
    }
}