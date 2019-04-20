using System;
using System.Windows;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Allows to register content regions and items region with dependency properties.
    /// </summary>
    public class RegionManager
    {

        private static readonly RegionsRegistry regionsRegistry;

        static RegionManager()
        {
            regionsRegistry = RegionsRegistry.Instance;
        }

        /// <summary>
        /// Sets the content region name.
        /// </summary>
        /// <param name="target">The control</param>
        /// <param name="regionName">The region name</param>
        public static void SetContentRegionName(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ContentRegionNameProperty, regionName);
        }

        /// <summary>
        /// Gets the content region name.
        /// </summary>
        /// <param name="target">The control</param>
        /// <returns>The region name</returns>
        public static string GetContentRegionName(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ContentRegionNameProperty) as string;
        }

        /// <summary>
        /// The content region name.
        /// </summary>
        public static readonly DependencyProperty ContentRegionNameProperty =
             DependencyProperty.RegisterAttached("ContentRegionName", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetContentRegionName));

        private static void OnSetContentRegionName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var control = (FrameworkElement)d;
                var regionName = (string)e.NewValue;

                var region = RegionsRegistry.Instance.RegisterContentRegion(regionName, control);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((s1, e1) =>
                {
                    region.isLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

        /// <summary>
        /// Sets the items region name.
        /// </summary>
        /// <param name="target">The control</param>
        /// <param name="regionName">The region name</param>
        public static void SetItemsRegionName(DependencyObject target, string regionName)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            target.SetValue(ItemsRegionNameProperty, regionName);
        }

        /// <summary>
        /// Gets the items region name.
        /// </summary>
        /// <param name="target">The control</param>
        /// <returns>The region name</returns>
        public static string GetItemsRegionName(DependencyObject target)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            return target.GetValue(ItemsRegionNameProperty) as string;
        }

        /// <summary>
        /// The items region name.
        /// </summary>
        public static readonly DependencyProperty ItemsRegionNameProperty =
            DependencyProperty.RegisterAttached("ItemsRegionName", typeof(string), typeof(RegionManager), new PropertyMetadata(OnSetItemsRegionName));

        private static void OnSetItemsRegionName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                var control = (FrameworkElement)d;
                var regionName = (string)e.NewValue;

                var region = RegionsRegistry.Instance.RegisterItemsRegion(regionName, control);

                var listener = new FrameworkElementLoaderListener(element);
                listener.Subscribe((s1, e1) =>
                {
                    region.isLoaded = true;

                    listener.Unsubscribe();
                });
            }
        }

    }
}
