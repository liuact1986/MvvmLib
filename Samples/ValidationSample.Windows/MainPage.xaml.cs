using ValidationSample.Windows.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ValidationSample.Windows
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainPageViewModel();

            this.DataContext = ViewModel;
        }

        public MainPageViewModel ViewModel { get;  }

    }
}
