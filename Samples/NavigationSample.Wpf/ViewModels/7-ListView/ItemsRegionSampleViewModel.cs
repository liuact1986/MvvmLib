using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ItemsRegionSampleViewModel
    {
        public IRelayCommand AddCommand { get; }

        public ItemsRegionSampleViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("ListView with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)");

            var listViewRegion = regionNavigationService.GetItemsRegion("ListViewRegion");

            AddCommand = new RelayCommand<Type>(async (sourceType) => await listViewRegion.AddAsync(sourceType));
        }
    }
}
