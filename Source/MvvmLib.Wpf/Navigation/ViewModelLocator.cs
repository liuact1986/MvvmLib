using System;
using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to resolve the ViewModel for a view.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Gets The resolve view model value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetResolveWindowViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(ResolveWindowViewModelProperty);
        }

        /// <summary>
        /// Sets The resolve view model value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetResolveWindowViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(ResolveWindowViewModelProperty, value);
        }

        /// <summary>
        /// Allows to resolve ViewModel for a Window.
        /// </summary>
        public static readonly DependencyProperty ResolveWindowViewModelProperty =
           DependencyProperty.RegisterAttached("ResolveWindowViewModel", typeof(bool), typeof(ViewModelLocator),
               new PropertyMetadata(false, OnResolveWindowViewModelChanged));

        private static void OnResolveWindowViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
                if ((bool)e.NewValue)
                    if (d.GetType().BaseType != typeof(Window))
                        throw new ArgumentException($"\"ResolveWindowViewModel\" is only available for Window");
                    else
                        ((Window)d).Activated += OnWindowActivated;
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

            if (context != null && context is ILoadedEventListener loadedEventListener)
                loadedEventListener.OnLoaded(window, null);

            window.Activated -= OnWindowActivated;
        }
    }

}
