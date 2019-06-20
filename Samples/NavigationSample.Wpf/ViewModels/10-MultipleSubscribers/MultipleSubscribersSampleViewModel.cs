using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class MultipleSubscribersSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public NavigationSourceContainer Navigation { get; }

        public MultipleSubscribersSampleViewModel(IEventAggregator eventAggregator) 
        {
            this.eventAggregator = eventAggregator;

            Navigation = NavigationManager.GetNavigationSources("MultipleSubscribersSample");
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Multiple Shells/Views and ContentControlNavigationSource");
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
