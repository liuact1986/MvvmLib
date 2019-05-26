using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class PeopleViewModel : BindableBase
    {
        public SharedSource<Person> DetailsSource { get; }
        public NavigationSource Navigation { get; }

        private IFakePeopleService fakePeopleService;

        public PeopleViewModel(IFakePeopleService fakePeopleService)
        {
            this.fakePeopleService = fakePeopleService;

            Navigation = NavigationManager.GetOrCreateNavigationSource("Details");

            DetailsSource = NavigationManager.GetOrCreateSharedSource<Person>();
            DetailsSource.SelectedItemChanged += OnDetailsSourceSelectedItemChanged;

            this.Load();
        }

        private async void OnDetailsSourceSelectedItemChanged(object sender, SharedSourceSelectedItemChangedEventArgs e)
        {
            var person = e.SelectedItem as Person;
            if (person != null)
            {
                await Navigation.NavigateAsync(typeof(PersonDetailsView), person.Id);
                // ViewModel
                //await Navigation.NavigateAsync(typeof(PersonDetailsViewModel), person.Id);
            }
        }

        private void Load()
        {
            var peopleList = fakePeopleService.GetPeople();
            DetailsSource.With(peopleList);
        }

        private async void ShowDetails(Person person)
        {
            await Navigation.NavigateAsync(typeof(PersonDetailsView), person.Id);
            // ViewModel
            //await Navigation.NavigateAsync(typeof(PersonDetailsViewModel), person.Id);
        }
    }
}
