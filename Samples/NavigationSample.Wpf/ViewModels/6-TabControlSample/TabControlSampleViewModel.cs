using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class TabControlSampleViewModel : BindableBase
    {
        private ItemsRegion tabControlRegion;
        public ItemsRegion TabControlRegion
        {
            get { return tabControlRegion; }
            set { SetProperty(ref tabControlRegion, value); }
        }

        public IRelayCommand AddCommand { get; }
        public IRelayCommand AddViewModelCommand { get; }

        public TabControlSampleViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("TabControl with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)");

            this.TabControlRegion = regionNavigationService.GetItemsRegion("TabControlRegion");

            AddCommand = new RelayCommand<Type>(async (sourceType) =>await TabControlRegion.AddAsync(sourceType));
            
            AddViewModelCommand = new RelayCommand<Type>(async (sourceType)
                => await regionNavigationService.AddAsync("TabControlRegion2", sourceType));
        }
    }
}
