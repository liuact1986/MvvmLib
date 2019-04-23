using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{

    public partial class WhenAvailableSampleView : UserControl
    {
        public WhenAvailableSampleView()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {

            await Task.Delay(2000);

            ContentControl1.Content = new WhenAvailableView();
            
        }
    }
}
