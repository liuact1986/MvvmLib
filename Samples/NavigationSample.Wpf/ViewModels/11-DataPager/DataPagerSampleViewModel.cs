using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class DataPagerSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public ObservableCollection<PersonModel> People { get; private set; }
        public DataPager DataPager { get; private set; }

        public ICommand FilterCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand AddCommand { get; set; }

        public DataPagerSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            FilterCommand = new RelayCommand<string>(Filter);
            SortCommand = new RelayCommand(Sort);
            AddCommand = new RelayCommand(Add);
        }

        private void Add()
        {
            var random = new Random();
            int rank = People.Count;
            var age = random.Next(15, 80);
            People.Add(new PersonModel { Id = rank, FirstName = $"First.{rank}", LastName = $"Last.{rank}", Age = age });
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("DataPager with DataGrid");
        }

        private void Sort()
        {
            DataPager.CustomSorter = new PersonSorter();
        }

        private void Filter(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                DataPager.Filter = null;
            }
            else
            {
                var age = int.Parse(args.ToString());
                DataPager.Filter = new Func<object, bool>(p => ((PersonModel)p).Age > age);
            }
        }

        private void Load()
        {
            int count = 250;
            var random = new Random();
            var peopleList = new List<PersonModel>();
            for (int i = 0; i < count; i++)
            {
                int rank = i + 1;
                var age = random.Next(15, 80);
                var person = new PersonModel { Id = rank, FirstName = $"First.{rank}", LastName = $"Last.{rank}", Age = age };
                peopleList.Add(person);
            }
            this.People = new ObservableCollection<PersonModel>(peopleList);
            this.DataPager = new DataPager(People, 10);
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

    public class PersonSorter : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return ((PersonModel)x).Age.CompareTo(((PersonModel)y).Age);
        }
    }
}
