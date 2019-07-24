using Microsoft.Win32;
using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ListCollectionViewExSampleViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        // public ObservableCollection<PersonModel> People { get; set; }
        // or list
        public List<PersonModel> People { get; set; } // the itemssource
        public ListCollectionViewEx CollectionView { get; }
        public ListCollectionViewCommands Commands { get; }

        public ICommand FilterCommand { get; set; }
        public ICommand SortCommand { get; set; }

        public PersonModel CurrentPerson
        {
            get
            {
                if (CollectionView.CurrentItem != null)
                    return CollectionView.CurrentItem as PersonModel;

                return null;
            }
        }

        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ListCollectionViewExSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            People = new List<PersonModel>
            {
                new PersonModel { Id = 1, FirstName = "First.1", LastName ="Last.1", Age = 30, Type = PersonType.Admin },
                new PersonModel { Id = 2, FirstName = "First.2", LastName ="Last.2", Age = 40, Type = PersonType.User },
                new PersonModel { Id = 3, FirstName = "First.3", LastName ="Last.3", Age = 50, Type = PersonType.SuperAdmin },
                new PersonModel { Id = 4, FirstName = "First.4", LastName ="Last.4", Age = 60, Type = PersonType.User },
                new PersonModel { Id = 5, FirstName = "First.5", LastName ="Last.5", Age = 70, Type = PersonType.Admin },
                new PersonModel { Id = 6, FirstName = "First.6", LastName ="Last.6", Age = 30, Type = PersonType.Admin },
                new PersonModel { Id = 7, FirstName = "First.7", LastName ="Last.7", Age = 40, Type = PersonType.User },
                new PersonModel { Id = 8, FirstName = "First.8", LastName ="Last.8", Age = 50, Type = PersonType.SuperAdmin },
                new PersonModel { Id = 9, FirstName = "First.9", LastName ="Last.9", Age = 60, Type = PersonType.User },
                new PersonModel { Id = 10, FirstName = "First.10", LastName ="Last.10", Age = 70, Type = PersonType.Admin },
                new PersonModel { Id = 11, FirstName = "First.11", LastName ="Last.11", Age = 30, Type = PersonType.Admin },
            };

            this.CollectionView = new ListCollectionViewEx(People);
            this.Commands = new ListCollectionViewCommands(this.CollectionView);

            SelectImageCommand = new DelegateCommand(SelectImage);
            FilterCommand = new DelegateCommand<string>(Filter);
            SortCommand = new DelegateCommand(Sort);
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);
        }

        private void Sort()
        {
            this.CollectionView.SortBy("Age", true);
        }

        private void Filter(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                this.CollectionView.ClearFilter();
            }
            else
            {
                var age = int.Parse(args.ToString());
                this.CollectionView.FilterBy<PersonModel>(p => p.Age > age);
            }
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("ListCollectionViewEx");
        }

        private void SelectImage()
        {
            if (CurrentPerson == null)
                return;

            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == true)
            {
                CurrentPerson.ImagePath = dialog.FileName;
            }
        }

        private void Save()
        {
            try
            {
                if (this.CollectionView.IsAddingNew)
                {
                    var current = this.CollectionView.CurrentAddItem as PersonModel;
                    current.ValidateAll();
                    if (!current.HasErrors)
                    {
                        // save to db ...

                        CollectionView.CommitNew();

                        eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} added!");
                    }
                }
                else if (this.CollectionView.IsEditingItem)
                {
                    var current = this.CollectionView.CurrentEditItem as PersonModel;
                    current.ValidateAll();
                    if (!current.HasErrors)
                    {

                        // save to db ..

                        CollectionView.CommitEdit();

                        eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} saved!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A problem occured:{ex.Message}");
            }
        }

        private void Delete()
        {
            if (this.CollectionView.CurrentItem == null)
                return;

            var current = this.CollectionView.CurrentItem as PersonModel;
            string name = current.FirstName;
            var result = MessageBox.Show($"Delete {name}?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            if (result)
            {
                try
                {
                    // remove from db ...

                    CollectionView.Remove(current);

                    eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{name} removed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"A problem occured:{ex.Message}");
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
