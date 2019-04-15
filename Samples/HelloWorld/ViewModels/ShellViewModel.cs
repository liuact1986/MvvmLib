using HelloWorld.Views;
using MvvmLib.Navigation;
using System.Windows;

namespace HelloWorld.ViewModels
{
    public class ShellViewModel : ILoadedEventListener
    {
        IRegionManager regionManager;

        public ShellViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            await regionManager.GetContentRegion("MainRegion").NavigateAsync(typeof(HomeView), "Hello World!");
        }
    }
}
