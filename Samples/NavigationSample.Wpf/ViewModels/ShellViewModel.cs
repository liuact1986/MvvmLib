using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
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

        public List<MenuItem> MenuItems { get; }

        public NavigationSource Navigation { get; private set; }

        public ICommand NavigateCommand { get; }

        public ICommand WhenAvailableCommand { get; }
 
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            Title = "Navigation Sample [WPF]";

            RegisterSources();

            NavigateCommand = new RelayCommand<Type>(Navigate);

            eventAggregator.GetEvent<ChangeTitleEvent>().Subscribe(OnChangeTitle);
            eventAggregator.GetEvent<NavigateEvent>().Subscribe(OnNavigate);
        }

        private void RegisterSources()
        {
            Navigation = NavigationManager.CreateNavigationSource("MainContent");

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

        public async void Navigate(Type sourceType)
        {
            await Navigation.NavigateAsync(sourceType);
        }

        private async void OnNavigate(NavigateEventArgs args)
        {
            await Navigation.NavigateAsync(args.SourceType, args.Parameter);
        }

    }

}
