using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MvvmLib.Adaptive
{
    public sealed partial class ResponsiveGridView : UserControl
    {

        public static readonly DependencyProperty ColumnsProperty =
             DependencyProperty.Register("Columns", typeof(int), typeof(ResponsiveGridView), new PropertyMetadata(0,OnColumnsChanged));

        public static readonly DependencyProperty OneRowModeProperty =
            DependencyProperty.Register("OneRowMode", typeof(bool), typeof(ResponsiveGridView), new PropertyMetadata(false));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ResponsiveGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ResponsiveGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(ResponsiveGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(ResponsiveGridView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(ResponsiveGridView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(ResponsiveGridView), new PropertyMetadata(null));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public bool OneRowMode
        {
            get { return (bool)GetValue(OneRowModeProperty); }
            set { SetValue(OneRowModeProperty, value); }
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public TransitionCollection ItemContainerTransitions { get; set; }

        public ResponsiveGridView()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (OneRowMode)
            {
                ActivateOneRowMode(ItemHeight);
            }
        }

        private void ActivateOneRowMode(double itemsHeight)
        {
            GridView.MaxHeight = itemsHeight;
            GridView.VerticalAlignment = VerticalAlignment.Top;

            ScrollViewer.SetVerticalScrollMode(GridView, ScrollMode.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(GridView, ScrollBarVisibility.Hidden);
        }

        private async static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode.IsInDesignModeStatic)
            {
                await Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        var gridView = d as ResponsiveGridView;
                        var columns = (int)e.NewValue;
                        gridView.ItemWidth = gridView.ActualWidth / columns;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
            }
        }

        private void ItemsWrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // called when width changed for orientation horizontal | called when height changed for orientation vertical

            if (Columns > 0)
            {
                var itemsWrapGrid = sender as ItemsWrapGrid;
                if (itemsWrapGrid != null)
                {
                    // orientation horizontal > stretch horizontal / number of columns
                    ItemWidth = (int) e.NewSize.Width / Columns;
                    //itemsWrapGrid.ItemWidth = e.NewSize.Width / Columns;
                }
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (ItemClickCommand != null)
            {
                if (ItemClickCommand.CanExecute(e.ClickedItem)) ItemClickCommand.Execute(e.ClickedItem);
            }
        }
    }
}
