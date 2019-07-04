using NavigationSample.Wpf.ViewModels;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class DataPagerSampleView : UserControl
    {
        public DataPagerSampleView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var viewModel = DataContext as DataPagerSampleViewModel;

                var item = e.AddedItems[0] as ComboBoxItem;
                if (int.TryParse(item.Content.ToString(), out int selection))
                {
                    viewModel.PagedSource.PageSize = selection;
                }
            }
        }
    }
}
