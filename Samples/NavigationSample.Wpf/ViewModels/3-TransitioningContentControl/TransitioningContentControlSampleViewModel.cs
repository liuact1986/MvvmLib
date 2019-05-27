using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitioningContentControlSampleViewModel :SyncTitleViewModel
    {
        public TransitioningContentControlSampleViewModel(IEventAggregator eventAggregator)
              : base(eventAggregator)
        {
            this.Title = "Animation with TransitioningContentControl";
        }
    }
}
