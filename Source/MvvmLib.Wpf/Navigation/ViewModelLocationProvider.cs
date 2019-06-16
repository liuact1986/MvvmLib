using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The ViewModelLocationProvider allows to resolve the view model type for a view and crete the view model instance.
    /// </summary>
    public class ViewModelLocationProvider
    {
        private readonly static Dictionary<Type, Type> customRegistrations;
        private readonly static Dictionary<Type, Type> cache;

        /// <summary>
        /// The default convention.
        /// </summary>
        private static readonly Func<Type, Type> defaultViewTypeToViewModelTypeResolver;

        /// <summary>
        /// Custom convention.
        /// </summary>
        private static Func<Type, Type> viewTypeToViewModelTypeResolver;

        /// <summary>
        /// The default view model factory. Can be override with <see cref="SetViewModelFactory(Func{Type, object})"/>.
        /// </summary>
        private static Func<Type, object> viewModelFactory;

        static ViewModelLocationProvider()
        {
            customRegistrations = new Dictionary<Type, Type>();
            cache = new Dictionary<Type, Type>();
            SetViewModelFactoryToDefault();
            defaultViewTypeToViewModelTypeResolver = new Func<Type, Type>(viewType =>
            {

                var viewFullName = viewType.FullName;
                viewFullName = viewFullName.Replace(".Views.", ".ViewModels.");
                var suffix = viewFullName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelFullName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", viewFullName, suffix);
                var viewModelType = viewType.Assembly.GetType(viewModelFullName);

                return viewModelType;
            });
        }

        /// <summary>
        /// Allows to change the convention used to resolve ViewModels for the views.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The new convention</param>
        public static void ChangeConvention(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            if (viewTypeToViewModelTypeResolver == null)
                throw new ArgumentNullException(nameof(viewTypeToViewModelTypeResolver));

            ViewModelLocationProvider.viewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Registers a ViewModelType for a ViewType.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <param name="viewModelType">The view model type</param>
        public static void RegisterCustom(Type viewType, Type viewModelType)
        {
            if (viewType == null)
                throw new ArgumentNullException(nameof(viewType));
            if (viewModelType == null)
                throw new ArgumentNullException(nameof(viewModelType));

            customRegistrations[viewType] = viewModelType;
        }

        /// <summary>
        /// Allows to change the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The new factory</param>
        public static void SetViewModelFactory(Func<Type, object> viewModelFactory)
        {
            if (viewModelFactory == null)
                throw new ArgumentNullException(nameof(viewModelFactory));

            ViewModelLocationProvider.viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Allows to resolve the view model type for a view type.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <returns>The view model type or null</returns>
        public static Type ResolveViewModelType(Type viewType)
        {
            if (cache.TryGetValue(viewType, out Type cachedViewModelType))
            {
                return cachedViewModelType;
            }
            else
            {
                Type viewModelType = null;
                if (customRegistrations.TryGetValue(viewType, out Type customRegistration))
                    viewModelType = customRegistration;
                else
                {
                    if (viewTypeToViewModelTypeResolver != null)
                        viewModelType = viewTypeToViewModelTypeResolver(viewType);
                    else
                        viewModelType = defaultViewTypeToViewModelTypeResolver(viewType);
                }

                if (viewModelType != null)
                    cache[viewType] = viewModelType;

                return viewModelType;
            }
        }

        /// <summary>
        /// Creates the view model instance with the view model factory.
        /// </summary>
        /// <param name="viewModelType">The view model type</param>
        /// <returns>The view model instance</returns>
        public static object CreateViewModelInstance(Type viewModelType)
        {
            var viewModel = viewModelFactory(viewModelType);
            return viewModel;
        }

        /// <summary>
        /// Resets to the default convention.
        /// </summary>
        public static void ResetConvention()
        {
            viewTypeToViewModelTypeResolver = null;
        }

        /// <summary>
        /// Resets the view model factory.
        /// </summary>
        public static void SetViewModelFactoryToDefault()
        {
            viewModelFactory = new Func<Type, object>(viewModelType => Activator.CreateInstance(viewModelType));
        }
    }
}
