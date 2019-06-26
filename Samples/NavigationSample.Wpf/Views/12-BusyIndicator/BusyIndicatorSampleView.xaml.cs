using MvvmLib.Message;
using NavigationSample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class BusyIndicatorSampleView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public BusyIndicatorSampleView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<BusyEvent>().Subscribe(OnBusyEvent);
        }

        private void OnBusyEvent(BusyEventArgs args)
        {
            Dispatcher.Invoke(() => BusyIndicator.SetIsBusy(args.IsBusy));
        }
    }
}
