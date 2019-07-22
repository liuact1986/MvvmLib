using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationViewModel : BindableBase, INavigationAware
    {
        private bool isCancelled;
        public bool IsCancelled
        {
            get { return isCancelled; }
            set { SetProperty(ref isCancelled, value); }
        }

        public NavigationSource Navigation { get; }

        public ObservableCollection<Duration> Durations { get; set; }

        private Duration selectedDuration;
        private readonly IEventAggregator eventAggregator;

        public Duration SelectedDuration
        {
            get { return selectedDuration; }
            set { SetProperty(ref selectedDuration, value); }
        }


        public ICommand NavigateCommand { get; }
        public ICommand CancelAnimationsCommand { get; }

        public AnimationViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            Navigation = NavigationManager.GetDefaultNavigationSource("AnimationSample");

            this.Durations = new ObservableCollection<Duration>
            {
                new Duration(TimeSpan.FromMilliseconds(400)),
                new Duration(TimeSpan.FromSeconds(4))
            };
            this.selectedDuration = this.Durations[0];

            NavigateCommand = new DelegateCommand<Type>((sourceType) =>
            {
                Navigation.Navigate(sourceType);
            });

            CancelAnimationsCommand = new DelegateCommand(() =>
            {
                IsCancelled = true;
                IsCancelled = false;
            });
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("ContentControl animation with AnimatableContentControl");
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
