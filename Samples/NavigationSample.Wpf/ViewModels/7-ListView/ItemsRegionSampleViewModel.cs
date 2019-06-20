using MvvmLib.Commands;
using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ItemsRegionSampleViewModel : INavigationAware
    {
        private IEventAggregator eventAggregator;

        public SharedSource<IDetailViewModel> DetailsSource { get; }
        public IRelayCommand AddCommand { get; }

        public ItemsRegionSampleViewModel(IEventAggregator eventAggregator, IInjector injector)
        {
            this.eventAggregator = eventAggregator;

            DetailsSource = NavigationManager.GetSharedSource<IDetailViewModel>();

            AddCommand = new RelayCommand<Type>(AddItem);
        }

        private void AddItem(Type sourceType)
        {
            var instance = DetailsSource.CreateNew(sourceType);
            DetailsSource.Items.Add(instance);
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("ListView with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)");
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
