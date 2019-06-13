using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace NavigationSample.Wpf.ViewModels
{
    public class HistorySampleViewModel 
    {
        public NavigationSource Navigation { get; }
        public NavigationBrowser NavigationBrowser { get; }

        public ObservableCollection<SourceMenuItem> BackStack { get; set; }
        public ObservableCollection<SourceMenuItem> ForwardStack { get; set; }

        public HistorySampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Observable History");

            this.BackStack = new ObservableCollection<SourceMenuItem>();
            this.ForwardStack = new ObservableCollection<SourceMenuItem>();

            Navigation = NavigationManager.GetDefaultNavigationSource("HistorySample");
            Navigation.CurrentChanged += OnCurrentSourceChanged;

            NavigationBrowser = new NavigationBrowser(Navigation.Sources);
        }

        private void OnCurrentSourceChanged(object sender, CurrentSourceChangedEventArgs e)
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
                    else if(i > index)
                    {
                        var source = Navigation.Sources.ElementAt(i);
                        this.ForwardStack.Add(new SourceMenuItem { Index = i, Source = source, DisplayName = source.GetType().Name });
                    }
                }
            }
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
