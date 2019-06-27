using MvvmLib.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NavigationSample.Wpf.Controls
{
    public class BusyIndicator : Control
    {
        private DispatcherTimer displayAfterTimer;

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        public BusyIndicator()
        {
            this.Visibility = Visibility.Collapsed;
            this.displayAfterTimer = new DispatcherTimer();
            this.displayAfterTimer.Tick += OnDisplayAfterTimerTick;
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

        public TimeSpan DisplayAfter
        {
            get { return (TimeSpan)GetValue(DisplayAfterProperty); }
            set { SetValue(DisplayAfterProperty, value); }
        }

        public static readonly DependencyProperty DisplayAfterProperty =
            DependencyProperty.Register("DisplayAfter", typeof(TimeSpan), typeof(BusyIndicator), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 100)));

        public string FocusAfterBusy
        {
            get { return (string)GetValue(FocusAfterBusyProperty); }
            set { SetValue(FocusAfterBusyProperty, value); }
        }

        public static readonly DependencyProperty FocusAfterBusyProperty =
            DependencyProperty.Register("FocusAfterBusy", typeof(string), typeof(BusyIndicator), new PropertyMetadata(null));

        private void SetIsBusy(bool isBusy)
        {
            if (isBusy)
            {
                if (DisplayAfter.Equals(TimeSpan.Zero))
                    this.Visibility = Visibility.Visible;
                else
                {
                    this.displayAfterTimer.Interval = DisplayAfter;
                    this.displayAfterTimer.Start();
                }
            }
            else
            {
                this.displayAfterTimer.Stop();
                this.Visibility = Visibility.Collapsed;

                if (FocusAfterBusy != null)
                    FocusHelper.Focus(FocusAfterBusy, this.Parent);
            }
        }

        private void OnDisplayAfterTimerTick(object sender, EventArgs e)
        {
            this.displayAfterTimer.Stop();
            this.Visibility = Visibility.Visible;
        }

       
    }

    public class FocusHelper
    {
        public static void Focus(string elementName, DependencyObject parent)
        {
            if (elementName == null)
                throw new ArgumentNullException(nameof(elementName));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var element = FindElementByName(elementName, parent);
            if (element != null)
                element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => element.Focus()));
        }

        private static FrameworkElement FindElementByName(string elementName, DependencyObject parent)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    if (child is FrameworkElement element)
                    {
                        if (element.Name == elementName)
                            return element;
                    }

                    var childElement = FindElementByName(elementName, child);
                    if (childElement != null)
                        return childElement;
                }
            }
            return null;
        }
    }
}
