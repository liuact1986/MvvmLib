using MvvmLib.Animation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region adapter resolver for <see cref="IItemsRegionAdapter"/>.
    /// </summary>
    public class RegionAdapterResolver
    {
        private static readonly Dictionary<Type, IItemsRegionAdapter> regionAdapters;

        /// <summary>
        /// The custom region adapters registered.
        /// </summary>
        public static IReadOnlyDictionary<Type, IItemsRegionAdapter> RegionAdapters
        {
            get { return regionAdapters; }
        }

        static RegionAdapterResolver()
        {
            regionAdapters = new Dictionary<Type, IItemsRegionAdapter>();
        }

        /// <summary>
        /// Registers the region adapter.
        /// </summary>
        /// <param name="targetType">The target control type</param>
        /// <param name="regionAdapter">The region adapter</param>
        public static void RegisterRegionAdapter(Type targetType, IItemsRegionAdapter regionAdapter)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (regionAdapter == null)
                throw new ArgumentNullException(nameof(regionAdapter));

            regionAdapters[targetType] = regionAdapter;
        }

        /// <summary>
        /// Gets the region adapter for the target type.
        /// </summary>
        /// <param name="control">The target control</param>
        /// <returns>The region adapter found</returns>
        public static IItemsRegionAdapter GetRegionAdapter(DependencyObject control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            // custom
            if (RegionAdapters.TryGetValue(control.GetType(), out IItemsRegionAdapter regionAdapter))
            {
                return regionAdapter;
            }

            // default 
            if (control is Selector selector)
            {
                return new SelectorRegionAdapter(selector);
            }
            if (control is ItemsControl itemsControl)
            {
                return new ItemsControlRegionAdapter(itemsControl);
            }
            if (control is TransitioningItemsControl transitioningItemsControl)
            {
                return new TransitioningItemsControlRegionAdapter(transitioningItemsControl);
            }

            // not found
            throw new ArgumentException($"No Region Adapter found for the type \"{control.GetType().Name}\"");
        }
    }
}