using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ItemsRegionSampleViewModel : SyncTitleViewModel
    {
        public SharedSource<IDetailViewModel> DetailsSource { get; }
        public IRelayCommand AddCommand { get; }

        public ItemsRegionSampleViewModel(IEventAggregator eventAggregator, IInjector injector)
              : base(eventAggregator)
        {
            this.Title = "ListView with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)";

            DetailsSource = NavigationManager.GetSharedSource<IDetailViewModel>();

            AddCommand = new RelayCommand<Type>(async (sourceType) =>
            {
                var instance = injector.GetNewInstance(sourceType);
                await DetailsSource.Items.AddAsync((IDetailViewModel)instance);
            });
        }
    }
}
