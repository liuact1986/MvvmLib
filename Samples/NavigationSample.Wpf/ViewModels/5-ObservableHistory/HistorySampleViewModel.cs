using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace NavigationSample.Wpf.ViewModels
{
    public class HistorySampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public NavigationSource Navigation { get; }
        public NavigationBrowser NavigationBrowser { get; }

        public ObservableCollection<SourceMenuItem> BackStack { get; set; }
        public ObservableCollection<SourceMenuItem> ForwardStack { get; set; }

        public HistorySampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.BackStack = new ObservableCollection<SourceMenuItem>();
            this.ForwardStack = new ObservableCollection<SourceMenuItem>();

            Navigation = NavigationManager.GetDefaultNavigationSource("HistorySample");
            NavigationBrowser = new NavigationBrowser(Navigation.Sources);

            Navigation.CurrentChanged += OnNavigationCurrentSourceChanged;
            NavigationBrowser.CollectionView.CurrentChanged += OnNavigationBrowserCurrentChanged;
        }

        private void OnNavigationBrowserCurrentChanged(object sender, System.EventArgs e)
        {
            //var position = this.NavigationBrowser.CollectionView.CurrentPosition;
            //this.Navigation.MoveTo(position);
        }

        private void OnNavigationCurrentSourceChanged(object sender, CurrentSourceChangedEventArgs e)
        {
            // do not use current item with view, 
            // the item is not the same instance for navigation source and browser (avoid binding troubles)
            this.NavigationBrowser.MoveCurrentToPosition(e.CurrentIndex);

            CreateContextMenus();
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Observable History");
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

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

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
