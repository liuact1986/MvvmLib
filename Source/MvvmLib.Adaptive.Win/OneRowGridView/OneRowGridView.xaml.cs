using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MvvmLib.Adaptive
{
    public sealed partial class OneRowGridView : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(OneRowGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(OneRowGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(OneRowGridView), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(OneRowGridView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(OneRowGridView), new PropertyMetadata(0));

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(OneRowGridView), new PropertyMetadata(null));

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

        public OneRowGridView()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ActivateOneRowMode(ItemHeight);
        }

        private void ActivateOneRowMode(double itemsHeight)
        {
            GridView.MaxHeight = itemsHeight;
            GridView.VerticalAlignment = VerticalAlignment.Top;

            ScrollViewer.SetVerticalScrollMode(GridView, ScrollMode.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(GridView, ScrollBarVisibility.Hidden);


            ScrollViewer.SetHorizontalScrollMode(GridView, ScrollMode.Enabled);
            ScrollViewer.SetHorizontalScrollBarVisibility(GridView, ScrollBarVisibility.Auto);
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
