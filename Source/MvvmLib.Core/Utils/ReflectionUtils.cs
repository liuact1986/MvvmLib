using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmLib.Utils
{
    public class ReflectionUtils
    {

        private static Type[] actionTypes = new Type[]
        {
            typeof(Action<>), typeof(Action<,>),typeof(Action<,,>),typeof(Action<,,,>),typeof(Action<,,,,>),typeof(Action<,,,,,>),typeof(Action<,,,,,,>),typeof(Action<,,,,,,,>)
        };

        private static Type[] funcTypes = new Type[]
        {
            typeof(Func<>), typeof(Func<,>),typeof(Func<,,>),typeof(Func<,,,>),typeof(Func<,,,,>),typeof(Func<,,,,,>),typeof(Func<,,,,,,>),typeof(Func<,,,,,,,>),typeof(Func<,,,,,,,,>)
        };

        public static ConstructorInfo GetDefaultConstructor(Type type, bool nonPublic = true)
        {
            var constructors = GetConstructors(type, nonPublic);
            return constructors.FirstOrDefault(c => c.Name != ".cctor");
        }

        public static BindingFlags GetFlags(bool nonPublic = true)
        {
            var flags = nonPublic ?
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
               : BindingFlags.Instance | BindingFlags.Public;
            return flags;
        }

        public static ConstructorInfo[] GetConstructors(Type type, BindingFlags flags)
        {
            return type.GetConstructors(flags);
        }

        public static ConstructorInfo[] GetConstructors(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetConstructors(flags);
        }

        public static IEnumerable<ConstructorInfo> GetParameterizedConstructors(Type type, bool nonPublic = true)
        {
            var constructors = GetConstructors(type, nonPublic);
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0)
                {
                    yield return constructor;
                }
            }
        }

        private static bool IsExpectedConstructor(ConstructorInfo constructor, Type[] parameterTypes)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == parameterTypes.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameterTypes[i];
                    if (parameter.ParameterType != parameterType)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static ConstructorInfo GetParameterizedConstructor(Type type, bool nonPublic = true)
        {
            var constructors = GetConstructors(type, nonPublic);
            foreach (var constructor in constructors)
            {
                var ctorParameters = constructor.GetParameters();
                if (ctorParameters.Length > 0)
                {
                    return constructor;
                }
            }
            return null;
        }

        public static ConstructorInfo GetParameterizedConstructor(Type type, Type[] parameterTypes, bool nonPublic = true)
        {
            var constructors = GetParameterizedConstructors(type, nonPublic);
            foreach (var constructor in constructors)
            {
                if (IsExpectedConstructor(constructor, parameterTypes))
                {
                    return constructor;
                }
            }
            return null;
        }

        public static ConstructorInfo GetConstructor(Type type, Type attributeType, bool nonPublic = true)
        {
            var constructors = GetConstructors(type, nonPublic);
            foreach (var constructor in constructors)
            {
                var attribute = constructor.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return constructor;
                }
            }
            return null;
        }

        public static object CreateInstance(Type type, bool nonPublic = true)
        {
            var constructor = ReflectionUtils.GetDefaultConstructor(type, nonPublic);
            if (constructor != null)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0)
                {
                    var parameterValues = new object[parameters.Length];
                    return constructor.Invoke(parameterValues);
                }
                else
                {
                    return constructor.Invoke(null);
                }
            }
            return null;
        }

        public static PropertyInfo[] GetProperties(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetProperties(flags);
        }

        public static FieldInfo[] GetFields(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetFields(flags);
        }

        public static bool IsEnumerableType(Type type)
        {
#if NETSTANDARD2_0
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type);
#else
            return typeof(IEnumerable).IsAssignableFrom(type);
#endif
        }

        public static bool IsDictionaryType(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType == typeof(IDictionary))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsGenericDictionaryType(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return true;
                }
            }
            return false;
        }


        public static Type[] GetGenericDictionaryArguments(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return interfaceType.GetGenericArguments();
                }
            }
            return null;
        }

        public static bool IsGenericListOrCollectionType(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType &&
                    (interfaceType.GetGenericTypeDefinition() == typeof(IList<>) || interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    return true;
                }
            }
            return false;
        }

        public static Type[] GetGenericListOrCollectionArguments(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType &&
                    (interfaceType.GetGenericTypeDefinition() == typeof(IList<>) || interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    return interfaceType.GetGenericArguments();
                }
            }
            return null;
        }

        public static bool IsListOrCollectionType(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType == typeof(IList) || interfaceType == typeof(ICollection))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDelegateType(Type type)
        {
            var isDelegateType = typeof(Delegate).IsAssignableFrom(type);
            return isDelegateType;
        }

        public static bool IsActionType(Type type)
        {
            var isActionType = type == typeof(Action) || type.IsGenericType && actionTypes.Contains(type.GetGenericTypeDefinition());
            return isActionType;
        }

        public static bool IsFuncType(Type type)
        {
            var isFuncType = type.IsGenericType && funcTypes.Contains(type.GetGenericTypeDefinition());
            return isFuncType;
        }
    }
}
