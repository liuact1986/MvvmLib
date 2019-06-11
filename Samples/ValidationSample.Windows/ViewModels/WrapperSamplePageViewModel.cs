using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ValidationSample.Models;
using ValidationSample.Wrapper;

namespace ValidationSample.ViewModels
{

    public class WrapperSamplePageViewModel : BindableBase
    {

        //public ObservableCollection<string> Pets { get; set; }

        public ObservableCollection<ValidationHandling> ValidationTypes { get; set; }

        public ObservableCollection<string> Summary { get; private set; }

        private UserWrapper user;
        public UserWrapper User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        private string selectedPet;
        public string SelectedPet
        {
            get { return selectedPet; }
            set
            {
                if (SetProperty(ref selectedPet, value))
                {
                    RemovePetCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand AddPetCommand { get; }
        public IRelayCommand RemovePetCommand { get; }
        public ICommand ResetCommand { get; }

        public WrapperSamplePageViewModel()
        {
            //Pets = new ObservableCollection<string>();
            Summary = new ObservableCollection<string>();
            ValidationTypes = new ObservableCollection<ValidationHandling>
            {
                ValidationHandling.OnPropertyChange,
                ValidationHandling.OnSubmit,
                ValidationHandling.Explicit
            };

            SaveCommand = new RelayCommand(OnSave);
            AddPetCommand = new RelayCommand(AddPet);
            RemovePetCommand = new RelayCommand(RemovePet, CanRemovePet);
            ResetCommand = new RelayCommand(OnReset);
        }

        private bool CanRemovePet()
        {
            return selectedPet != null;
        }

        private void RemovePet()
        {
            if (selectedPet != null)
            {
                //user.Model.Pets.Remove(selectedPet);
                //Pets.Remove(selectedPet);
                user.Pets.Remove(selectedPet);
            }
        }

        public void Load()
        {
            User = new UserWrapper(new User
            {
                Id = 1,
                FirstName = "Marie",
                LastName = "Bell",
                Pets = new Collection<string> { "Cat" }
            });

            //InitializeUser(user);
            //InitializePets(user.Pets);

            this.user.BeginEdit();
        }

        private void InitializeUser(User user)
        {
            User = new UserWrapper(user);
        }

        private void InitializePets(ICollection<string> pets)
        {
            //Pets.Clear();
            //foreach (var pet in pets)
            //    Pets.Add(pet);

            //User.Pets = new ObservableCollection<string>(pets);
        }

        private void OnSave()
        {
            this.user.ValidateAll();

            Summary.Clear();
            var summary = this.user.GetErrorSummary();
            foreach (var errors in summary.Values)
            {
                foreach (var error in errors)
                    Summary.Add(error);
            }
        }

        private void OnReset()
        {
            this.user.CancelEdit();
            InitializePets(this.user.Model.Pets);

            Summary.Clear();

            this.user.BeginEdit();
        }

        private void AddPet()
        {
            var pets = new string[] { "Chicken", "Dog", "Hamster", "Rabbit", "Hedgehog", "Squirrel" };

            var random = new Random();
            int index = random.Next(pets.Length - 1);

            var pet = pets[index];
            var basePet = pet;

            int count = 2;
            while (User.Pets.Contains(pet))
            {
                pet = $"{basePet} {count}";
                count++;
            }

            User.Pets.Add(pet);

            //user.Model.Pets.Add(pet);
            //Pets.Add(pet);
        }
    }

}
