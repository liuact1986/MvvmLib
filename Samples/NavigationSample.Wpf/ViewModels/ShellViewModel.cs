using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public NavigationSource Navigation { get; }

        public ICommand NavigateCommand { get; }

        public ICommand WhenAvailableCommand { get; }

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            Title = "Navigation Sample [WPF]";

            RegisterSources();

            Navigation = NavigationManager.CreateNavigationSource("MainContent");

            NavigateCommand = new RelayCommand<Type>(OnNavigate);
            eventAggregator.GetEvent<ChangeTitleEvent>().Subscribe(OnChangeTitle);

        }

        private void RegisterSources()
        {
            // 1. Master Details
            NavigationManager.CreateNavigationSource("Details");
            // 2. AnimatableContentControl
            NavigationManager.CreateNavigationSource("AnimationSample");

            // 5. History Sample
            NavigationManager.CreateNavigationSource("HistorySample");
            // 6. TabControl and ListView
            NavigationManager.GetOrCreateSharedSource<IDetailViewModel>();
        }

        private void OnChangeTitle(string newTitle)
        {
            Title = newTitle;
        }

        private async void OnNavigate(Type sourceType)
        {
            await Navigation.NavigateAsync(sourceType);
        }
    }
}
