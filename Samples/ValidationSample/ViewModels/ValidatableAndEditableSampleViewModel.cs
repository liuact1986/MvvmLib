using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ValidationSample.Models;

namespace ValidationSample.ViewModels
{
    public class ValidatableAndEditableSampleViewModel : BindableBase
    {
        private Person person;
        public Person Person
        {
            get { return person; }
            set { SetProperty(ref person, value); }
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

        public ObservableCollection<string> Summary { get; private set; }

        public List<ValidationHandling> ValidationTypes => new List<ValidationHandling>
        {
            ValidationHandling.OnPropertyChange,
            ValidationHandling.OnSubmit,
            ValidationHandling.Explicit
        };

        public ICommand SaveCommand { get; }
        public ICommand AddPetCommand { get; }
        public IRelayCommand RemovePetCommand { get; }
        public ICommand ResetCommand { get; }

        public ValidatableAndEditableSampleViewModel()
        {
            Summary = new ObservableCollection<string>();

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
                this.person.Pets.Remove(selectedPet);
            }
        }

        public void Load()
        {
            var person = new Person(1, "Marie", "Bell");
            person.Pets.Add("Cat");

            this.Person = person;

            this.person.BeginEdit();
        }

        private void OnSave()
        {
            this.person.ValidateAll();

            Summary.Clear();
            var summary = this.person.GetErrorSummary();
            foreach (var errors in summary.Values)
            {
                foreach (var error in errors)
                    Summary.Add(error);
            }
        }

        private void OnReset()
        {
            this.person.CancelEdit();

            Summary.Clear();

            this.person.BeginEdit();
        }

        private void AddPet()
        {
            var pets = new string[] { "Chicken", "Dog", "Hamster", "Rabbit", "Hedgehog", "Squirrel" };

            var random = new Random();
            int index = random.Next(pets.Length - 1);

            var pet = pets[index];
            var basePet = pet;

            int count = 2;
            while (person.Pets.Contains(pet))
            {
                pet = $"{basePet} {count}";
                count++;
            }

            person.Pets.Add(pet);
        }
    }

}
