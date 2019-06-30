using MvvmLib.Animation;
using MvvmLib.Message;
using NavigationSample.Wpf.Controls;
using NavigationSample.Wpf.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.Views
{
    public partial class Shell : Window
    {
        private bool isPaneOpened;
        private bool isAnimating;

        public Shell(IEventAggregator eventAggregator)
        {
            isPaneOpened = true;
            isAnimating = false;

            InitializeComponent();

            eventAggregator.GetEvent<NotificationMessageEvent>().Subscribe(OnNotificationPublished);
        }

        private void OnNotificationPublished(string message)
        {
            NotificationControl.PushNotification(new NotificationItem { Content = message });
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

                var visibility = isPaneOpened ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
                ListView1.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, visibility);
            });
            storyboard.Begin();
        }
    }
}
