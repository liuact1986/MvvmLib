using HelloWorld.Views;
using MvvmLib.Navigation;
using System.Windows;

namespace HelloWorld.ViewModels
{
    public class ShellViewModel : ILoadedEventListener
    {
        private readonly IRegionNavigationService regionNavigationService;

        public ShellViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(typeof(HomeView), "Hello World!");
        }
    }
}
