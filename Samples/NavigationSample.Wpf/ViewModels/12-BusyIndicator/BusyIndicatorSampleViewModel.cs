using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System.Threading.Tasks;

namespace NavigationSample.Wpf.ViewModels
{
    public class BusyIndicatorSampleViewModel : INavigationAware
    {
        private IEventAggregator eventAggregator;

        public NavigationSource Navigation { get; }

        public BusyIndicatorSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Navigation = new NavigationSource();
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Show how to handle loading with navigation");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }
    }

    public class ViewHViewModel : BindableBase, INavigationAware
    {
        private string message;
        private readonly IEventAggregator eventAggregator;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewHViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            Message = "ViewH [ViewModel]";
        }

        public async Task LoadAsync()
        {
            // load some data from a service ...

            await Task.Delay(5000);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public async void OnNavigatingTo(NavigationContext navigationContext)
        {
            eventAggregator.GetEvent<BusyEvent>().Publish(new BusyEventArgs { IsBusy = true });

            await LoadAsync();

            eventAggregator.GetEvent<BusyEvent>().Publish(new BusyEventArgs { IsBusy = false });
        }
    }

    public class BusyEvent : ParameterizedEvent<BusyEventArgs>
    {

    }

    public class BusyEventArgs
    {
        public bool IsBusy { get; set; }
        public string Message { get; set; }
    }

}
