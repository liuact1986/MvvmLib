using System;
using System.Windows;

namespace MvvmLib.Adaptive
{
    public class MainWindowSizeChangeStrategy : IAdaptiveSizeChangeStrategy
    {
        public double CurrentWidth => DesignMode.IsInDesignMode ? -1 : Application.Current.MainWindow.ActualWidth;

        public bool HasWidth => !double.IsNaN(this.CurrentWidth);

        public MainWindowSizeChangeStrategy()
        {
            if (!DesignMode.IsInDesignMode)
            {
                Application.Current.MainWindow.SizeChanged += OnSizeChanged;               
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RaiseSizeChanged(e.NewSize.Width);
        }

        public event EventHandler<AdaptiveSizeChangedEventArgs> SizeChanged;

        protected void RaiseSizeChanged(double witdh)
        {
            this.SizeChanged?.Invoke(this, new AdaptiveSizeChangedEventArgs(witdh));
        }
    }

}
