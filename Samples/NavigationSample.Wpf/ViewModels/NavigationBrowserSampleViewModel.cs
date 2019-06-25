using Microsoft.Win32;
using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class NavigationBrowserSampleViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        // public ObservableCollection<PersonModel> People { get; set; }
        // or list
        public List<PersonModel> People { get; set; } // the itemssource
        public NavigationBrowser Browser { get; }

        public PersonModel CurrentPerson
        {
            get
            {
                if (Browser.Current != null)
                    return Browser.Current as PersonModel;

                return null;
            }
        }

        public ICommand SelectImageCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public NavigationBrowserSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            People = new  List<PersonModel>
            {
               new PersonModel { Id=1, FirstName="First1",LastName="Last1" },
               new PersonModel { Id=2, FirstName="First2",LastName="Last2" },
               new PersonModel { Id=3, FirstName="First3",LastName="Last3"}
            };

            this.Browser = new NavigationBrowser(People);

            SelectImageCommand = new RelayCommand(SelectImage);
            DeleteCommand = new RelayCommand(Delete);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("NavigationBrowser with enumerables, lists and collections");
        }

        private void SelectImage()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == true)
            {
                ((PersonModel)this.Browser.Current).ImagePath = dialog.FileName;
            }
        }

        private void Delete()
        {
            var current = CurrentPerson;
            if (current != null)
            {
                var confirmation = MessageBox.Show($"Delete {current.FirstName}?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                if (confirmation)
                {
                    Browser.Delete();
                }
            }
        }

        private void Save()
        {
            var current = CurrentPerson;
            if (current != null)
            {
                current.ValidateAll();
                if (!current.HasErrors)
                {
                    Browser.Save();
                    eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} saved!");
                }
            }
        }

        private void Cancel()
        {
            var current = CurrentPerson;
            if (current != null)
            {
                if (current.Id > 0)
                {
                    Browser.Cancel();
                }
                else
                {
                    Browser.Delete();
                }
            }
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
          
        }
    }
}
