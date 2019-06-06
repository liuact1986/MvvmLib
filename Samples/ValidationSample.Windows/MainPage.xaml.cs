using ValidationSample.Windows.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ValidationSample.Windows
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnWrapperSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title.Text = "Wrapper Sample";
            MainContent.Content = new WrapperSamplePage();
        }

        private void OnValidatableAndEditableSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title.Text = "Validatable Sample";
            MainContent.Content = new ValidatableAndEditableSamplePage();
        }

        private void OnViewModelValidatableSampleClick(object sender, RoutedEventArgs e)
        {
            this.Title.Text = "ViewModel Validatable Sample";
            MainContent.Content = new ViewModelValidatableSamplePage();
        }
    }
}
