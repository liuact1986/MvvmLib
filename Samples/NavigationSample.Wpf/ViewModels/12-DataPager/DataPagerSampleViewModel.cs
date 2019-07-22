using Microsoft.Win32;
using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class DataPagerSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public ObservableCollection<PersonModel> People { get; private set; }
        public PagedSource PagedSource { get; private set; }

        public ICommand FilterCommand { get; set; }
        public ICommand SortCommand { get; set; }

        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DataPagerSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            SelectImageCommand = new DelegateCommand(SelectImage);
            FilterCommand = new DelegateCommand<string>(Filter);
            SortCommand = new DelegateCommand(Sort);
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);
        }

        private void SelectImage()
        {
            if (this.PagedSource.CurrentItem == null)
                return;

            var current = this.PagedSource.CurrentItem as PersonModel;

            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dialog.ShowDialog() == true)
            {
                current.ImagePath = dialog.FileName;
            }
        }

        private void Save()
        {
            try
            {
                if (this.PagedSource.IsAddingNew)
                {
                    var current = this.PagedSource.CurrentAddItem as PersonModel;
                    current.ValidateAll();
                    if (!current.HasErrors)
                    {
                        // save to db ...

                        PagedSource.CommitNew();

                        eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{current.FirstName} added!");
                    }
                }
                else if (this.PagedSource.IsEditingItem)
                {
                    var current = this.PagedSource.CurrentEditItem as PersonModel;
                    current.ValidateAll();
                    if (!current.HasErrors)
                    {

                        // save to db ..

                        PagedSource.CommitEdit();

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
            if (this.PagedSource.CurrentItem == null)
                return;

            var current = this.PagedSource.CurrentItem as PersonModel;
            string name = current.FirstName;
            var result = MessageBox.Show($"Delete {name}?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            if (result)
            {
                try
                {
                    // remove from db ...

                    PagedSource.Remove(current);

                    eventAggregator.GetEvent<NotificationMessageEvent>().Publish($"{name} removed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"A problem occured:{ex.Message}");
                }
            }
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("DataPager and PagedSource for DataGrid");
        }

        private void Sort()
        {
            PagedSource.CustomSort = new PersonByAgeSorter();
        }

        private void Filter(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                PagedSource.ClearFilter();
            }
            else
            {
                var age = int.Parse(args.ToString());
                PagedSource.FilterBy<PersonModel>(p => p.Age > age);
            }
        }

        private void Load()
        {
            int count = 100;
            var random = new Random();

            var list = new List<PersonModel>();
            for (int i = 0; i < count; i++)
            {
                int rank = i + 1;
                var age = random.Next(15, 80);
                var person = new PersonModel { Id = rank, FirstName = $"First.{rank}", LastName = $"Last.{rank}", Age = age };
                list.Add(person);
            }
            People = new ObservableCollection<PersonModel>(list);
            this.PagedSource = new PagedSource(People, 10);
            this.PagedSource.CurrentChanged += PagedSource_CurrentChanged;
        }

        private void PagedSource_CurrentChanged(object sender, EventArgs e)
        {
            
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            Load();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SetTitle();
        }
    }

    public class PersonByAgeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((PersonModel)x).Age.CompareTo(((PersonModel)y).Age);
        }
    }

    public class PersonByAgeDescendingSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            return -((PersonModel)x).Age.CompareTo(((PersonModel)y).Age);
        }
    }
}
