using System;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public class RegionAdapterContainer
    {
        static Dictionary<Type, IContentRegionAdapter> contentRegionAdapters { get; set; }
        static Dictionary<Type, IItemsRegionAdapter> itemsRegionAdapters { get; set; }

        static RegionAdapterContainer()
        {
            contentRegionAdapters = new Dictionary<Type, IContentRegionAdapter>();
            itemsRegionAdapters = new Dictionary<Type, IItemsRegionAdapter>();

            RegisterDefaultAdapters();
        }

        private static void RegisterDefaultAdapters()
        {
            contentRegionAdapters.Clear();
            itemsRegionAdapters.Clear();

            RegisterAdapter(new ItemsControlAdapter());
            RegisterAdapter(new TabControlAdapter());
        }

        public static void RegisterAdapter(IContentRegionAdapter contentRegionAdapter)
        {
            contentRegionAdapters[contentRegionAdapter.TargetType] = contentRegionAdapter;
        }

        public static void RegisterAdapter(IItemsRegionAdapter itemsRegionAdapter)
        {
            itemsRegionAdapters[itemsRegionAdapter.TargetType] = itemsRegionAdapter;
        }

        public static IContentRegionAdapter GetContentRegionAdapter(Type targetType)
        {
            if (contentRegionAdapters.ContainsKey(targetType))
            {
                return contentRegionAdapters[targetType];
            }
            throw new Exception("No ContentRegionAdapater registered for type " + nameof(targetType));
        }

        public static IItemsRegionAdapter GetItemsRegionAdapter(Type targetType)
        {
            if (itemsRegionAdapters.ContainsKey(targetType))
            {
                return itemsRegionAdapters[targetType];
            }
            throw new Exception("No ItemsRegionAdapater registered for type " + nameof(targetType));
        }

    }
}