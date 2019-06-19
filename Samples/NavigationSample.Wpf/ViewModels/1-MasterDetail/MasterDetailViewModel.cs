using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;

namespace NavigationSample.Wpf.ViewModels
{

    public class MasterDetailViewModel: INavigationAware
    {
        public NavigationSource Navigation { get; }
        public SharedSource<Person> DetailsSource { get; }

        private IFakePeopleService fakePeopleService;

        public MasterDetailViewModel(IEventAggregator eventAggregator, IFakePeopleService fakePeopleService)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Master Detail with ISelectable (PersonDetailsViewModel)");
            
             Navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
             DetailsSource = NavigationManager.GetSharedSource<Person>("MasterDetails");

            this.fakePeopleService = fakePeopleService;

            Navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
            DetailsSource = NavigationManager.GetSharedSource<Person>("MasterDetails");

            DetailsSource.SelectedItemChanged += OnDetailsSourceSelectedItemChanged;
        }

        private void OnDetailsSourceSelectedItemChanged(object sender, SharedSourceSelectedItemChangedEventArgs e)
        {
            var person = e.SelectedItem as Person;
            if (person != null)
            {
                Navigation.Navigate(typeof(PersonDetailsView), person.Id);
                // ViewModel
                //Navigation.Navigate(typeof(PersonDetailsViewModel), person.Id);
            }
        }

        private void Load()
        {
            var peopleList = fakePeopleService.GetPeople();
            DetailsSource.Load(peopleList);
        }

        public void OnNavigatingFrom()
        {
            
        }

        public void OnNavigatingTo(object parameter)
        {
            Load();
        }

        public void OnNavigatedTo(object parameter)
        {
           
        }
    }
}
