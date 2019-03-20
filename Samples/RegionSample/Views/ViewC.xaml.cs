using RegionSample.ViewModels;
using System.Windows.Controls;
using MvvmLib.Navigation;

namespace RegionSample.Views
{
    public partial class ViewC : UserControl, ILoadedEventListener
    {
        public ViewC()
        {
            InitializeComponent();

            DataContext = new ViewCViewModel();
        }

        public void OnLoaded(object parameter)
        {
            LoadedMessage.Text = "ViewC loaded (code-behind) with parameter " + parameter.ToString();
        }
    }
}
