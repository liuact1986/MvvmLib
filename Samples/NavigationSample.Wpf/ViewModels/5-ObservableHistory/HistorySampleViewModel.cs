using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class HistorySampleViewModel :SyncTitleViewModel
    {
        public NavigationSource Navigation { get; }

        public HistorySampleViewModel(IEventAggregator eventAggregator)
              : base(eventAggregator)
        {
            this.Title = "Observable History";

            Navigation = NavigationManager.GetNavigationSource("HistorySample");
        }
    }
}
