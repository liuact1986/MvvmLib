using Microsoft.Win32;
using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class SharedSourceNavigationAndEditionSampleViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ISubscriberOptions personSavedSubscriberOptions;
        private readonly ISubscriberOptions cancelAddingPersonSubscriberOptions;
        private readonly ISubscriberOptions cancelUpdatingPersonSubscriberOptions;

        public SharedSource<PersonViewModel> PeopleSource { get; }

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
            this.eventAggregator = eventAggregator;

            State = DataFormState.IsReading;

            personSavedSubscriberOptions = eventAggregator.GetEvent<PersonSavedEvent>().Subscribe(OnPersonSaved);
            cancelAddingPersonSubscriberOptions = eventAggregator.GetEvent<CancelAddingPersonEvent>().Subscribe(OnCancelAddingPerson);
            cancelUpdatingPersonSubscriberOptions = eventAggregator.GetEvent<CancelUpdatingPersonEvent>().Subscribe(OnCancelUpdatingPerson);

            this.PeopleSource = NavigationManager.GetOrCreateSharedSource<PersonViewModel>();

            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Multiple Views with SharedSource");
        }

        private void Load()
        {
            // for demo, in real app use a data service for example
            var people = new InitItemCollection<PersonViewModel>();
            for (int i = 0; i < 3; i++)
            {
                var viewModel = this.PeopleSource.CreateNew();
                var person = new PersonModel
                {
                    Id = i + 1,
                    FirstName = $"First.{i}",
                    LastName = $"Last.{i}"
                };
                people.Add(viewModel, person);
            }
            PeopleSource.Load(people);
        }

        private void Add()
        {
            State = DataFormState.IsAdding;
            var item = this.PeopleSource.AddNew(new PersonModel()); // for demo, pass the id and use a data service in real app
            this.PeopleSource.MoveTo(item);
        }

        private void Update()
        {
            State = DataFormState.IsEditing;
        }

        private void Delete()
        {
            if (PeopleSource.SelectedIndex >= 0)
            {
                if (MessageBox.Show($"Delete {PeopleSource.SelectedItem.Person.FirstName}?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PeopleSource.RemoveAt(PeopleSource.SelectedIndex);
                }
            }
        }

        private void OnPersonSaved(int obj)
        {
            State = DataFormState.IsReading;
        }

        private void OnCancelAddingPerson()
        {
            var last = this.PeopleSource.SelectedItem;
            this.PeopleSource.Remove(last);
            State = DataFormState.IsReading;
        }

        private void OnCancelUpdatingPerson()
        {
            State = DataFormState.IsReading;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            personSavedSubscriberOptions.Unsubscribe();
            cancelAddingPersonSubscriberOptions.Unsubscribe();
            cancelUpdatingPersonSubscriberOptions.Unsubscribe();
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

    public enum DataFormState
    {
        IsAdding,
        IsEditing,
        IsReading
    }

    // for demo, use a model + wrapper in real app
    public class PersonModel : Validatable
    {
        static int lastId = 3;
        public static int GetId()
        {
            lastId = lastId + 1;
            return lastId;
        }

        public int Id { get; internal set; }

        private string firstName;
        [Required]
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        [Required]
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

        private int age;
        public int Age
        {
            get { return age; }
            set { SetProperty(ref age, value); }
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
                person.ValidateAll();
                if (!person.HasErrors)
                {
                    // save to db ..
                    // notify
                    eventAggregator.GetEvent<PersonSavedEvent>().Publish(person.Id);
                }
            }
            else
            {
                person.ValidateAll();
                if (!person.HasErrors)
                {
                    // save to db ..
                    int fakeId = PersonModel.GetId();
                    person.Id = fakeId;

                    // notify
                    eventAggregator.GetEvent<PersonSavedEvent>().Publish(fakeId);
                }
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

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            Person = navigationContext.Parameter as PersonModel;
            this.person.BeginEdit();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
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
