using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class HistorySampleViewModel
    {
        public NavigationSource Navigation { get; }

        public HistorySampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("Observable History");

            Navigation = NavigationManager.GetNavigationSource("HistorySample");
        }
    }
}
