using MvvmLib.Message;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{

    public class MasterDetailViewModel : SyncTitleViewModel
    {
        public NavigationSource Navigation { get; }

        public MasterDetailViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.Title = "Master Detail with ISelectable (PersonDetailsViewModel)";
            Navigation = NavigationManager.GetOrCreateNavigationSource("Details");
        }

    }
}
