using MvvmLib.Message;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class ContentControlNavigationSourceSampleViewModel : SyncTitleViewModel
    {
        public NavigationSource Navigation { get; }

        public ContentControlNavigationSourceSampleViewModel(IEventAggregator eventAggregator)
             : base(eventAggregator)
        {
            this.Title = "ContentControlNavigationSource (\"SourceName\" Attached property)";

            Navigation = NavigationManager.GetOrCreateNavigationSource("ContentControlNavigationSourceSample");
        }
    }
}
