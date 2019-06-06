using System.Windows.Controls;
using ValidationSample.ViewModels;

namespace ValidationSample.Views
{
    public partial class ViewModelValidatableSampleView : UserControl
    {
        public ViewModelValidatableSampleView()
        {
            InitializeComponent();

            this.DataContext = new ViewModelValidatableSampleViewModel();
        }
    }
}
