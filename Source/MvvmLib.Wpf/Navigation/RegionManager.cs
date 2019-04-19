using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to register content regions and items region with dependency properties, get region for navigation with RegionManager instance.
    /// </summary>
    public class RegionManager : IRegionManager
    {
        #region Static members

        internal static Dictionary<string, List<ContentRegion>> contentRegions { get; }

        internal static Dictionary<string, List<ItemsRegion>> itemsRegions { get; }

        public static int ContentRegionsCount => contentRegions.Count;
        public static int ItemsRegionsCount => itemsRegions.Count;

        public static void SetContentRegionName(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ContentRegionNameProperty, regionName);
        }

        public static string GetContentRegionName(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ContentRegionNameProperty) as string;
        }

        public static readonly DependencyProperty ContentRegionNameProperty =
             DependencyProperty.RegisterAttached("ContentRegionName", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetContentRegionName));

        private static void OnSetContentRegionName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var regionName = e.NewValue.ToString();
                var region = RegionManager.RegisterContentRegion(regionName, d);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((o, c) =>
                {
                    region.isLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

        public static void SetItemsRegionName(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ItemsRegionNameProperty, regionName);
        }

        public static string GetItemsRegionName(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ItemsRegionNameProperty) as string;
        }

        public static readonly DependencyProperty ItemsRegionNameProperty =
            DependencyProperty.RegisterAttached("ItemsRegionName", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetItemsRegionName));

        private static void OnSetItemsRegionName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var regionName = e.NewValue.ToString();
                var region = RegionManager.RegisterItemsRegion(regionName, d);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((o, c) =>
                {
                    region.isLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

        static RegionManager()
        {
            contentRegions = new Dictionary<string, List<ContentRegion>>();
            itemsRegions = new Dictionary<string, List<ItemsRegion>>();
        }

        public static bool ContainsContentRegionName(string regionName)
        {
            return contentRegions.ContainsKey(regionName);
        }

        public static bool ContainsItemsRegionName(string regionName)
        {
            return itemsRegions.ContainsKey(regionName);
        }

        public static ContentRegion RegisterContentRegion(string regionName, DependencyObject element)
        {
            if (!ContainsContentRegionName(regionName))
                contentRegions[regionName] = new List<ContentRegion>();

            var region = new ContentRegion(regionName, element);
            contentRegions[regionName].Add(region);

            return region;
        }

        public static ContentRegion GetContentRegionByName(string regionName, string controlName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (string.IsNullOrWhiteSpace(controlName)) { throw new Exception("Control name required"); }
            if (!ContainsContentRegionName(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            var regions = contentRegions[regionName];
            foreach (var region in regions)
            {
                if (region.ControlName == controlName)
                {
                    return region;
                }
            }
            return null;
        }

        internal static ContentRegion FindContentRegion(string regionName, DependencyObject child)
        {
            if (!ContainsContentRegionName(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            var regions = contentRegions[regionName];
            foreach (var region in regions)
            {
                if (region.Control == child)
                {
                    return region;
                }
            }
            return null;
        }

        public static ItemsRegion GetItemsRegionByName(string regionName, string controlName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (string.IsNullOrWhiteSpace(controlName)) { throw new Exception("Control name required"); }
            if (!ContainsItemsRegionName(regionName)) { throw new Exception("No items region registered with the region name \"" + regionName + "\""); }

            var regions = itemsRegions[regionName];
            foreach (var region in regions)
            {
                if (region.ControlName == controlName)
                {
                    return region;
                }
            }
            return null;
        }

        internal static ItemsRegion FindItemsRegion(string regionName, DependencyObject child)
        {
            if (!ContainsItemsRegionName(regionName)) { throw new Exception("No items region registered with the region name \"" + regionName + "\""); }

            var regions = itemsRegions[regionName];
            foreach (var region in regions)
            {
                if (region.Control == child)
                {
                    return region;
                }
            }
            return null;
        }

        public static ItemsRegion RegisterItemsRegion(string regionName, DependencyObject element)
        {
            if (!ContainsItemsRegionName(regionName))
            {
                itemsRegions[regionName] = new List<ItemsRegion>();
            }

            var region = new ItemsRegion(regionName, element);
            itemsRegions[regionName].Add(region);

            return region;
        }

        public static bool UnregisterContentRegion(ContentRegion contentRegion)
        {
            string regionName = contentRegion.RegionName;
            lock (contentRegions)
            {
                if (ContainsContentRegionName(regionName))
                {
                    contentRegions[regionName].Remove(contentRegion);
                    if (contentRegions[regionName].Count == 0)
                        contentRegions.Remove(regionName);
                    return true;
                }
            }
            return false;
        }

        public static bool UnregisterItemsRegion(ItemsRegion itemsRegion)
        {
            string regionName = itemsRegion.RegionName;
            lock (itemsRegions)
            {
                if (ContainsItemsRegionName(regionName))
                {
                    itemsRegions[regionName].Remove(itemsRegion);
                    if (itemsRegions[regionName].Count == 0)
                    {
                        itemsRegions.Remove(regionName);
                    }
                    return true;
                }
            }
            return false;
        }

        public static void ClearContentRegions()
        {
            contentRegions.Clear();
        }

        public static void ClearItemsRegions()
        {
            itemsRegions.Clear();
        }

        public static void ClearRegions()
        {
            ClearContentRegions();
            ClearItemsRegions();
        }

        internal static void RemoveNonLoadedContentRegions()
        {
            lock (contentRegions)
            {
                foreach (var regions in contentRegions.Values)
                {
                    var regionsToRemove = new List<ContentRegion>();
                    foreach (var region in regions)
                    {
                        if (!region.isLoaded)
                        {
                            regionsToRemove.Add(region);
                        }
                    }

                    if (regionsToRemove.Count > 0)
                    {
                        foreach (var region in regionsToRemove)
                        {
                            regions.Remove(region);
                        }
                    }
                }

            }
        }

        internal static void RemoveNonLoadedItemsRegions()
        {
            lock (itemsRegions)
            {
                foreach (var regions in itemsRegions.Values)
                {
                    var regionsToRemove = new List<ItemsRegion>();
                    foreach (var region in regions)
                    {
                        if (!region.isLoaded)
                        {
                            regionsToRemove.Add(region);
                        }
                    }

                    if (regionsToRemove.Count > 0)
                    {
                        foreach (var region in regionsToRemove)
                        {
                            regions.Remove(region);
                        }
                    }
                }
            }
        }

        internal static void RemoveNonLoadedRegions()
        {
            RemoveNonLoadedContentRegions();
            RemoveNonLoadedItemsRegions();
        }

        #endregion // Static members

        /// <summary>
        /// Gets the first content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new ArgumentNullException(nameof(regionName)); }

            if (ContainsContentRegionName(regionName))
            {
                var region = contentRegions[regionName].LastOrDefault();
                if (region == null) { throw new Exception("No region registered for the region name \"" + regionName + "\""); }

                return region;
            }
            throw new Exception("No content region registered for the region name \"" + regionName + "\"");
        }

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="name">The control name property</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName, string name)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new ArgumentNullException(nameof(regionName)); }
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }

            var region = GetContentRegionByName(regionName, name);
            if (region == null) { throw new Exception("No region found for the region name \"" + regionName + "\" with the name \"" + name + "\""); }
            return region;
        }

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="name">The control name property</param>
        /// <returns>The items region</returns>
        public ItemsRegion GetItemsRegion(string regionName, string name)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new ArgumentNullException(nameof(regionName)); }
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }

            var region = RegionManager.GetItemsRegionByName(regionName, name);
            if (region == null) { throw new Exception("No region found for the region name \"" + regionName + "\" with the name \"" + name + "\""); }
            return region;
        }

        /// <summary>
        /// Get the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        public ItemsRegion GetItemsRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new ArgumentNullException(nameof(regionName)); }

            if (ContainsItemsRegionName(regionName))
            {
                var region = RegionManager.itemsRegions[regionName].LastOrDefault();
                if (region == null) { throw new Exception("No region registered for the region name \"" + regionName + "\""); }
                return region;
            }
            throw new Exception("No items region registered for the region name \"" + regionName + "\"");
        }

        /// <summary>
        /// Get all content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of content regions</returns>
        public List<ContentRegion> GetContentRegions(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (!ContainsContentRegionName(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            return contentRegions[regionName];
        }

        /// <summary>
        /// Gets the items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        public List<ItemsRegion> GetItemsRegions(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new ArgumentNullException(nameof(regionName)); }
            if (!ContainsItemsRegionName(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            return itemsRegions[regionName];

        }
    }
}
