using NavigationSample.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.Views
{
    public partial class Shell : Window
    {
        private bool isPaneOpened;

        public Shell()
        {
            InitializeComponent();

            isPaneOpened = true;

            var menuItems = new List<MenuItem>
            {
                new MenuItem("Master Detail (ISelectable)", IconKind.TableEdit, () => Navigate(typeof(MasterDetailView))),
                new MenuItem("Animatable Content Control", IconKind.Gamepad, () => Navigate(typeof(AnimationView))),
                new MenuItem("Transitioning Content Control", IconKind.Tennis, () => Navigate(typeof(TransitioningContentControlSampleView))),
                new MenuItem("Transitioning Items Control", IconKind.Football, () => Navigate(typeof(AnimationQueueView))),
                new MenuItem("Observable History", IconKind.History, () => Navigate(typeof(HistorySampleView))),
                new MenuItem("TabControl", IconKind.Tab, () => Navigate(typeof(TabControlSampleView))),
                new MenuItem("ListView", IconKind.LibraryBooks, () => Navigate(typeof(ItemsRegionSampleView))),
                new MenuItem("ContentControl NavigationSource", IconKind.Car, () => Navigate(typeof(ContentControlNavigationSourceSampleView))),
                new MenuItem("Shared Source", IconKind.Airplane, () => Navigate(typeof(SharedSourceSampleView)))
            };

            ListView1.ItemsSource = menuItems;
            this.Loaded += OnShellLoaded;
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            ListView1.SelectedIndex = 0;
        }

        private void Navigate(Type sourceType)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel.NavigateCommand.Execute(sourceType);
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            var storyboardName = isPaneOpened ? "CloseMenu" : "OpenMenu";
            var storyboard = (Storyboard)HamburgerButton.FindResource(storyboardName);

            storyboard.Begin();

            isPaneOpened = !isPaneOpened;
        }

        private void ListView1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as MenuItem;
                if (item != null && item.Action != null)
                {
                    item.Action.Invoke();
                }
            }
        }
    }

    public class MenuItem
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        private IconKind icon;
        public IconKind Icon
        {
            get { return icon; }
        }

        private Action action;
        public Action Action
        {
            get { return action; }
        }

        public MenuItem(string name, IconKind icon)
        {
            this.name = name;
            this.icon = icon;
        }

        public MenuItem(string name, IconKind icon, Action action)
        {
            this.name = name;
            this.icon = icon;
            this.action = action;
        }
    }
}
