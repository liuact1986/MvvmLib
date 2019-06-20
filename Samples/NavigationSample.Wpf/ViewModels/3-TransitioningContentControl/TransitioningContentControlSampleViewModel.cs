using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitioningContentControlSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public TransitioningContentControlSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Animation with TransitioningContentControl");
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
