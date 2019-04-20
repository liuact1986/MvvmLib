using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class PeopleViewModel : BindableBase
    {
        public ObservableCollection<Person> People { get; set; }

        private Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                if (SetProperty(ref selectedPerson, value))
                {
                    OnShowDetails(selectedPerson);
                }
            }
        }

        private IRegionNavigationService regionNavigationService;
        private IFakePeopleService fakePeopleService;

        public ICommand SelectedPersonChangedCommand { get; }

        public PeopleViewModel(IRegionNavigationService regionNavigationService, IFakePeopleService fakePeopleService)
        {
            this.regionNavigationService = regionNavigationService;
            this.fakePeopleService = fakePeopleService;

            SelectedPersonChangedCommand = new RelayCommand<Person>(OnShowDetails);

            this.Load();
        }

        private void Load()
        {
            var peopleList = fakePeopleService.GetPeople();
            this.People = new ObservableCollection<Person>(peopleList);
            if (this.People.Count > 0)
            {
                SelectedPerson = People[0];
            }
        }

        private async void OnShowDetails(Person person)
        {
            await regionNavigationService.GetContentRegion("Detail").NavigateAsync(typeof(PersonDetailsView), person.Id);
        }
    }
}
