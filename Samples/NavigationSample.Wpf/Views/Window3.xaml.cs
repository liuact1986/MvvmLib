using MvvmLib.Navigation;
using System.Windows;

namespace NavigationSample.Wpf.Views
{
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();

            var from = NavigationManager.GetNavigationSources("MultipleSubscribersSample")[0];

            var to = new NavigationSource();
            to.Sync(from);
            ListView1.ItemsSource = to.Sources;
            ListView1.SelectedItem = to.Current;
            //var collectionSync = new CollectionSync(from, to);
        }
    }
   
    //public class CollectionSync
    //{

    //    public CollectionSync(NavigationSource from, NavigationSource target)
    //    {
    //        From = from;
    //        Target = target;

    //        ((INotifyCollectionChanged)from.Entries).CollectionChanged += OnItemsSourceCollectionChanged;
    //    }

    //    public NavigationSource From { get; }
    //    public NavigationSource Target { get; }

    //    private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        switch (e.Action)
    //        {
    //            case NotifyCollectionChangedAction.Add:
    //            case NotifyCollectionChangedAction.Remove:
    //            case NotifyCollectionChangedAction.Replace:
    //            case NotifyCollectionChangedAction.Move:
    //                Target.Sync(From);
    //                break;
    //            case NotifyCollectionChangedAction.Reset:
    //                Target.ClearSources();
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
}
