using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitioningContentControlSampleViewModel
    {
        public TransitioningContentControlSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Animation with TransitioningContentControl");
        }
    }
}
