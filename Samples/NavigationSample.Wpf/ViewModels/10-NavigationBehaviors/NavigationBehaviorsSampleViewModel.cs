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
    public class NavigationBehaviorsSampleViewModel : SyncTitleViewModel
    {
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

        public NavigationSource Navigation { get; }

        public ICommand SayHelloCommand { get; }

        public ObservableCollection<Person> People { get; set; }

        public ICommand SelectPersonCommand { get; }

        public NavigationBehaviorsSampleViewModel(IEventAggregator eventAggregator, IFakePeopleService fakePeopleService)
            : base(eventAggregator)
        {
            this.fakePeopleService = fakePeopleService;

            this.Title = "Navigation Behaviors";

            Navigation = NavigationManager.GetNavigationSource("MainContent");

            SayHelloCommand = new RelayCommand<string>(SayHello);
            SelectPersonCommand = new RelayCommand<Person>(ShowPersonDetails);
            Load();
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

    }
}
