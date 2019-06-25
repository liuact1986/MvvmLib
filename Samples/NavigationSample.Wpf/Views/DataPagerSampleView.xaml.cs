using NavigationSample.Wpf.Controls;
using NavigationSample.Wpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NavigationSample.Wpf.Views
{
    public partial class DataPagerSampleView : UserControl
    {
        private DataPagerSampleViewModel viewModel;

        public DataPagerSampleView()
        {
            InitializeComponent();

            this.Loaded += DataPagerSampleView_Loaded;
        }

        private void DataPagerSampleView_Loaded(object sender, RoutedEventArgs e)
        {

            this.viewModel = DataContext as DataPagerSampleViewModel;
            BuildPagingMenu();

            viewModel.DataPager.Refreshed += OnPagerRefreshed;
        }

        private void OnPagerRefreshed(object sender, EventArgs e)
        {
            BuildPagingMenu();
        }

        private void BuildPagingMenu()
        {
            PagingMenu.Children.Clear();
            //
            var firstPageButton = new Button();
            firstPageButton.Content = new MaterialDesignIcon { Icon = IconKind.SkipPrevious, Brush = new SolidColorBrush(Colors.Black) };
            firstPageButton.SetBinding(Button.CommandProperty, new Binding { Path = new PropertyPath("DataPager.MoveToFirstPageCommand") });
            PagingMenu.Children.Add(firstPageButton);

            var previousPageButton = new Button();
            previousPageButton.Content = new MaterialDesignIcon { Icon = IconKind.MenuLeft, Brush = new SolidColorBrush(Colors.Black) };
            previousPageButton.SetBinding(Button.CommandProperty, new Binding { Path = new PropertyPath("DataPager.MoveToPreviousPageCommand") });
            PagingMenu.Children.Add(previousPageButton);

            int pageCount = viewModel.DataPager.PageCount;
            int count = pageCount > 10 ? 10 : pageCount;
            var listView = new ListView { BorderThickness = new Thickness(0.0), Background = Brushes.Transparent };
            //var buttonStyle = Application.Current.FindResource("ButtonStyle") as Style;
            for (int i = 0; i < count; i++)
            {
                var pageButton = new Button();
                pageButton.Content = $"{i + 1}";
                pageButton.MinWidth = 30.0;
                pageButton.SetBinding(Button.CommandProperty, new Binding { Path = new PropertyPath("DataPager.MoveToPageCommand") });
                pageButton.SetValue(Button.CommandParameterProperty, i + 1);
                //pageButton.SetValue(Button.StyleProperty, buttonStyle);
                listView.Items.Add(pageButton);
            }

            listView.SetBinding(ListView.SelectedIndexProperty, new Binding { Path = new PropertyPath("DataPager.PageIndex") });

            var factoryPanel = new FrameworkElementFactory(typeof(StackPanel));
            factoryPanel.SetValue(StackPanel.IsItemsHostProperty, true);
            factoryPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            var template = new ItemsPanelTemplate();
            template.VisualTree = factoryPanel;

            listView.ItemsPanel = template;

            PagingMenu.Children.Add(listView);

            var nextPageButton = new Button();
            nextPageButton.Content = new MaterialDesignIcon { Icon = IconKind.MenuRight, Brush = new SolidColorBrush(Colors.Black) };
            nextPageButton.SetBinding(Button.CommandProperty, new Binding { Path = new PropertyPath("DataPager.MoveToNextPageCommand") });
            PagingMenu.Children.Add(nextPageButton);

            var lastPageButton = new Button();
            lastPageButton.Content = new MaterialDesignIcon { Icon = IconKind.SkipNext, Brush = new SolidColorBrush(Colors.Black) };
            lastPageButton.SetBinding(Button.CommandProperty, new Binding { Path = new PropertyPath("DataPager.MoveToLastPageCommand") });
            PagingMenu.Children.Add(lastPageButton);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var item = e.AddedItems[0] as ComboBoxItem;
                if (int.TryParse(item.Content.ToString(), out int selection))
                {
                    viewModel.DataPager.PageSize = selection;
                }
            }
        }
    }
}
