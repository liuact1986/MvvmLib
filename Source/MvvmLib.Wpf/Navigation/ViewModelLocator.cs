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
            var context = GetViewModel(window.GetType());
            SetViewModel(window, context);
            window.Activated -= OnWindowActivated;

            if (window is ILoadedEventListener)
            {
                ((ILoadedEventListener)window).OnLoaded(null);
            }
            if(context != null && context is ILoadedEventListener)
            {
                ((ILoadedEventListener)context).OnLoaded(null);
            }
        }

        public static object GetViewModel(Type viewType)
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType);
            if (viewModelType != null)
            {
                return ViewModelLocationProvider.ResolveViewModel(viewModelType);
            }
            return null;
        }

        public static bool GetAutoWireValueForElement(DependencyObject element)
        {
            var window = Window.GetWindow(element);
            if (window != null)
            {
                var value = window.GetValue(ResolveWindowViewModelProperty);
                return value != null && (bool)value;
            }
            return false;
        }

        public static bool IsNullDataContext(FrameworkElement view)
        {
            return view.DataContext == null;
        }

        public static void SetViewModel(FrameworkElement view)
        {
            if (IsNullDataContext(view))
            {
                var viewModel = ViewModelLocator.GetViewModel(view.GetType());
                view.DataContext = viewModel;
            }
        }

        public static void SetViewModel(FrameworkElement view, object viewModel)
        {
            if (IsNullDataContext(view))
            {
                view.DataContext = viewModel;
            }
        }
    }

}
