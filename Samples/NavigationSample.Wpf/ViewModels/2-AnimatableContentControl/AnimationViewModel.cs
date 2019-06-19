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
    public class AnimationViewModel : BindableBase
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
        public Duration SelectedDuration
        {
            get { return selectedDuration; }
            set { SetProperty(ref selectedDuration, value); }
        }


        public ICommand NavigateCommand { get; }
        public ICommand CancelAnimationsCommand { get; }

        public AnimationViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("ContentControl animation with AnimatableContentControl");

            Navigation = NavigationManager.GetDefaultNavigationSource("AnimationSample");

            this.Durations = new ObservableCollection<Duration>
            {
                new Duration(TimeSpan.FromMilliseconds(400)),
                new Duration(TimeSpan.FromSeconds(4))
            };
            this.selectedDuration = this.Durations[0];

            NavigateCommand = new RelayCommand<Type>((sourceType) =>
            {
                Navigation.Navigate(sourceType);
            });

            CancelAnimationsCommand = new RelayCommand(() =>
            {
                IsCancelled = true;
                IsCancelled = false;
            });
        }
    }
}
