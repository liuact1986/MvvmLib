using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class TabControlSampleViewModel : SyncTitleViewModel
    {
        public SharedSource<IDetailViewModel> DetailsSource { get; }

        public IRelayCommand AddCommand { get; }
        public IRelayCommand AddViewModelCommand { get; }

        public TabControlSampleViewModel(IEventAggregator eventAggregator, IInjector injector)
              : base(eventAggregator)
        {
            this.Title = "TabControl with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)";

            DetailsSource = NavigationManager.GetOrCreateSharedSource<IDetailViewModel>();

            AddCommand = new RelayCommand<Type>(async (sourceType) =>
            {
                var instance = injector.GetNewInstance(sourceType);

                await DetailsSource.Items.AddAsync((IDetailViewModel)instance);
            });
        }
    }
}
