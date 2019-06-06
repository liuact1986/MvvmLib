using System.Windows.Controls;
using ValidationSample.ViewModels;

namespace ValidationSample.Views
{
    public partial class ValidatableAndEditableSampleView : UserControl
    {
        private ValidatableAndEditableSampleViewModel viewModel;

        public ValidatableAndEditableSampleView()
        {
            InitializeComponent();

            this.viewModel = new ValidatableAndEditableSampleViewModel();

            this.DataContext = viewModel;
            this.Loaded += OnLoaded;
        }


        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Load();
        }

    }
}
