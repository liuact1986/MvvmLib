using System;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public class RegionAdapterContainer
    {
        private readonly static Dictionary<Type, IItemsRegionAdapter> itemsRegionAdapters;

        static RegionAdapterContainer()
        {
            itemsRegionAdapters = new Dictionary<Type, IItemsRegionAdapter>();

            RegisterDefaultAdapters();
        }

        private static void RegisterDefaultAdapters()
        {
            itemsRegionAdapters.Clear();

            RegisterRegionAdapter(new ItemsControlAdapter());
            RegisterRegionAdapter(new TabControlAdapter());
        }

        public static void RegisterRegionAdapter(IItemsRegionAdapter itemsRegionAdapter)
        {
            itemsRegionAdapters[itemsRegionAdapter.TargetType] = itemsRegionAdapter;
        }

        public static IItemsRegionAdapter GetRegionAdapter(Type targetType)
        {
            if (itemsRegionAdapters.ContainsKey(targetType))
                return itemsRegionAdapters[targetType];

            throw new Exception($"No ItemsRegionAdapater registered for the type \"{nameof(targetType)}\"");
        }

    }
}