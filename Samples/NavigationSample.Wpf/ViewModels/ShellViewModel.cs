using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private IRegionNavigationService regionNavigationService;

        public ICommand NavigateCommand { get; }

        public ICommand WhenAvailableCommand { get; }

        public ShellViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            Title = "Navigation Sample [WPF]";

            this.regionNavigationService = regionNavigationService;

            NavigateCommand = new RelayCommand<Type>(OnNavigate);

            WhenAvailableCommand = new RelayCommand(OnWhenAvailable);

            eventAggregator.GetEvent<ChangeTitleEvent>().Subscribe(OnChangeTitle);
        }

        private void OnChangeTitle(string newTitle)
        {
            Title = newTitle;
        }

        private async void OnWhenAvailable()
        {
            OnChangeTitle("Wait region loaded with WhenAvailable");

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

        private async void OnNavigate(Type sourceType)
        {
            await regionNavigationService.GetContentRegion("MainRegion").NavigateAsync(sourceType);
        }
    }
}
