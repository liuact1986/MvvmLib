using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class HistorySampleViewModel
    {
        private readonly IRegionNavigationService navigationService;

        public ContentRegionHistory ContentRegion1History { get; set; }

        public IRelayCommand NavigateToRootCommand { get; }
        public IRelayCommand NavigateCommand { get; }
        public IRelayCommand GoBackCommand { get; }
        public IRelayCommand GoForwardCommand { get; }

        public HistorySampleViewModel(IRegionNavigationService navigationService, IEventAggregator eventAggregator)
        {
            this.navigationService = navigationService;
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Observable History");

            var contentRegion = navigationService.GetContentRegion("ContentRegion", "ContentRegion1");
            ContentRegion1History = contentRegion.History;

            contentRegion.CanGoBackChanged += OnContentRegionCanGoBackChanged;
            contentRegion.CanGoForwardChanged += OnContentRegionCanGoForwardChanged;

            NavigateToRootCommand = new RelayCommand(async () =>
            {
                await contentRegion.NavigateToRootAsync();
            },
            () => contentRegion.CanGoBack);

            NavigateCommand = new RelayCommand<Type>(async (viewOrViewModelType) =>
            {
                await contentRegion.NavigateAsync(viewOrViewModelType, viewOrViewModelType.Name + " [default]");
            });

            GoBackCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoBackAsync();
            },
            () => contentRegion.CanGoBack);

            GoForwardCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoForwardAsync();
            },
            () => contentRegion.CanGoForward);
        }

        private void OnContentRegionCanGoBackChanged(object sender, EventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void OnContentRegionCanGoForwardChanged(object sender, EventArgs e)
        {
            GoForwardCommand.RaiseCanExecuteChanged();
        }
    }
}
