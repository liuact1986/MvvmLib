using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class MultipleSubscribersSampleViewModel
    {
        public NavigationSourceContainer Navigation { get; }

        public MultipleSubscribersSampleViewModel(IEventAggregator eventAggregator) 
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Multiple Shells/Views and ContentControlNavigationSource");

            Navigation = NavigationManager.GetNavigationSources("MultipleSubscribersSample");
        }
    }
}
