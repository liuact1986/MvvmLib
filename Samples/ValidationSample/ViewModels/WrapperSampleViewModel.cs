﻿using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ValidationSample.Models;
using ValidationSample.Wrapper;

namespace ValidationSample.ViewModels
{

    public class WrapperSampleViewModel : BindableBase
    {
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

        public ObservableCollection<string> Summary { get; private set; }

        //public ObservableCollection<string> Pets { get; set; }

        public List<ValidationHandling> ValidationTypes => new List<ValidationHandling>
        {
            ValidationHandling.OnPropertyChange,
            ValidationHandling.OnSubmit,
            ValidationHandling.Explicit
        };

        public ICommand SaveCommand { get; }
        public ICommand AddPetCommand { get; }
        public IDelegateCommand RemovePetCommand { get; }
        public ICommand ResetCommand { get; }

        public WrapperSampleViewModel()
        {
            //Pets = new ObservableCollection<string>();
            Summary = new ObservableCollection<string>();

            SaveCommand = new DelegateCommand(OnSave);
            AddPetCommand = new DelegateCommand(AddPet);
            RemovePetCommand = new DelegateCommand(RemovePet, CanRemovePet);
            ResetCommand = new DelegateCommand(OnReset);
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
            // load from database in real sample for example
            var user = new User
            {
                Id = 1,
                FirstName = "Marie",
                LastName = "Bell",
                Pets = new Collection<string> { "Cat" }
            };

            InitializeUser(user);
            InitializePets(user.Pets);

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
            while(User.Pets.Contains(pet))
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
