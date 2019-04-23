using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The registry for content and items regions.
    /// </summary>
    public class RegionsRegistry
    {
        private readonly ILogger DefaultLogger = new DebugLogger();

        private ILogger logger;

        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        private readonly Dictionary<string, List<ContentRegion>> contentRegions = new Dictionary<string, List<ContentRegion>>();

        /// <summary>
        /// The content regions list.
        /// </summary>
        public Dictionary<string, List<ContentRegion>> ContentRegions
        {
            get { return contentRegions; }
        }

        private readonly Dictionary<string, List<ItemsRegion>> itemsRegions = new Dictionary<string, List<ItemsRegion>>();

        /// <summary>
        /// Invoked on region created.
        /// </summary>
        public event EventHandler<RegionRegisteredEventArgs> RegionRegistered;

        /// <summary>
        /// Invoked on region removed.
        /// </summary>
        public event EventHandler<RegionUnregisteredEventArgs> RegionUnregistered;

        /// <summary>
        /// The items regions list.
        /// </summary>
        public Dictionary<string, List<ItemsRegion>> ItemsRegions
        {
            get { return itemsRegions; }
        }

        private static readonly RegionsRegistry instance = new RegionsRegistry();

        /// <summary>
        /// The default static instance of the regions registry.
        /// </summary>
        public static RegionsRegistry Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Registers a content region.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <returns>The region created</returns>
        public ContentRegion RegisterContentRegion(string regionName, FrameworkElement control)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (!contentRegions.ContainsKey(regionName))
                contentRegions[regionName] = new List<ContentRegion>();

            var region = new ContentRegion(regionName, control, this);
            contentRegions[regionName].Add(region);

            var eventArgs = new RegionRegisteredEventArgs(regionName, region);
            RegionRegistered?.Invoke(this, eventArgs);

            Logger.Log($"Content region \"{regionName}\" created", Category.Info, Priority.Low);

            return region;
        }

        /// <summary>
        /// Registers an items region.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <returns>The region created</returns>
        public ItemsRegion RegisterItemsRegion(string regionName, FrameworkElement control)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (!ItemsRegions.ContainsKey(regionName))
                ItemsRegions[regionName] = new List<ItemsRegion>();

            var region = new ItemsRegion(regionName, control, this);
            ItemsRegions[regionName].Add(region);

            var eventArgs = new RegionRegisteredEventArgs(regionName, region);
            RegionRegistered?.Invoke(this, eventArgs);

            Logger.Log($"Items region \"{regionName}\" created", Category.Info, Priority.Low);

            return region;
        }

        /// <summary>
        /// Gets the last content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region or null</returns>
        public ContentRegion GetContentRegion(string regionName)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));

            if (contentRegions.TryGetValue(regionName, out List<ContentRegion> candidates))
            {
                if (candidates.Count > 0)
                {
                    var region = candidates.LastOrDefault();
                    return region;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The content region  found or null</returns>
        public ContentRegion GetContentRegion(string regionName, string controlName)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (controlName == null)
                throw new ArgumentNullException(nameof(controlName));

            if (contentRegions.TryGetValue(regionName, out List<ContentRegion> candidates))
            {
                var region = candidates.FirstOrDefault(c => c.ControlName == controlName);
                return region;
            }
            return null;
        }

        /// <summary>
        /// Gets the content region for the region name and child.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control to check</param>
        /// <returns>The content region found or null</returns>
        public ContentRegion GetContentRegion(string regionName, DependencyObject control)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (contentRegions.TryGetValue(regionName, out List<ContentRegion> candidates))
            {
                var region = candidates.FirstOrDefault(c => c.Control == control);
                return region;
            }
            return null;
        }

        /// <summary>
        /// Gets the last items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The items region found or null</returns>
        public ItemsRegion GetItemsRegion(string regionName)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));

            if (itemsRegions.TryGetValue(regionName, out List<ItemsRegion> candidates))
            {
                if (candidates.Count > 0)
                {
                    var region = candidates.LastOrDefault();
                    return region;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name property</param>
        /// <returns>The items region found or null</returns>
        public ItemsRegion GetItemsRegion(string regionName, string controlName)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (controlName == null)
                throw new ArgumentNullException(nameof(controlName));


            if (itemsRegions.TryGetValue(regionName, out List<ItemsRegion> candidates))
            {
                var region = candidates.FirstOrDefault(c => c.ControlName == controlName);
                return region;
            }
            return null;
        }

        /// <summary>
        /// Gets the items region for the region name and child.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control to check</param>
        /// <returns>The content region found or null</returns>
        public ItemsRegion GetItemsRegion(string regionName, DependencyObject control)
        {
            if (regionName == null)
                throw new ArgumentNullException(nameof(regionName));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (itemsRegions.TryGetValue(regionName, out List<ItemsRegion> candidates))
            {
                var region = candidates.FirstOrDefault(c => c.Control == control);
                return region;
            }
            return null;
        }

        /// <summary>
        /// Unregister the content region.
        /// </summary>
        /// <param name="contentRegion">The content region</param>
        /// <returns>True if region removed successfully</returns>
        public bool UnregisterContentRegion(ContentRegion contentRegion)
        {
            if (contentRegion == null)
                throw new ArgumentNullException(nameof(contentRegion));

            string regionName = contentRegion.RegionName;
            lock (contentRegions)
            {
                if (contentRegions.ContainsKey(regionName))
                {
                    contentRegions[regionName].Remove(contentRegion);
                    if (contentRegions[regionName].Count == 0)
                        contentRegions.Remove(regionName);

                    var eventArgs = new RegionUnregisteredEventArgs(regionName);
                    RegionUnregistered?.Invoke(this, eventArgs);

                    Logger.Log($"Content region \"{regionName}\" unregistered", Category.Info, Priority.Low);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unregister the iems region.
        /// </summary>
        /// <param name="itemsRegion">The items region</param>
        /// <returns>True if region removed successfully</returns>
        public bool UnregisterItemsRegion(ItemsRegion itemsRegion)
        {
            if (itemsRegion == null)
                throw new ArgumentNullException(nameof(itemsRegion));

            string regionName = itemsRegion.RegionName;
            lock (itemsRegions)
            {
                if (itemsRegions.ContainsKey(regionName))
                {
                    itemsRegions[regionName].Remove(itemsRegion);
                    if (itemsRegions[regionName].Count == 0)
                        itemsRegions.Remove(regionName);

                    var eventArgs = new RegionUnregisteredEventArgs(regionName);
                    RegionUnregistered?.Invoke(this, eventArgs);

                    Logger.Log($"Items region \"{regionName}\" unregistered", Category.Info, Priority.Low);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove all content regions not loaded.
        /// </summary>
        public void RemoveNonLoadedContentRegions()
        {
            lock (contentRegions)
            {
                foreach (var regions in contentRegions.Values)
                {
                    var regionsToRemove = new List<ContentRegion>();
                    foreach (var region in regions)
                        if (!region.isLoaded)
                            regionsToRemove.Add(region);

                    if (regionsToRemove.Count > 0)
                        foreach (var region in regionsToRemove)
                        {
                            string regionName = region.RegionName;
                            regions.Remove(region);
                            Logger.Log($"Content region \"{regionName}\" (not loaded) unregistered", Category.Info, Priority.Low);
                        }
                }
            }
        }

        /// <summary>
        /// Remove all items regions not loaded.
        /// </summary>
        public void RemoveNonLoadedItemsRegions()
        {
            lock (itemsRegions)
            {
                foreach (var regions in itemsRegions.Values)
                {
                    var regionsToRemove = new List<ItemsRegion>();
                    foreach (var region in regions)
                        if (!region.isLoaded)
                            regionsToRemove.Add(region);

                    if (regionsToRemove.Count > 0)
                        foreach (var region in regionsToRemove)
                        {
                            string regionName = region.RegionName;
                            regions.Remove(region);
                            Logger.Log($"Items region \"{regionName}\" (not loaded) unregistered", Category.Info, Priority.Low);
                        }
                }
            }
        }

        /// <summary>
        /// Remove all regions not loaded.
        /// </summary>
        public void RemoveNonLoadedRegions()
        {
            RemoveNonLoadedContentRegions();
            RemoveNonLoadedItemsRegions();
        }
    }
}
