using System;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Controls
{
    public class BusyIndicator : Control
    {
        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }


        public BusyIndicator()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(BusyIndicator), new PropertyMetadata(null));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, OnIsBusyChanged));

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var busyIndicator = d as BusyIndicator;
            busyIndicator.SetIsBusy((bool)e.NewValue);
        }

        public void SetIsBusy(bool isBusy)
        {
            if (isBusy)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = Visibility.Collapsed;
        }
    }
}
