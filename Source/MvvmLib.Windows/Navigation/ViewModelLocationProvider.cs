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
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);

                return Type.GetType(viewModelName);
            };

        static Dictionary<Type, Type> viewTypeToViewModelTypeCustomRegistrations = new Dictionary<Type, Type>();

        static Func<Type, object> viewModelFactory = (viewModelType) => Activator.CreateInstance(viewModelType);

        /// <summary>
        /// Changes the default convention.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The new convention to use</param>
        public static void SetViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            ViewModelLocationProvider.viewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// The view model factory receives a view model type and have to create an instance of the view model. It could uses a ioc container to create the instance and resolve view model dependences.
        /// </summary>
        /// <param name="viewModelFactory"></param>
        public static void SetViewModelFactory(Func<Type, object> viewModelFactory)
        {
            ViewModelLocationProvider.viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Gets the view model type for the view type.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <returns>The view model type</returns>
        public static Type ResolveViewModelType(Type viewType)
        {
            // custom Registration ? 
            if (viewTypeToViewModelTypeCustomRegistrations.ContainsKey(viewType))
            {
                return viewTypeToViewModelTypeCustomRegistrations[viewType];
            }

            var viewModelType = viewTypeToViewModelTypeResolver(viewType); ;
            return viewModelType;
        }


        /// <summary>
        /// Returns an instance of the view model type.
        /// </summary>
        /// <param name="viewModelType">The view model type</param>
        /// <returns>The instance</returns>
        public static object ResolveViewModel(Type viewModelType)
        {
            return viewModelFactory(viewModelType);
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
    }
}
