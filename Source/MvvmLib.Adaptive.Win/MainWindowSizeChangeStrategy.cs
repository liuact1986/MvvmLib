using System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MvvmLib.Adaptive
{
    public class MainWindowSizeChangeStrategy : IAdaptiveSizeChangeStrategy
    {
        public double CurrentWidth => DesignMode.IsInDesignModeStatic ? -1 : Window.Current.Bounds.Width;

        public bool HasWidth => !double.IsNaN(this.CurrentWidth);

        public MainWindowSizeChangeStrategy()
        {
            if (!DesignMode.IsInDesignModeStatic)
            {
                Window.Current.SizeChanged += OnSizeChanged;            
            }
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.RaiseSizeChanged(e.Size.Width);
        }

        public event EventHandler<AdaptiveSizeChangedEventArgs> SizeChanged;

        protected void RaiseSizeChanged(double witdh)
        {
            this.SizeChanged?.Invoke(this, new AdaptiveSizeChangedEventArgs(witdh));
        }
    }

}
