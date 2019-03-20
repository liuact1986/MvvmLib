using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public interface IRegionManager
    {
        ContentRegion GetContentRegion(string regionName);
        ContentRegion GetContentRegion(string regionName, string name);
        List<ContentRegion> GetContentRegions(string regionName);
        ItemsRegion GetItemsRegion(string regionName);
        ItemsRegion GetItemsRegion(string regionName, string name);
        List<ItemsRegion> GetItemsRegions(string regionName);
    }
}