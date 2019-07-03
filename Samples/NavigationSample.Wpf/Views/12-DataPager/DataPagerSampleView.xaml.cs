using NavigationSample.Wpf.ViewModels;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class DataPagerSampleView : UserControl
    {
        private DataPagerSampleViewModel viewModel;

        public DataPagerSampleView()
        {
            InitializeComponent();

            this.viewModel = DataContext as DataPagerSampleViewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var item = e.AddedItems[0] as ComboBoxItem;
                if (int.TryParse(item.Content.ToString(), out int selection))
                {
                    viewModel.PagedSource.PageSize = selection;
                }
            }
        }
    }
}
