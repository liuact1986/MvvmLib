using MvvmLib.Mvvm;
using System.Linq;
using MvvmLib.Navigation;
using System.Windows;

namespace RegionSample.ViewModels
{
    public class CustomWindowViewModel : BindableBase, ILoadedEventListener
    {
        IRegionNavigationService navigationService;

        public CustomWindowViewModel(IRegionNavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            var regions = navigationService.GetContentRegions("ContentRegion");
            var region = regions.FirstOrDefault(); // get the first content region with the region name "ContentRegion" of the Shell 

            var customViewContentRegion = regions.LastOrDefault(); // get the content region of the window with the same region name \"ContentRegion\"

            // get current history entry and navigate with the content region of the window to the view with parameter
            var currentEntry = region.History.Current;
            if (currentEntry != null)
                await customViewContentRegion.NavigateAsync(currentEntry.SourceType, currentEntry.Parameter);
        }
    }
}
