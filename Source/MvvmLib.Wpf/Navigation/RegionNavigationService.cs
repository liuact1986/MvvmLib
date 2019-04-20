using System.Collections.Generic;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Allows to get content and items regions and navigate.
    /// </summary>
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly RegionsRegistry regionsRegistry;

        /// <summary>
        /// Creates the regions navigation service.
        /// </summary>
        public RegionNavigationService()
            :this(RegionsRegistry.Instance)
        { }

        /// <summary>
        /// Creates the regions navigation service.
        /// </summary>
        /// <param name="regionsRegistry">The regions registry used by the navigation service</param>
        public RegionNavigationService(RegionsRegistry regionsRegistry)
        {
            this.regionsRegistry = regionsRegistry;
        }

        /// <summary>
        /// Gets the first content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName)
        {
            var region = regionsRegistry.GetContentRegion(regionName);
            if(region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");

            return region;
        }

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName, string controlName)
        {
            var region = regionsRegistry.GetContentRegion(regionName, controlName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\" and control name \"{controlName}\"");

            return region;
        }

        /// <summary>
        /// Gets the content regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of content regions</returns>
        public IReadOnlyList<ContentRegion> GetContentRegions(string regionName)
        {
            if(regionsRegistry.ContentRegions.TryGetValue(regionName, out List<ContentRegion> regions))
            {
                var readOnlyRegions = regions.AsReadOnly();
                return readOnlyRegions;
            }
            throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");
        }

        /// <summary>
        /// Get the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        public ItemsRegion GetItemsRegion(string regionName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");

            return region;
        }

        /// <summary>
        /// Gets the items regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of items regions</returns>
        public IReadOnlyList<ItemsRegion> GetItemsRegions(string regionName)
        {
            if (regionsRegistry.ItemsRegions.TryGetValue(regionName, out List<ItemsRegion> regions))
            {
                var readOnlyRegions = regions.AsReadOnly();
                return readOnlyRegions;
            }
            throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");
        }

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The items region</returns>
        public ItemsRegion GetItemsRegion(string regionName, string controlName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName, controlName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\" and control name \"{controlName}\"");

            return region;
        }
    }
}
