using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class NavigationBehaviorsSampleViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IFakePeopleService fakePeopleService;

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string message2;
        public string Message2
        {
            get { return message2; }
            set { SetProperty(ref message2, value); }
        }

        private string message3;
        public string Message3
        {
            get { return message3; }
            set { SetProperty(ref message3, value); }
        }

        public NavigationSource Navigation { get; }

        public ICommand SayHelloCommand { get; }

        public ObservableCollection<Person> People { get; set; }

        public ICommand SelectPersonCommand { get; }

        public NavigationBehaviorsSampleViewModel(IEventAggregator eventAggregator, IFakePeopleService fakePeopleService)
        {
            this.eventAggregator = eventAggregator;
            this.fakePeopleService = fakePeopleService;

            Navigation = NavigationManager.GetDefaultNavigationSource("Main");

            SayHelloCommand = new RelayCommand<string>(SayHello);
            SelectPersonCommand = new RelayCommand<Person>(ShowPersonDetails);
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Navigation Behaviors");
        }

        private void SayHello(string value)
        {
            Message = $"Hello {value}! {DateTime.Now.ToLongTimeString()}";
            //MessageBox.Show(Message);
        }

        private void Load()
        {
            var peopleList = fakePeopleService.GetPeople();
            People = new ObservableCollection<Person>(peopleList);
        }

        private void ShowPersonDetails(Person person)
        {
            Message2 = $"Selected Person: {person.ToString()}";
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
            Load();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
          
        }

        //private void MyMethod()
        //{
        //    Message3 = $"MyMethod invoked {DateTime.Now.ToLongTimeString()}";
        //}

        private void MyMethod(object parameter)
        {
            Message3 = $"MyMethod invoked witth parameter '{parameter}' {DateTime.Now.ToLongTimeString()}";
        }
    }
}
