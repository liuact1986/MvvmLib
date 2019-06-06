using System.Windows.Controls;
using ValidationSample.ViewModels;

namespace ValidationSample.Views
{
    public partial class WrapperSampleView : UserControl
    {
        private WrapperSampleViewModel viewModel;

        public WrapperSampleView()
        {
            InitializeComponent();

            this.viewModel = new WrapperSampleViewModel();
            this.DataContext = viewModel;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Load();
        }

    }
}
