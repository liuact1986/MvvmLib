using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ChildRegionSampleViewModel : BindableBase
    {
        private IRegionNavigationService regionNavigationService;

        public IRelayCommand NavigateToRootCommand { get; }
        public IRelayCommand NavigateCommand { get; }
        public ICommand NavigateToViewModelCommand { get; }

        private ContentRegion region;

        public IRelayCommand GoBackCommand { get; }
        public IRelayCommand GoForwardCommand { get; }

        public ChildRegionSampleViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            this.regionNavigationService = regionNavigationService;

            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Child Regions Sample");

            region = regionNavigationService.GetContentRegion("ChildRegionSample");

            region.CanGoBackChanged += OnCanGoBackChanged;
            region.CanGoForwardChanged += OnCanGoForwardChanged;

            NavigateCommand = new RelayCommand<Type>(async (sourceType) => await region.NavigateAsync(sourceType));
            NavigateToViewModelCommand = new RelayCommand<Type>(async (sourceType) => await region.NavigateAsync(sourceType, true));

            NavigateToRootCommand = new RelayCommand(async () => await region.NavigateToRootAsync(), () => region.CanGoBack);
            GoBackCommand = new RelayCommand(async () => await region.GoBackAsync(), () => region.CanGoBack);
            GoForwardCommand = new RelayCommand(async () => await region.GoForwardAsync(), () => region.CanGoForward);
        }

        private void OnCanGoBackChanged(object sender, EventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void OnCanGoForwardChanged(object sender, EventArgs e)
        {
            GoForwardCommand.RaiseCanExecuteChanged();
        }
    }

    public class ComposedViewAViewModel : BindableBase, IIsLoaded
    {
        private IRegionNavigationService regionNavigationService;

        public ComposedViewAViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
        }

        public async void OnLoaded(object parameter)
        {
            if (parameter is bool b) // use ViewModel
                await regionNavigationService.GetContentRegion("Zero").NavigateAsync(typeof(FirstViewModel), true);
            else
                await regionNavigationService.GetContentRegion("Zero").NavigateAsync(typeof(First), false);
        }
    }

    public class FirstViewModel : BindableBase, IIsLoaded, ICanActivate, ICanDeactivate
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private IRegionNavigationService regionNavigationService;

        public FirstViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
            this.Title = "First [ViewModel]";
        }

        public async void OnLoaded(object parameter)
        {
            if (parameter is bool b)
                await regionNavigationService.GetContentRegion("First").NavigateAsync(typeof(SecondViewModel), true);
            else
                await regionNavigationService.GetContentRegion("First").NavigateAsync(typeof(Second), false);
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Can activate First?", "Activate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Can deactivate First?", "Deactivate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }

    public class SecondViewModel : BindableBase, ICanActivate, ICanDeactivate
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public SecondViewModel()
        {
            this.Title = "Second [ViewModel]";
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Can activate Second?", "Activate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Can deactivate Second?", "Deactivate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }
}
