using MvvmLib.Animation;
using MvvmLib.Navigation;
using NavigationSample.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace NavigationSample.Wpf.Views
{
    public partial class Shell : Window
    {
        private bool isPaneOpened;
        private bool isAnimating;
        private bool handleSelectionChanged;
        private List<MenuItem> menuItems;

        public ShellViewModel ViewModel { get; set; }

        public Shell()
        {
            InitializeComponent();

            isPaneOpened = true;
            isAnimating = false;

            this.menuItems = new List<MenuItem>
            {
                new MenuItem(nameof(MasterDetailView), "Master Detail (ISelectable)", IconKind.TableEdit, () => Navigate(typeof(MasterDetailView))),
                new MenuItem(nameof(AnimationView), "Animatable Content Control", IconKind.Gamepad, () => Navigate(typeof(AnimationView))),
                new MenuItem(nameof(TransitioningContentControlSampleView), "Transitioning Content Control", IconKind.Tennis, () => Navigate(typeof(TransitioningContentControlSampleView))),
                new MenuItem(nameof(AnimationQueueView), "Transitioning Items Control", IconKind.Football, () => Navigate(typeof(AnimationQueueView))),
                new MenuItem(nameof(HistorySampleView),"Observable History", IconKind.History, () => Navigate(typeof(HistorySampleView))),
                new MenuItem(nameof(TabControlSampleView),"TabControl", IconKind.Tab, () => Navigate(typeof(TabControlSampleView))),
                new MenuItem(nameof(ItemsRegionSampleView), "ListView", IconKind.LibraryBooks, () => Navigate(typeof(ItemsRegionSampleView))),
                new MenuItem(nameof(SharedSourceSampleView), "Shared Source", IconKind.Airplane, () => Navigate(typeof(SharedSourceSampleView))),
                new MenuItem(nameof(NavigationBehaviorsSampleView), "Navigation Behaviors", IconKind.BellRing, () => Navigate(typeof(NavigationBehaviorsSampleView))),
                new MenuItem(nameof(MultipleSubscribersSampleView), "Multiple Shells/Views", IconKind.BookMultiple, () => Navigate(typeof(MultipleSubscribersSampleView))),
                new MenuItem(nameof(SharedSourceNavigationAndEditionSampleView), "Navigation and Edition", IconKind.Pencil, () => Navigate(typeof(SharedSourceNavigationAndEditionSampleView)))
            };

            this.Loaded += OnShellLoaded;
        }

        private void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as ShellViewModel;
            ViewModel.Navigation.Navigated += OnNavigated;

            ListView1.ItemsSource = menuItems;

            handleSelectionChanged = true;
            ListView1.SelectedIndex = 0;
        }

        private void ListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (handleSelectionChanged)
                if (e.AddedItems.Count > 0)
                {
                    var item = e.AddedItems[0] as MenuItem;
                    if (item != null && item.Action != null)
                    {
                        item.Action.Invoke();
                    }
                }
        }

        private void OnNavigated(object sender, NavigatedEventArgs e)
        {
            if (e.NavigationType == NavigationType.Back)
            {
                var pageName = e.SourceType.Name;
                var menuItem = menuItems.FirstOrDefault(m => m.Tag == pageName);
                if (menuItem != null)
                {
                    handleSelectionChanged = false;
                    ListView1.SelectedItem = menuItem;
                    handleSelectionChanged = true;
                }
            }
        }

        private async void Navigate(Type sourceType)
        {
            await ViewModel.Navigation.NavigateAsync(sourceType);
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var storyboardName = isPaneOpened ? "CloseMenu" : "OpenMenu";
            var storyboard = (Storyboard)HamburgerButton.FindResource(storyboardName);
            var storyboardAccessor = new StoryboardAccessor(storyboard);
            storyboardAccessor.HandleCompleted(() =>
            {
                storyboardAccessor.UnhandleCompleted();
                isAnimating = false;
                isPaneOpened = !isPaneOpened;
            });
            storyboard.Begin();
        }
    }

    public class MenuItem
    {
        private string tag;
        public string Tag
        {
            get { return tag; }
        }

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

        public MenuItem(string tag, string name, IconKind icon, Action action)
        {
            this.tag = tag;
            this.name = name;
            this.icon = icon;
            this.action = action;
        }
    }
}
