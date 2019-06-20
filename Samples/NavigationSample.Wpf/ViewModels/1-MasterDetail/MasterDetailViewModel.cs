using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System.Linq;

namespace NavigationSample.Wpf.ViewModels
{

    public class MasterDetailViewModel : INavigationAware
    {
        // source for listView of People (lookups?)
        public SharedSource<Person> PeopleListSource { get; }

        // source for ContentControl that displays Person details (PersonDetailsView)
        public NavigationSource Navigation { get; }

        private readonly IEventAggregator eventAggregator;
        private IFakePeopleService fakePeopleService;

        public MasterDetailViewModel(IEventAggregator eventAggregator, IFakePeopleService fakePeopleService)
        {
            this.eventAggregator = eventAggregator;
            this.fakePeopleService = fakePeopleService;

            Navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
            PeopleListSource = NavigationManager.GetSharedSource<Person>("MasterDetails");

            Navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
            PeopleListSource = NavigationManager.GetSharedSource<Person>("MasterDetails");

            PeopleListSource.SelectedItemChanged += OnDetailsSourceSelectedItemChanged;
            Navigation.CurrentChanged += OnNavigationCurrentChanged;
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Master Detail with ISelectable (PersonDetailsViewModel)");
        }

        private bool handleSelectionChanged = true;

        private void OnDetailsSourceSelectedItemChanged(object sender, SharedSourceSelectedItemChangedEventArgs e)
        {
            if (handleSelectionChanged)
            {
                var person = e.SelectedItem as Person;
                if (person != null)
                {
                    Navigation.Navigate(typeof(PersonDetailsView), person.Id);
                    // ViewModel
                    //Navigation.Navigate(typeof(PersonDetailsViewModel), person.Id);
                }
            }
        }

        private void OnNavigationCurrentChanged(object sender, CurrentSourceChangedEventArgs e)
        {
            // sync
            var selectedView = e.Current as PersonDetailsView;
            if(selectedView != null)
            {
                var selectedPerson = (selectedView.DataContext as PersonDetailsViewModel).Person;
                var personLookup = PeopleListSource.Items.FirstOrDefault(p => p.Id == selectedPerson.Id);
                if (personLookup != null)
                {
                    handleSelectionChanged = false;
                    PeopleListSource.SelectedItem = personLookup;
                    handleSelectionChanged = true;
                }
            }
        }

        private void Load()
        {
            var peopleList = fakePeopleService.GetPeople();
            PeopleListSource.Load(peopleList);
        }


        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            PeopleListSource.SelectedItemChanged -= OnDetailsSourceSelectedItemChanged;
            Navigation.CurrentChanged -= OnNavigationCurrentChanged;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
            Load();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
