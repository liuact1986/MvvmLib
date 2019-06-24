using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class NavigationAllSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public NavigationSourceContainer Navigation { get; }
        public KeyedNavigationSource TabControlNavigationSource { get; }
        public KeyedNavigationSource ListViewNavigationSource { get; }

        public ICommand InsertCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public NavigationAllSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            Navigation = NavigationManager.GetNavigationSources("MultipleSubscribersSample");

            // add sources 
            Navigation.AddNewSource(typeof(ViewC), "View C!");
            Navigation.AddNewSource(typeof(ViewD), "View D!");
            Navigation.MoveToLast();

            this.TabControlNavigationSource = new KeyedNavigationSource("TabControl"); // tabControl
            this.ListViewNavigationSource = new KeyedNavigationSource("ListView"); // listview multiple

            Navigation.Register(TabControlNavigationSource); 
            Navigation.Register(ListViewNavigationSource); 

            Navigation[0].ClearSourcesOnNavigate = false;
            Navigation[1].ClearSourcesOnNavigate = false;
            Navigation[2].ClearSourcesOnNavigate = false;

            InsertCommand = new RelayCommand(Insert);
            ClearCommand = new RelayCommand(Clear);
        }

        private void Clear()
        {
            Navigation.ClearSources();
        }

        private void Insert()
        {
            var random = new Random();
            var index = random.Next(Navigation[0].Sources.Count);

            Navigation.InsertNewSource(index, typeof(ViewB), $"New View B Message {index}");
            Navigation.MoveTo(index);
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Multiple Shells/Views with NavigationSource");
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            Navigation.Unregister(TabControlNavigationSource);
            Navigation.Unregister(ListViewNavigationSource);
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
