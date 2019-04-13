using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to get region for navigation with RegionManager instance.
    /// </summary>
    public interface IRegionManager
    {
        /// <summary>
        /// Gets the first content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName);

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="name">The control name property</param>
        /// <returns>The content region</returns>
        ContentRegion GetContentRegion(string regionName, string name);

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="name">The control name property</param>
        /// <returns>The items region</returns>
        ItemsRegion GetItemsRegion(string regionName, string name);

        /// <summary>
        /// Get the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        ItemsRegion GetItemsRegion(string regionName);

        /// <summary>
        /// Get all content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of content regions</returns>
        List<ContentRegion> GetContentRegions(string regionName);

        /// <summary>
        /// Gets the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        List<ItemsRegion> GetItemsRegions(string regionName);
    }
}