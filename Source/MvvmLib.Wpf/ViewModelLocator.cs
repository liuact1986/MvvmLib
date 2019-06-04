using System;
using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to resolve the ViewModel with the <see cref="ViewModelLocationProvider"/> for a view. 
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Gets the Resolve View Model value for the dependency object.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>True if ResoleViewModel is requested</returns>
        public static bool GetResolveViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(ResolveViewModelProperty);
        }

        /// <summary>
        /// Sets the Resolve View Model value.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <param name="value">The bool value</param>
        public static void SetResolveViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(ResolveViewModelProperty, value);
        }

        /// <summary>
        /// Allows to resolve ViewModel for a view.
        /// </summary>
        public static readonly DependencyProperty ResolveViewModelProperty =
            DependencyProperty.RegisterAttached("ResolveViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, OnResolveViewModelChanged));

        private static void OnResolveViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                if ((bool)e.NewValue)
                {
                    if (!(d is FrameworkElement))
                        throw new InvalidOperationException($"The ResolveViewModel attached property only support a view. Type \"{d.GetType().Name}\"");

                    var frameworkElement = d as FrameworkElement;
                    frameworkElement.Initialized += OnFrameworkElementInitialized;
                }
            }
        }

        private static void OnFrameworkElementInitialized(object sender, EventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;

            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(frameworkElement.GetType());
            if (viewModelType != null)
            {
                // problem with attached property SourceName defined after ResolveViewModel
                // => tries to create view model instance and inject dependencies
                // => but not find ContentControlNavigationSource
                var context = ViewModelLocationProvider.CreateViewModelInstance(viewModelType);
                frameworkElement.DataContext = context;
            }

            frameworkElement.Initialized += OnFrameworkElementInitialized;
        }
    }

}
