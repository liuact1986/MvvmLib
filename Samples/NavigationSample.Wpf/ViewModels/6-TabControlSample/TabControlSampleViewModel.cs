using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class TabControlSampleViewModel : INavigationAware
    {
        private IEventAggregator eventAggregator;

        public SharedSource<IDetailViewModel> DetailsSource { get; }

        public IDelegateCommand AddCommand { get; }
        public IDelegateCommand AddViewModelCommand { get; }

        public TabControlSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            DetailsSource = NavigationManager.GetSharedSource<IDetailViewModel>();

            AddCommand = new DelegateCommand<Type>(AddItem);
        }

        private void AddItem(Type sourceType)
        {
            if (sourceType == typeof(ViewCViewModel))
                DetailsSource.AddNew<ViewCViewModel>();

            if (sourceType == typeof(ViewDViewModel))
                DetailsSource.AddNew<ViewDViewModel>();
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("TabControl with IIsSelected (ViewCViewModel) and ISelectable (ViewDViewModel)");
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
