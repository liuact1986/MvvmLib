using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class RegionManager : IRegionManager
    {
        internal static Dictionary<string, List<ContentRegion>> contentRegions { get; }

        internal static Dictionary<string, List<ItemsRegion>> itemsRegions { get; }

        public static int ContentRegionsCount => contentRegions.Count;

        public static int ItemsRegionsCount => itemsRegions.Count;

        public static readonly DependencyProperty ContentRegionProperty =
             DependencyProperty.RegisterAttached("ContentRegion", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetContentRegion));

        public static void SetContentRegion(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ContentRegionProperty, regionName);
        }

        public static string GetContentRegion(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ContentRegionProperty) as string;
        }

        private static void OnSetContentRegion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var regionName = e.NewValue.ToString();
                var region = RegionManager.RegisterContentRegion(regionName, d);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((o, c) =>
                {
                    region.IsLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

        public static readonly DependencyProperty ItemsRegionProperty =
            DependencyProperty.RegisterAttached("ItemsRegion", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetItemsRegion));

        public static void SetItemsRegion(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ItemsRegionProperty, regionName);
        }

        public static string GetItemsRegion(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ItemsRegionProperty) as string;
        }

        private static void OnSetItemsRegion(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var regionName = e.NewValue.ToString();
                var region = RegionManager.RegisterItemsRegion(regionName, d);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((o, c) =>
                {
                    region.IsLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

        static RegionManager()
        {
            contentRegions = new Dictionary<string, List<ContentRegion>>();
            itemsRegions = new Dictionary<string, List<ItemsRegion>>();
        }

        public static bool IsContentRegionNameRegistered(string regionName)
        {
            return contentRegions.ContainsKey(regionName);
        }

        public static bool IsItemsRegionNameRegistered(string regionName)
        {
            return itemsRegions.ContainsKey(regionName);
        }

        public static ContentRegion RegisterContentRegion(string regionName, DependencyObject element)
        {
            if (!IsContentRegionNameRegistered(regionName))
            {
                contentRegions[regionName] = new List<ContentRegion>();
            }

            var region = new ContentRegion(regionName, element);
            contentRegions[regionName].Add(region);

            return region;
        }

        public static ContentRegion GetContentRegionByName(string regionName, string controlName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (string.IsNullOrWhiteSpace(controlName)) { throw new Exception("Control name required"); }
            if (!IsContentRegionNameRegistered(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            var regions = contentRegions[regionName];
            foreach (var region in regions)
            {
                if (region.Name == controlName)
                {
                    return region;
                }
            }
            return null;
        }

        internal static ContentRegion FindContentRegion(string regionName, DependencyObject child)
        {
            if (!IsContentRegionNameRegistered(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

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
            if (!IsItemsRegionNameRegistered(regionName)) { throw new Exception("No items region registered with the region name \"" + regionName + "\""); }

            var regions = itemsRegions[regionName];
            foreach (var region in regions)
            {
                if (region.Name == controlName)
                {
                    return region;
                }
            }
            return null;
        }

        internal static ItemsRegion FindItemsRegion(string regionName, DependencyObject child)
        {
            if (!IsItemsRegionNameRegistered(regionName)) { throw new Exception("No items region registered with the region name \"" + regionName + "\""); }

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
            if (!IsItemsRegionNameRegistered(regionName))
            {
                itemsRegions[regionName] = new List<ItemsRegion>();
            }

            var region = new ItemsRegion(regionName, element);
            itemsRegions[regionName].Add(region);

            return region;
        }

        public static bool UnregisterContentRegions(string regionName)
        {
            if (IsContentRegionNameRegistered(regionName))
            {
                contentRegions.Remove(regionName);
                return true;
            }
            return false;
        }

        public static bool UnregisterItemsRegions(string regionName)
        {
            if (IsItemsRegionNameRegistered(regionName))
            {
                itemsRegions.Remove(regionName);
                return true;
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

        /// <summary>
        /// Returns the first content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns></returns>
        public ContentRegion GetContentRegion(string regionName)
        {
            if (RegionManager.IsContentRegionNameRegistered(regionName))
            {
                var region = RegionManager.contentRegions[regionName].LastOrDefault();
                if (region == null) { throw new Exception("No region registered for the region name \"" + regionName + "\""); }
                return region;
            }
            throw new Exception("No content region registered for the region name \"" + regionName + "\"");
        }

        /// <summary>
        /// Returns the content region.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="name">The control name property</param>
        /// <returns></returns>
        public ContentRegion GetContentRegion(string regionName, string name)
        {
            var region = RegionManager.GetContentRegionByName(regionName, name);
            if (region == null) { throw new Exception("No region found for the region name \"" + regionName + "\" with the name \"" + name + "\""); }
            return region;
        }

        public List<ContentRegion> GetContentRegions(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (!IsContentRegionNameRegistered(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            return contentRegions[regionName];
        }

        public ItemsRegion GetItemsRegion(string regionName)
        {
            if (RegionManager.IsItemsRegionNameRegistered(regionName))
            {
                var region = RegionManager.itemsRegions[regionName].LastOrDefault();
                if (region == null) { throw new Exception("No region registered for the region name \"" + regionName + "\""); }
                return region;
            }
            throw new Exception("No items region registered for the region name \"" + regionName + "\"");
        }

        public ItemsRegion GetItemsRegion(string regionName, string name)
        {
            var region = RegionManager.GetItemsRegionByName(regionName, name);
            if (region == null) { throw new Exception("No region found for the region name \"" + regionName + "\" with the name \"" + name + "\""); }
            return region;
        }

        public List<ItemsRegion> GetItemsRegions(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName)) { throw new Exception("Region name required"); }
            if (!IsItemsRegionNameRegistered(regionName)) { throw new Exception("No content region registered with the region name \"" + regionName + "\""); }

            return itemsRegions[regionName];

        }

        public static void CleanContentRegions()
        {
            lock (contentRegions)
            {
                foreach (var regions in contentRegions.Values)
                {
                    var regionsToRemove = new List<ContentRegion>();
                    foreach (var region in regions)
                    {
                        if (!region.IsLoaded)
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

        public static void CleanItemsRegions()
        {
            lock (itemsRegions)
            {
                foreach (var regions in itemsRegions.Values)
                {
                    var regionsToRemove = new List<ItemsRegion>();
                    foreach (var region in regions)
                    {
                        if (!region.IsLoaded)
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

        public static void Clean()
        {
            CleanContentRegions();
        }

    }
}
