using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class NavigationSourceSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public NavigationSource Navigation { get; }

        public ObservableCollection<SourceMenuItem> BackStack { get; set; }
        public ObservableCollection<SourceMenuItem> ForwardStack { get; set; }

        public ICommand InsertCommand { get; set; }
        public ICommand RemoveFirstCommand { get; set; }

        public NavigationSourceSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.BackStack = new ObservableCollection<SourceMenuItem>();
            this.ForwardStack = new ObservableCollection<SourceMenuItem>();

            Navigation = NavigationManager.GetDefaultNavigationSource("HistorySample");

            InsertCommand = new DelegateCommand(Insert);
            RemoveFirstCommand = new DelegateCommand(RemoveFirst);
        }

        private void RemoveFirst()
        {
            if (Navigation.Sources.Count > 0)
                Navigation.RemoveSourceAt(0);
        }

        private void Insert()
        {
            Navigation.InsertNewSource(0, typeof(ViewD), "View D Inserted at index 0");
        }

        private void OnNavigationCurrentSourceChanged(object sender, CurrentSourceChangedEventArgs e)
        {
            CreateContextMenus();
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("NavigationSource Sample");
        }

        private void CreateContextMenus()
        {
            int index = Navigation.CurrentIndex;
            this.BackStack.Clear();
            this.ForwardStack.Clear();
            if (index != -1)
            {
                int count = Navigation.Sources.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i == index) { }
                    else if (i < index)
                    {
                        var source = Navigation.Sources.ElementAt(i);
                        this.BackStack.Add(new SourceMenuItem { Index = i, Source = source, DisplayName = source.GetType().Name });
                    }
                    else if (i > index)
                    {
                        var source = Navigation.Sources.ElementAt(i);
                        this.ForwardStack.Add(new SourceMenuItem { Index = i, Source = source, DisplayName = source.GetType().Name });
                    }
                }
            }
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            Navigation.CurrentChanged -= OnNavigationCurrentSourceChanged;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Navigation.CurrentChanged += OnNavigationCurrentSourceChanged;
        }
    }
    public class SourceMenuItem : BindableBase
    {
        public object Source { get; set; }

        public int Index { get; set; }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set { SetProperty(ref displayName, value); }
        }
    }
}
