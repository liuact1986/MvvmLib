using MvvmLib.Message;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class MultipleSubscribersSampleViewModel: SyncTitleViewModel
    {
        public NavigationSourceContainer Navigation { get; }

        public MultipleSubscribersSampleViewModel(IEventAggregator eventAggregator) 
            : base(eventAggregator)
        {
            this.Title = "Multiple Shells/Views and ContentControlNavigationSource";

            Navigation = NavigationManager.GetNavigationSources("MultipleSubscribersSample");
        }

    }
}
