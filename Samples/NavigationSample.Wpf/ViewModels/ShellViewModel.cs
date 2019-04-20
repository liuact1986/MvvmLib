using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel  :ILoadedEventListener
    {
        private IRegionNavigationService regionNavigationService;

        public ICommand NavigateCommand { get; }

        public ShellViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;

            NavigateCommand = new RelayCommand<Type>(OnNavigate);
        }

        private async void OnNavigate(Type viewType)
        {
            await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(viewType);
        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            var region = regionNavigationService.GetContentRegion("MainRegion");
            region.ConfigureAnimation(new OpacityAnimation { From = 0, To = 1 }, new OpacityAnimation { From = 1, To = 0 });
        }
    }
}
