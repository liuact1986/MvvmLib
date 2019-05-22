using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class MasterDetailViewModel : IIsLoaded
    {
        private IRegionNavigationService regionNavigationService;

        public ICommand NavigateCommand { get; }

        public MasterDetailViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            this.regionNavigationService = regionNavigationService;

            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Master Detail with ISelectable (PersonDetailsViewModel)");
        }

        public async void OnLoaded(object parameter)
        {
            await regionNavigationService.GetContentRegion("Master").NavigateAsync(typeof(PeopleView));
        }
    }
}
