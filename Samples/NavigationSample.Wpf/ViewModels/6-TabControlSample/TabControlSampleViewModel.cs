using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class TabControlSampleViewModel
    {
        public SharedSource<IDetailViewModel> DetailsSource { get; }

        public IRelayCommand AddCommand { get; }
        public IRelayCommand AddViewModelCommand { get; }

        public TabControlSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("TabControl with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)");

            DetailsSource = NavigationManager.GetSharedSource<IDetailViewModel>();

            AddCommand = new RelayCommand<Type>(AddItem);
        }

        private async void AddItem(Type sourceType)
        {
            var instance = DetailsSource.CreateNew(sourceType);
            await DetailsSource.Items.AddAsync((IDetailViewModel)instance);
        }
    }
}
