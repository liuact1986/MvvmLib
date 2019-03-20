using System.Collections.Generic;

namespace MvvmLib.IoC
{
    public static class IInjectorResolverExtensions
    {
        public static T BuildUp<T>(this IInjectorResolver container, T instance) where T: class
        {
            return (T)container.BuildUp(instance);
        }

        public static T BuildUp<T>(this IInjectorResolver container, T instance, string name) where T : class
        {
            return (T)container.BuildUp(typeof(T), name);
        }

        public static T BuildUp<T>(this IInjectorResolver container) where T : class
        {
            return (T)container.BuildUp(typeof(T));
        }

        public static T GetInstance<T>(this IInjectorResolver container, string name)
        {
            return (T)container.GetInstance(typeof(T), name);
        }

        public static T GetInstance<T>(this IInjectorResolver container)
        {
            return (T)container.GetInstance(typeof(T));
        }

        public static T GetNewInstance<T>(this IInjectorResolver container, string name)
        {
            return (T)container.GetNewInstance(typeof(T), name);
        }

        public static T GetNewInstance<T>(this IInjectorResolver container)
        {
            return (T)container.GetNewInstance(typeof(T));
        }

        public static List<object> GetAllInstances<T>(this IInjectorResolver container)
        {
            return container.GetAllInstances(typeof(T));
        }
    }
}
