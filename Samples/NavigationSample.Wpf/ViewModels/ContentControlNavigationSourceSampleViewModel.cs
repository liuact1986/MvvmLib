using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class ContentControlNavigationSourceSampleViewModel
    {
        public NavigationSource Navigation { get; }

        public ContentControlNavigationSourceSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("ContentControl \"SourceName\" Attached property Sample");

            Navigation = NavigationManager.GetOrCreateNavigationSource("ContentControlNavigationSourceSample");
        }
    }
}
