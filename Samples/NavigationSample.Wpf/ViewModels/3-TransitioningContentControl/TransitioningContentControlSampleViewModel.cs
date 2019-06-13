using MvvmLib.Message;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitioningContentControlSampleViewModel
    {
        public TransitioningContentControlSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Animation with TransitioningContentControl");
        }
    }
}
