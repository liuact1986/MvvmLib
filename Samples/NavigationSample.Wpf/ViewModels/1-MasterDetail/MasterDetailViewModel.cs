using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{

    public class MasterDetailViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;
        private IFakePeopleLookupService fakePeopleLookupService;

        public SharedSource<NavigationItemViewModel> PeopleListSource { get; } // for listview
        public NavigationSource Navigation { get; } // for content control

        public ICommand AddCommand { get; set; }
        public IRelayCommand DeleteCommand { get; set; }

        public SubscriberOptions<PersonEventArgs> PersonAddedSubscriberOptions { get; private set; }
        public SubscriberOptions<PersonEventArgs> PersonUpdatedSubscriberOptions { get; private set; }

        public MasterDetailViewModel(IEventAggregator eventAggregator, IFakePeopleLookupService fakePeopleLookupService)
        {
            this.eventAggregator = eventAggregator;
            this.fakePeopleLookupService = fakePeopleLookupService;

            Navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
            PeopleListSource = new SharedSource<NavigationItemViewModel>();

            AddCommand = new RelayCommand(Add);
            DeleteCommand = new RelayCommand(Delete, () => PeopleListSource.SelectedIndex != -1);
            PeopleListSource.SelectedItemChanged += PeopleListSource_SelectedItemChanged;
        }

        private void PeopleListSource_SelectedItemChanged(object sender, SharedSourceSelectedItemChangedEventArgs e)
        {
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private void Delete()
        {
            // rmove from db

            Navigation.RemoveSource(Navigation.Current);
            PeopleListSource.Remove(PeopleListSource.SelectedItem);
        }

        private void Add()
        {
            Navigation.Navigate(typeof(PersonDetailsView), -1);
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
                var person = e.SelectedItem as NavigationItemViewModel;
                if (person != null)
                {
                    Navigation.Navigate(typeof(PersonDetailsView), person.Id);
                    // ViewModel
                    //Navigation.Navigate(typeof(PersonDetailsViewModel), person.Id);
                }
            }
        }

        private void Load()
        {
            var peopleList = fakePeopleLookupService.GetPeople();
            var items = new List<NavigationItemViewModel>();
            foreach (var person in peopleList)
            {
                items.Add(new NavigationItemViewModel { Id = person.Id, DisplayName = person.DisplayName });
            }
            PeopleListSource.Load(items);
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            PeopleListSource.SelectedItemChanged -= OnDetailsSourceSelectedItemChanged;
            this.PersonAddedSubscriberOptions.Unsubscribe();
            this.PersonUpdatedSubscriberOptions.Unsubscribe();
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();

            PeopleListSource.SelectedItemChanged += OnDetailsSourceSelectedItemChanged;
            this.PersonAddedSubscriberOptions = eventAggregator.GetEvent<PersonAddedEvent>().Subscribe(OnPersonAdded);
            this.PersonUpdatedSubscriberOptions = eventAggregator.GetEvent<PersonUpdatedSuccesfullyEvent>().Subscribe(OnPersonUpdated);
            Load();
        }

        private void OnPersonAdded(PersonEventArgs args)
        {
            var item = this.PeopleListSource.Items.FirstOrDefault(p => p.Id == args.Id);
            if (item == null)
                this.PeopleListSource.Add(new NavigationItemViewModel { Id = args.Id, DisplayName = args.DisplayName });
        }

        private void OnPersonUpdated(PersonEventArgs args)
        {
            var item = this.PeopleListSource.Items.FirstOrDefault(p => p.Id == args.Id);
            if (item != null)
                item.DisplayName = args.DisplayName;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }

    public class NavigationItemViewModel : BindableBase
    {
        public int Id { get; set; }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set { SetProperty(ref displayName, value); }
        }
    }

    public class PersonDetailsViewModel : BindableBase, INavigationAware, ISelectable, ICanDeactivate
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

        public ICommand SaveCommand { get; }

        private readonly IEventAggregator eventAggregator;
        private IFakePeopleService fakePeopleService;
        private ChangeTracker tracker;

        public PersonDetailsViewModel(IEventAggregator eventAggregator, IFakePeopleService fakePeopleService)
        {
            this.eventAggregator = eventAggregator;
            this.fakePeopleService = fakePeopleService;

            SaveCommand = new RelayCommand<object>(Save);
        }

        private void Save(object value)
        {
            if (this.person.Id > 0)
            {
                // update database ..
                tracker.AcceptChanges();
                eventAggregator.GetEvent<PersonUpdatedSuccesfullyEvent>().Publish(new PersonEventArgs { Id = person.Id, DisplayName = person.FirstName });
            }
            else
            {
                // validation ... save
                fakePeopleService.Add(person);
                tracker.AcceptChanges();
                eventAggregator.GetEvent<PersonAddedEvent>().Publish(new PersonEventArgs { Id = person.Id, DisplayName = person.FirstName });
            }

            SaveMessage = $"Save: {person.Id}, {person.FirstName} {person.LastName}, {person.EmailAddress} {value} {DateTime.Now.ToLongTimeString()}";
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            int id = (int)navigationContext.Parameter;
            Load(id);
        }

        private void Load(int id)
        {
            if (id > 0)
            {
                var person = fakePeopleService.GetPersonById(id);
                this.Person = person;
            }
            else
            {
                var person = new Person();
                this.Person = person;
            }

            this.tracker = new ChangeTracker(person);
        }

        public bool IsTarget(Type viewType, object parameter)
        {
            if (parameter != null)
                return person.Id == (int)parameter;

            return false;
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            bool canDeactivate = true;
            if (person.Id > 0)
            {
                //this.tracker.CheckChanges();
                //if (this.tracker.HasChanges)
                //{
                //    canDeactivate = MessageBox.Show("You've not saved changes. Leave?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                //}
            }
            else
            {
                canDeactivate = MessageBox.Show("You've not saved this item. Leave?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                if (canDeactivate)
                {
                    var navigation = NavigationManager.GetDefaultNavigationSource("MasterDetails");
                    navigation.NotifyOnCurrentChanged = false;
                    navigation.RemoveSourceAt(navigation.CurrentIndex);
                    navigation.NotifyOnCurrentChanged = true;
                }
            }

            tracker.AcceptChanges();
            continuationCallback(canDeactivate);
        }
    }

    public class PersonAddedEvent : ParameterizedEvent<PersonEventArgs>
    {

    }

    public class PersonUpdatedSuccesfullyEvent : ParameterizedEvent<PersonEventArgs>
    {

    }

    public class CancelPersonAddingEvent : ParameterizedEvent<PersonEventArgs>
    {

    }

    public class PersonEventArgs
    {
        public string DisplayName { get; internal set; }
        public int Id { get; internal set; }
    }

}
