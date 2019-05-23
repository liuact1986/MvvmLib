using HelloWorld.Views;
using MvvmLib.Navigation;

namespace HelloWorld.ViewModels
{
    public class ShellViewModel : IIsLoaded
    {
        private readonly IRegionNavigationService regionNavigationService;

        public ShellViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
        }

        public async void OnLoaded(object parameter)
        {
            await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(typeof(HomeView), "Hello World!");
        }
    }
}
