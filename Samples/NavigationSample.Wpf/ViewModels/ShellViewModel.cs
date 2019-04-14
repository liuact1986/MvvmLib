using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel
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
            await regionManager.GetContentRegion("MainRegion")
                .NavigateAsync(viewType, EntranceTransitionType.SlideInFromLeft, ExitTransitionType.SlideOutToRight);
        }
    }
}
