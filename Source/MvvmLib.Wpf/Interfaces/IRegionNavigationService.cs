using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to navigate and insert sources for content and items regions.
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

        /// <summary>
        /// Navigates to the source for the content region.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> NavigateAsync(string contentRegionName, Type sourceType, object parameter);

        /// <summary>
        /// Navigates to the source for the content region.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> NavigateAsync(string contentRegionName, Type sourceType);

        /// <summary>
        /// Redirects to the source and remove the previous entry from history.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The type of the view or view model to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> RedirectAsync(string contentRegionName, Type sourceType, object parameter);
        /// <summary>
        /// Redirect to the vsource and remove the previous entry from history.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The type of the view or view model to redirect</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> RedirectAsync(string contentRegionName, Type sourceType);

        /// <summary>
        /// Navigates to the previous entry.
        /// </summary>
        /// <param name="contentRegionName"></param>
        /// <returns></returns>
        Task<bool> GoBackAsync(string contentRegionName);

        /// <summary>
        /// Navigates to the next entry.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> GoForward(string contentRegionName);

        /// <summary>
        /// Navigates to the root entry.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <returns>True on navigation  success</returns>
        Task<bool> NavigateToRootAsync(string contentRegionName);

        /// <summary>
        /// Inserts a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if inserted</returns>
        Task<bool> InsertAsync(string itemsRegionName, int index, Type sourceType, object parameter);

        /// <summary>
        /// Inserts the source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if inserted</returns>
        Task<bool> InsertAsync(string itemsRegionName, int index, Type sourceType);

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if added</returns>
        Task<bool> AddAsync(string itemsRegionName, Type sourceType, object parameter);

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if added</returns>
        Task<bool> AddAsync(string itemsRegionName, Type sourceType);

        /// <summary>
        /// Removes the entry from the items region history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <returns>True if removed</returns>
        Task<bool> RemoveAtAsync(string itemsRegionName, int index);

        /// <summary>
        /// Tries to find the source and remove the entry from history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="source">The source</param>
        /// <returns>True if removed</returns>
        Task<bool> RemoveAsync(string itemsRegionName, object source);

        /// <summary>
        /// Clears the region history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <returns>True if success</returns>
        Task<bool> Clear(string itemsRegionName);
    }
}