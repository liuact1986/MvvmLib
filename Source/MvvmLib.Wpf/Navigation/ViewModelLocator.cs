using MvvmLib.Logger;
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
        private static readonly ILogger Logger;

        static ViewModelLocator()
        {
            Logger = new DebugLogger();
        }

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

                    var view = d as FrameworkElement;
                    view.Loaded += OnViewLoaded;
                }
            }
        }

        private static void OnViewLoaded(object sender, EventArgs e)
        {
            var view = sender as FrameworkElement;
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(view.GetType());
            if (viewModelType != null)
            {
                // Initialized => sub child => child => parent
                // Loaded => parent => child => sub child
                var context = ViewModelLocationProvider.CreateViewModelInstance(viewModelType);
                if (context == null)
                    Logger.Log($"No ViewModel found for \"{viewModelType.Name}\" with ResolveViewModel dependency attached property on \"{view.GetType()}\"", Category.Warn, Priority.High);
                else
                {
                    view.DataContext = context;

                    if (context is IIsLoaded)
                        ((IIsLoaded)context).OnLoaded();
                }
            }
            else
                Logger.Log($"No ViewModel Type found with ResolveViewModel dependency attached property on \"{view.GetType()}\"", Category.Warn, Priority.High);

            view.Loaded -= OnViewLoaded;
        }
    }

}
