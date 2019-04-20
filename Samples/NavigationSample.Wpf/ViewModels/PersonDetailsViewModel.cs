using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using System;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class PersonDetailsViewModel : BindableBase, INavigatable, ISelectable //, IViewLifetimeStrategy
    {
        private Person person;
        public Person Person
        {
            get { return person; }
            set { SetProperty(ref person, value); }
        }

        private string saveMessage;
        public string SaveMessage
        {
            get { return saveMessage; }
            set { SetProperty(ref saveMessage, value); }
        }

        private IRegionNavigationService regionNavigationService;

        public ICommand SaveCommand { get; }

        //public StrategyType Strategy => StrategyType.Singleton;

        private IFakePeopleService fakePeopleService;

        public PersonDetailsViewModel(IRegionNavigationService regionNavigationService, IFakePeopleService fakePeopleService)
        {
            this.regionNavigationService = regionNavigationService;
            this.fakePeopleService = fakePeopleService;

            SaveCommand = new RelayCommand<object>(OnSave);
        }

        private void OnSave(object value)
        {
            SaveMessage = $"Save:{person.Id} {person.FirstName} {person.LastName} {person.EmailAddress} {value} {DateTime.Now.ToLongTimeString()}";
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            int id = (int)parameter;
            var person = fakePeopleService.GetPersonById(id);
            Person = person;
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public bool IsTarget(Type viewType, object parameter)
        {
            if (parameter != null)
            {
                return person.Id == (int)parameter;
            }
            return false;
        }
    }
}
