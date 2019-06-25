using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NavigationSample.Wpf.Controls
{
    public class NotificationControl : Control
    {
        private const string NotificationItemsControlPartName = "PART_NotificationItemsControl";
        private ItemsControl notificationItemsControl;

        public TimeSpan Timeout
        {
            get { return (TimeSpan)GetValue(TimeoutProperty); }
            set { SetValue(TimeoutProperty, value); }
        }

        public static readonly DependencyProperty TimeoutProperty =
            DependencyProperty.Register("Timeout", typeof(TimeSpan), typeof(NotificationControl), new PropertyMetadata(new TimeSpan(0, 0, 5)));


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(NotificationControl), new PropertyMetadata(null));

        static NotificationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationControl), new FrameworkPropertyMetadata(typeof(NotificationControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.notificationItemsControl = this.GetTemplateChild(NotificationItemsControlPartName) as ItemsControl;
        }

        public void PushNotification(INotificationItem notification)
        {
            notificationItemsControl.Items.Insert(0, notification);

            var timer = new DispatcherTimer { Interval = Timeout };
            EventHandler timerCallback = null;
            timerCallback = (s, e) =>
            {
                timer.Stop();
                timer.Tick -= timerCallback;
                notificationItemsControl.Items.Remove(notification);
            };
            timer.Tick += timerCallback;
            timer.Start();
        }
    }

    public interface INotificationItem
    {
        object Content { get; set; }
    }

    public class NotificationItem : INotificationItem
    {
        public object Content { get; set; }
    }
}
