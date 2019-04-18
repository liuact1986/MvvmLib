using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel  :ILoadedEventListener
    {
        private IRegionManager regionManager;

        public ICommand NavigateCommand { get; }

        public ShellViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            NavigateCommand = new RelayCommand<Type>(OnNavigate);
        }

        private async void OnNavigate(Type viewType)
        {
            await regionManager.GetContentRegion("MainRegion").NavigateAsync(viewType);
        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            var region = regionManager.GetContentRegion("MainRegion");
            region.DefaultRegionAnimation.EntranceAnimation = new OpacityAnimation
            {
                From = 0,
                To = 1
            };
            region.DefaultRegionAnimation.ExitAnimation = new OpacityAnimation
            {
                From = 1,
                To = 0
            };
        }
    }
}
