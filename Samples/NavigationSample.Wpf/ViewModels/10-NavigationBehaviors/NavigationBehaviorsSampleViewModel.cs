using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class NavigationBehaviorsSampleViewModel : SyncTitleViewModel
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public NavigationSource Navigation { get; }

        public ICommand SayHelloCommand { get; }

        public NavigationBehaviorsSampleViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            this.Title = "Navigation Behaviors";

            Navigation = NavigationManager.GetNavigationSource("MainContent");

            SayHelloCommand = new RelayCommand<string>(SayHello);
        }

        private void SayHello(string value)
        {
            Message = $"Hello {value}! {DateTime.Now.ToLongTimeString()}";
            //MessageBox.Show(Message);
        }

    }
}
