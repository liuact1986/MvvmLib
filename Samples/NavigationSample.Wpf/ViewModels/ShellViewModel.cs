using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Controls;
using NavigationSample.Wpf.Events;
using NavigationSample.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NavigationSample.Wpf.ViewModels
{
    public class ShellViewModel : BindableBase, IIsLoaded
    {
        private readonly IEventAggregator eventAggregator;
        private bool handleSelectionChanged;

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public NavigationSource Navigation { get; }
        public SharedSource<MenuItem> MenuItemsSource { get; }

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            OnTitleChanged("Navigation Sample [WPF]");
            handleSelectionChanged = true;

            this.Navigation = NavigationManager.GetDefaultNavigationSource("Main");
            this.MenuItemsSource = NavigationManager.GetSharedSource<MenuItem>();

            this.Navigation.Navigated += OnNavigated;
            this.MenuItemsSource.SelectedItemChanged += OnMenuItemsSelectionChanged;
            eventAggregator.GetEvent<TitleChangedEvent>().Subscribe(OnTitleChanged);
        }

        private void Load()
        {
            this.MenuItemsSource.Load(new List<MenuItem>
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
                 new MenuItem(nameof(NavigationAllSampleView), "Multiple Shells/Views", IconKind.BookMultiple, () => Navigate(typeof(NavigationAllSampleView))),
                 new MenuItem(nameof(SharedSourceNavigationAndEditionSampleView), "Navigation and Edition", IconKind.Pencil, () => Navigate(typeof(SharedSourceNavigationAndEditionSampleView)))
            });
        }

        private void OnMenuItemsSelectionChanged(object sender, SharedSourceSelectedItemChangedEventArgs e)
        {
            if (handleSelectionChanged)
            {
                var selectedMenuItem = e.SelectedItem as MenuItem;
                if (selectedMenuItem != null && selectedMenuItem.Action != null)
                {
                    selectedMenuItem.Action.Invoke();
                }
            }
        }

        private void Navigate(Type sourceType)
        {
            Navigation.Navigate(sourceType);
        }

        private void OnTitleChanged(string title)
        {
            this.Title = title;
        }

        private void OnNavigated(object sender, NavigatedEventArgs e)
        {
            // sync menu item selection
            if (e.NavigationType == NavigationType.Back)
            {
                var pageName = e.SourceType.Name;
                var menuItem = MenuItemsSource.Items.FirstOrDefault(m => m.Tag == pageName);
                if (menuItem != null)
                {
                    handleSelectionChanged = false;
                    MenuItemsSource.SelectedItem = menuItem;
                    handleSelectionChanged = true;
                }
            }
        }

        public void OnLoaded()
        {
            Load();
        }
    }
}
