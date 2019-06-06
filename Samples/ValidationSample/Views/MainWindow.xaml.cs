using System.Windows;
using ValidationSample.ViewModels;

namespace ValidationSample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWrapperSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title = "Wrapper Sample";
            MainContent.Content = new WrapperSampleView();
        }

        private void OnValidatableAndEditableSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title = "Validatable Sample";
            MainContent.Content = new ValidatableAndEditableSampleView();
        }

        private void OnViewModelValidatableSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title = "ViewModel Validatable Sample";
            MainContent.Content = new ViewModelValidatableSampleView();
        }
    }
}
