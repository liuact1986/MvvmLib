using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Models
{
    public class Person : Validatable
    {
        public int Id { get; set; }

        private string firstName;
        [Required]
        [StringLength(5)]
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }


        private string lastName;
        [StringLength(2)]
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        [MaxItems(3, ErrorMessage = "The person cannot have more than 2 pets.")]
        public ObservableCollection<string> Pets { get; set; }

        public Person()
        {
            this.Pets = new ObservableCollection<string>();
        }

        public Person(int id, string firstName, string lastName)
        {
            this.Pets = new ObservableCollection<string>();
            this.Pets.CollectionChanged += (s, e) =>
            {
                this.ValidateProperty(nameof(Pets));
            };

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
                        yield return "Marie is not allowed";
                    }
                    break;
            }
        }
    }
}
