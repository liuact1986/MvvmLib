using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using ValidationSample.Windows.Models;

namespace ValidationSample.Windows.ViewModels
{
    //public class MainPageViewModel : BindableBase
    //{
    //    private UserWrapper user;
    //    public UserWrapper User
    //    {
    //        get { return user; }
    //        set { SetProperty(ref user, value); }
    //    }

    //    public List<ValidationHandling> ValidationTypes => new List<ValidationHandling>
    //    {
    //        ValidationHandling.OnPropertyChange,
    //        ValidationHandling.OnSubmit,
    //        ValidationHandling.Explicit
    //    };

    //    public ICommand SaveCommand { get; }

    //    public ICommand ResetCommand { get; }

    //    public MainPageViewModel()
    //    {
    //        User = new UserWrapper(new Models.User
    //        {
    //            Id = 1
    //        });
    //        this.user.BeginEdit();

    //        SaveCommand = new RelayCommand(OnSave);
    //        ResetCommand = new RelayCommand(OnReset);
    //    }

    //    private void OnSave()
    //    {
    //        this.User.ValidateAll();

    //        // for sample (view notification for Base class properties)
    //        this.RaisePropertyChanged("User");
    //    }

    //    private void OnReset()
    //    {
    //        this.user.CancelEdit();
    //        this.RaisePropertyChanged("User");
    //    }
    //}


    /// <summary>
    /// With ValidatableAndEditable
    /// </summary>
    public class MainPageViewModel : BindableBase
    {
        private User user;
        public User User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        public List<ValidationHandling> ValidationTypes => new List<ValidationHandling>
        {
            ValidationHandling.OnPropertyChange,
            ValidationHandling.OnSubmit,
            ValidationHandling.Explicit
        };

        public ICommand SaveCommand { get; }

        public ICommand ResetCommand { get; }

        public MainPageViewModel()
        {
            User = new User
            {
                Id = 1
            };
            this.user.BeginEdit();

            SaveCommand = new RelayCommand(OnSave);
            ResetCommand = new RelayCommand(OnReset);
        }

        private void OnSave()
        {
            this.User.ValidateAll();

            // for sample (view notification for Base class properties)
            this.OnPropertyChanged("User");
        }

        private void OnReset()
        {
            this.user.CancelEdit();
            this.OnPropertyChanged("User");
        }
    }
}
