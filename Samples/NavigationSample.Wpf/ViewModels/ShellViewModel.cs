using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel : BindableBase, ILoadedEventListener
    {
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }


        private IRegionNavigationService regionNavigationService;

        public ICommand NavigateCommand { get; }

        public ICommand WhenAvailableCommand { get; }

        public ShellViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;

            NavigateCommand = new RelayCommand<Type>(OnNavigate);

            WhenAvailableCommand = new RelayCommand(OnWhenAvailable);
        }

        private async void OnWhenAvailable()
        {
            IsBusy = true;

            if (!await regionNavigationService.NavigateWhenAvailable("WhenAvailableRegion", typeof(ViewC),
                "When available parameter", OnNavigationCompleted))
            {

                await Task.Delay(3000);

                await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(typeof(WhenAvailableSampleView));
            }
            else
            {
                IsBusy = false;
            }
        }

        private void OnNavigationCompleted()
        {
            IsBusy = false;
        }

        private async void OnNavigate(Type viewType)
        {
            await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(viewType);
        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            var region = regionNavigationService.GetContentRegion("MainRegion");
            //region.ConfigureAnimation(new OpacityAnimation { From = 0, To = 1 }, new OpacityAnimation { From = 1, To = 0 });
        }
    }

    public class ViewCViewModel : BindableBase, INavigatable
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
                Message = (string)parameter;
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {

        }
    }
}
