using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to get content and items regions and navigate.
    /// </summary>
    public interface IRegionNavigationService
    {
        /// <summary>
        /// Gets the first content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName);

        /// <summary>
        /// Gets the content regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of content regions</returns>
        IReadOnlyList<ContentRegion> GetContentRegions(string regionName);

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName, string controlName);

        /// <summary>
        /// Get the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        ItemsRegion GetItemsRegion(string regionName);

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The items region</returns>
        ItemsRegion GetItemsRegion(string regionName, string controlName);

        /// <summary>
        /// Gets the items regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of items regions</returns>
        IReadOnlyList<ItemsRegion> GetItemsRegions(string regionName);
    }
}
