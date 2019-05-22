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
            set { SetProperty(ref isCancelled , value); }
        }

        public ObservableCollection<Duration> Durations { get; set; }

        public ICommand NavigateCommand { get; }
        public ICommand CancelAnimationsCommand { get; }

        public AnimationViewModel(IRegionNavigationService regionNavigationService, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeTitleEvent>().Publish("ContentControl animation with AnimatableContentControl");

            this.Durations = new ObservableCollection<Duration>
            {
                new Duration(TimeSpan.FromMilliseconds(400)),
                new Duration(TimeSpan.FromSeconds(4)),
            };

            NavigateCommand = new RelayCommand<Type>(async (sourceType) =>
            {
                await regionNavigationService.NavigateAsync("AnimationSample", sourceType);
            });

            CancelAnimationsCommand = new RelayCommand(() =>
            {
                IsCancelled = true;
                IsCancelled = false;
            });
        }
    }
}
