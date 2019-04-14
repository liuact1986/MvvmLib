using RegionSample.ViewModels;
using System.Windows.Controls;
using MvvmLib.Navigation;
using System.Windows;

namespace RegionSample.Views
{
    public partial class ViewC : UserControl, ILoadedEventListener
    {
        public ViewC()
        {
            InitializeComponent();

            DataContext = new ViewCViewModel();
        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            LoadedMessage.Text = "ViewC loaded (code-behind) with parameter " + parameter.ToString();
        }
    }
}
