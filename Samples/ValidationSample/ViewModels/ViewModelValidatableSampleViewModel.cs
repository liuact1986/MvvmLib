using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using ValidationSample.Models;

namespace ValidationSample.ViewModels
{
    public class ViewModelValidatable : Validatable
    {
        public int Id { get; set; }

        private string firstName;
        [Required]
        [StringLength(8)]
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        [StringLength(3)]
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        [MaxItems(3, ErrorMessage = "The user cannot have more than 2 pets (ViewModel)")]
        public ObservableCollection<string> Pets { get; set; }

        public ViewModelValidatable()
        {
            this.Pets = new ObservableCollection<string>();        }

        public ViewModelValidatable(int id, string firstName, string lastName)
        {
            this.Pets = new ObservableCollection<string>();
            this.Id = id;
            this.firstName = firstName;
            this.lastName = lastName;
        }

        protected override IEnumerable<string> DoCustomValidations(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(FirstName):
                    if (string.Equals(FirstName, "Marie", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Marie is not allowed (ViewModel)";
                    }
                    break;
            }
        }
    }

    public class ViewModelValidatableSampleViewModel : BindableBase
    {
        public ViewModelValidatable User { get; set; }

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
        public IDelegateCommand RemovePetCommand { get; }
        public ICommand ResetCommand { get; }

        public ViewModelValidatableSampleViewModel()
        {
            this.User = new ViewModelValidatable(1, "Marie", "Bell");
            this.User.Pets.Add("Cat");
            this.User.Pets.CollectionChanged += OnPetsCollectionChanged;

            this.User.BeginEdit();

            Summary = new ObservableCollection<string>();

            SaveCommand = new DelegateCommand(OnSave);
            AddPetCommand = new DelegateCommand(AddPet);
            RemovePetCommand = new DelegateCommand(RemovePet, CanRemovePet);
            ResetCommand = new DelegateCommand(OnReset);
        }

        private void OnPetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.User.CanValidateOnPropertyChanged)
                this.User.ValidateProperty("Pets");
        }

        private bool CanRemovePet()
        {
            return selectedPet != null;
        }

        private void RemovePet()
        {
            if (selectedPet != null)
            {
                this.User.Pets.Remove(selectedPet);
            }
        }

        private void OnSave()
        {
            this.User.ValidateAll();

            Summary.Clear();
            var summary = this.User.GetErrorSummary();
            foreach (var errors in summary.Values)
            {
                foreach (var error in errors)
                    Summary.Add(error);
            }
        }

        private void OnReset()
        {
            this.User.CancelEdit();
            this.User.Pets.CollectionChanged += OnPetsCollectionChanged;

            Summary.Clear();

            this.User.BeginEdit();
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
        }
    }

}
