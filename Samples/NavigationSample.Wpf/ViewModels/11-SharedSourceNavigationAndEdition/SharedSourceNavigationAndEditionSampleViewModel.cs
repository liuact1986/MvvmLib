using Microsoft.Win32;
using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    class SharedSourceNavigationAndEditionSampleViewModel : BindableBase, INavigationAware
    {
        public SharedSource<PersonViewModel> PeopleSource { get; }

        public NavigationBrowser Browser { get; }

        private DataFormState state;
        public DataFormState State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public SharedSourceNavigationAndEditionSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Navigation and Edition with SharedSource");

            State = DataFormState.IsShowingDetails;

            eventAggregator.GetEvent<PersonSavedEvent>().Subscribe(OnPersonSaved);
            eventAggregator.GetEvent<CancelAddingPersonEvent>().Subscribe(OnCancelAddingPerson);
            eventAggregator.GetEvent<CancelUpdatingPersonEvent>().Subscribe(OnCancelUpdatingPerson);

            this.PeopleSource = NavigationManager.GetOrCreateSharedSource<PersonViewModel>();

            this.Browser = new NavigationBrowser(this.PeopleSource.Items);
            this.Browser.CollectionView.CurrentChanged += OnCollectionViewCurrentChanged;

            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
        }

        private void Load()
        {
            // for demo, in real app use a data service for example
            var people = new Dictionary<PersonViewModel, object>();
            for (int i = 0; i < 3; i++)
            {
                var viewModel = this.PeopleSource.CreateNew();
                var person = new PersonModel
                {
                    Id = i + 1,
                    FirstName = $"First.{i}",
                    LastName = $"Last.{i}"
                };
                viewModel.Person = person; // for demo , load the person with a service from the view model in real app
                people.Add(viewModel, person.Id); // dictionary of items + parameter passed to INavigationAware functions
            }
            this.PeopleSource.Load(people);
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            var position = this.Browser.CollectionView.CurrentPosition;
            this.PeopleSource.SelectedIndex = position;
        }

        private async void Add()
        {
            State = DataFormState.IsAdding;
            var viewModel = this.PeopleSource.CreateNew();
            viewModel.Person = new PersonModel(); 
            await this.PeopleSource.Items.AddAsync(viewModel, -1);
            Browser.MoveCurrentToLast();
        }

        private void Update()
        {
            State = DataFormState.IsEditing;
        }

        private async void Delete()
        {
            if (PeopleSource.SelectedIndex >= 0)
            {
                if (MessageBox.Show($"Delete {PeopleSource.SelectedItem.Person.FirstName}?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    await PeopleSource.Items.RemoveAtAsync(PeopleSource.SelectedIndex);
                }
            }
        }

        private void OnPersonSaved(int obj)
        {
            State = DataFormState.IsShowingDetails;
        }

        private async void OnCancelAddingPerson()
        {
            var last = this.PeopleSource.SelectedItem;
            await this.PeopleSource.Items.RemoveAsync(last);
            State = DataFormState.IsShowingDetails;
        }

        private void OnCancelUpdatingPerson()
        {
            State = DataFormState.IsShowingDetails;
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

    public enum DataFormState
    {
        IsAdding,
        IsEditing,
        IsShowingDetails
    }

    // for demo, use a model + wrapper in real app
    public class PersonModel : Editable
    {
        static int lastId = 3;
        public static int GetId()
        {
            lastId = lastId + 1;
            return lastId;
        }

        public int Id { get; internal set; }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set { SetProperty(ref imagePath, value); }
        }

        public PersonModel()
        {
            ImagePath = "../../Assets/default-user.jpg";
        }
    }

    public class PersonViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        private PersonModel person;
        public PersonModel Person
        {
            get { return person; }
            set { SetProperty(ref person, value); }
        }

        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public PersonViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SelectImage()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == true)
            {
                this.person.ImagePath = dialog.FileName;
            }
        }

        private void Save()
        {
            if (person.Id > 0)
            {
                eventAggregator.GetEvent<PersonSavedEvent>().Publish(person.Id);
            }
            else
            {
                int fakeId = PersonModel.GetId();
                person.Id = fakeId;
                eventAggregator.GetEvent<PersonSavedEvent>().Publish(fakeId);
            }
            this.person.BeginEdit();
        }

        private void Cancel()
        {
            if (person.Id > 0)
            {
                // cancel edit
                this.person.CancelEdit();
                eventAggregator.GetEvent<CancelUpdatingPersonEvent>().Publish();
            }
            else
            {
                // cancel add
                eventAggregator.GetEvent<CancelAddingPersonEvent>().Publish();
            }
            this.person.BeginEdit();
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            // in rela app load person with a data service
            person.Id = (int)parameter;
            this.person.BeginEdit();
        }

        public void OnNavigatedTo(object parameter)
        {

        }
    }


    public class PersonSavedEvent : ParameterizedEvent<int>
    {

    }

    public class PersonUpdatedEvent : ParameterizedEvent<int>
    {

    }

    public class CancelAddingPersonEvent : EmptyEvent
    {

    }

    public class CancelUpdatingPersonEvent : EmptyEvent
    {

    }

}
