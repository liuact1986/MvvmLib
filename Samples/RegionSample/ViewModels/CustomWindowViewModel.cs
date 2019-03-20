using MvvmLib.Mvvm;
using System.Linq;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{
    public class CustomWindowViewModel : BindableBase, ILoadedEventListener
    {
        IRegionManager regionManager;

        public CustomWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public async void OnLoaded(object parameter)
        {
            var regions = regionManager.GetContentRegions("ContentRegion");
            var region = regions.FirstOrDefault();
            var customViewContentRegion = regions.LastOrDefault();
            var currentEntry = region.History.Current;
            if (currentEntry != null)
            {
                await customViewContentRegion.NavigateAsync(currentEntry.SourceType);
            }
        }
    }
}
