using MvvmLib.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NavigationSample.Wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour Window3.xaml
    /// </summary>
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

            var c = new HistoryCollection<object>();

        }
    }

    public class HistoryCollection<T> : Collection<T>
    {
        private List<NavigationEntry> entries;
        public List<NavigationEntry> Entries
        {
            get { return entries; }
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
