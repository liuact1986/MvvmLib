using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class ViewModelLocator
    {
        public static readonly DependencyProperty ResolveWindowViewModelProperty =
           DependencyProperty.RegisterAttached("ResolveWindowViewModel", typeof(bool), typeof(ViewModelLocator),
               new PropertyMetadata(false, OnResolveWindowViewModelChanged));

        public static bool GetResolveWindowViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(ResolveWindowViewModelProperty);
        }

        public static void SetResolveWindowViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(ResolveWindowViewModelProperty, value);
        }

        private static void OnResolveWindowViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d.GetType().BaseType == typeof(Window))
            {
                ((Window)d).Activated += OnWindowActivated;
            }
        }

        private static void OnWindowActivated(object sender, System.EventArgs e)
        {
            var window = sender as Window;

            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(window.GetType());

            object context = null;
            if (viewModelType != null)
            {
                context = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                window.DataContext = context;
            }

            window.Activated -= OnWindowActivated;

            if (window is ILoadedEventListener)
            {
                ((ILoadedEventListener)window).OnLoaded(null);
            }
            if (context != null && context is ILoadedEventListener)
            {
                ((ILoadedEventListener)context).OnLoaded(null);
            }
        }
    }

}
