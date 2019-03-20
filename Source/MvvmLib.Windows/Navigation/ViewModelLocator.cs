using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// ALlows to discover and bind the view model to  the view.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Returns true if the view model have to be resolved.
        /// </summary>
        /// <param name="obj">The view</param>
        /// <returns>True or false</returns>
        public static bool GetResolveViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(ResolveViewModelProperty);
        }

        /// <summary>
        /// Sets if the view model have to be resolved.
        /// </summary>
        /// <param name="obj">The view</param>
        /// <param name="value">The new value</param>
        public static void SetResolveViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(ResolveViewModelProperty, value);
        }

        /// <summary>
        /// Allows to resolve the view model for the view.
        /// </summary>
        public static readonly DependencyProperty ResolveViewModelProperty =
            DependencyProperty.RegisterAttached("ResolveViewModel", typeof(bool), typeof(ViewModelLocator),
                new PropertyMetadata(false, OnResolveViewModelChanged));


        private static void OnResolveViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (((bool?)e.NewValue) == true)
                {
                    FrameworkElement view = d as FrameworkElement;
                    if (view != null)
                    {
                        Type viewModelType = ViewModelLocationProvider.ResolveViewModelType(view.GetType());
                        object viewModel = null;
                        if (viewModelType != null)
                        {
                            viewModel = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                        }
                        view.DataContext = viewModel;
                    }
                }
            }
        }

        /// <summary>
        /// returns true if the view model have to be notified on view loaded
        /// </summary>
        /// <param name="obj">The view</param>
        /// <returns>True or false</returns>
        public static bool GetHandleLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(HandleLoadedProperty);
        }

        /// <summary>
        /// Sets  if the view model have to be notified on view loaded
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetHandleLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(HandleLoadedProperty, value);
        }

        /// <summary>
        /// Allows to handle the view loaded event in the view model with ILoadedEventlistener contract.
        /// </summary>
        public static readonly DependencyProperty HandleLoadedProperty =
            DependencyProperty.RegisterAttached("HandleLoaded", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, OnHandleLoadedChanged));

        private static void OnHandleLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (((bool?)e.NewValue) == true)
                {
                    FrameworkElement view = d as FrameworkElement;
                    if (view != null)
                    {
                        view.Loaded += OnViewLoaded;
                    }
                }
            }
        }

        private static void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement view = sender as FrameworkElement;
            object viewModel = view.DataContext;

            view.Loaded -= OnViewLoaded;

            if (viewModel is ILoadedEventListener)
            {
                ((ILoadedEventListener)viewModel).OnLoaded();
            }
        }
    }

}
