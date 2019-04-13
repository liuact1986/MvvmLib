using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The ViewModelLocationProvider class locates the view model for the view.
    /// </summary>
    public class ViewModelLocationProvider
    {
        static Func<Type, Type> viewTypeToViewModelTypeResolver =
            viewType =>
            {
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

                return Type.GetType(viewModelName);
            };

        static Dictionary<Type, Type> viewTypeToViewModelTypeCache = new Dictionary<Type, Type>();

        static Dictionary<Type, Type> viewTypeToViewModelTypeCustomRegistrations = new Dictionary<Type, Type>();

        static Func<Type, object> viewModelFactory = (viewModelType) => Activator.CreateInstance(viewModelType);

        /// <summary>
        /// Allows to change the convention used to resolve ViewModels for the views.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The new convention</param>
        public static void SetViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            ViewModelLocationProvider.viewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Registers a ViewModelType for a ViewType.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <param name="viewModelType">The view model type</param>
        public static void RegisterCustom(Type viewType, Type viewModelType)
        {
            viewTypeToViewModelTypeCustomRegistrations[viewType] = viewModelType;
        }

        /// <summary>
        /// The view model factory receives a view model type and have to create an instance of the view model. It could uses a ioc container to create the instance and resolve view model dependences.
        /// </summary>
        /// <param name="viewModelFactory"></param>
        public static void SetViewModelFactory(Func<Type, object> viewModelFactory)
        {
            ViewModelLocationProvider.viewModelFactory = viewModelFactory;
        }

        internal static Type ResolveViewModelType(Type viewType)
        {
            if (viewTypeToViewModelTypeCache.ContainsKey(viewType))
            {
                return viewTypeToViewModelTypeCache[viewType];
            }
            else
            {
                // custom Registration ? 
                if (viewTypeToViewModelTypeCustomRegistrations.ContainsKey(viewType))
                {
                    return viewTypeToViewModelTypeCustomRegistrations[viewType];
                }

                var viewModelType = viewTypeToViewModelTypeResolver(viewType); 
                viewTypeToViewModelTypeCache.Add(viewType, viewModelType);
                return viewModelType;
            }
        }

        internal static object ResolveViewModel(Type viewModelType)
        {
            return viewModelFactory(viewModelType);
        }
    }
}
