using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ValidationSample.Models;

namespace ValidationSample.Wrapper
{
    public class UserWrapper : ModelWrapper<User>
    {
        public UserWrapper(User model)
            : base(model)
        {
            this.Pets = new ObservableCollection<string>();
            this.InitiliazePets();
            this.EditionCancelled += OnEditionCancelled;
        }

        private void OnEditionCancelled(object sender, EventArgs e)
        {
            this.Pets.CollectionChanged -= OnPetsCollectionChanged;
            this.InitiliazePets();
        }

        private void InitiliazePets()
        {
            this.Pets.Clear();
            if (this.Model.Pets.Count > 0)
            {
                foreach (var pet in this.Model.Pets)
                {
                    this.Pets.Add(pet);
                }
            }
            this.Pets.CollectionChanged += OnPetsCollectionChanged;
        }

        private void OnPetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update model
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        Model.Pets.Add((string)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        Model.Pets.Remove((string)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Model.Pets.Clear();
                    break;
                default:
                    break;
            }

            // validate
            if (this.CanValidateOnPropertyChanged)
                this.ValidateProperty("Pets");
        }

        public int Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public ObservableCollection<string> Pets { get; set; }

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
