using ValidationSample.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ValidationSample.Windows.Views
{

    public sealed partial class WrapperSamplePage : Page
    {
        private WrapperSamplePageViewModel viewModel;

        public WrapperSamplePage()
        {
            InitializeComponent();

            this.viewModel = new WrapperSamplePageViewModel();
            this.DataContext = viewModel;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.Load();
        }

    }
}
