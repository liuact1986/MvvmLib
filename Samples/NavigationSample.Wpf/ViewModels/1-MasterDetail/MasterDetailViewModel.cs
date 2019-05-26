using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class MasterDetailViewModel
    {
        public NavigationSource Navigation { get; }

        public MasterDetailViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Master Detail with ISelectable (PersonDetailsViewModel)");

            Navigation = NavigationManager.GetOrCreateNavigationSource("Details");
        }
    }
}
