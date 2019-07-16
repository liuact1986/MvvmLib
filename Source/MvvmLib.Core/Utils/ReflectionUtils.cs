using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace MvvmLib.Utils
{

    /// <summary>
    /// The Reflection utils class.
    /// </summary>
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

        /// <summary>
        /// Gets the first constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructor</returns>
        public static ConstructorInfo GetDefaultConstructor(Type type, bool nonPublic = true)
        {
            var constructors = GetConstructors(type, nonPublic);
            return constructors.FirstOrDefault(c => c.Name != ".cctor");
        }

        /// <summary>
        /// Gets the binding flags.
        /// </summary>
        /// <param name="nonPublic">Non public</param>
        /// <returns>the flags</returns>
        public static BindingFlags GetFlags(bool nonPublic = true)
        {
            var flags = nonPublic ?
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
               : BindingFlags.Instance | BindingFlags.Public;
            return flags;
        }

        /// <summary>
        /// Gets the constructors for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="flags">The flags</param>
        /// <returns>The constructors</returns>
        public static ConstructorInfo[] GetConstructors(Type type, BindingFlags flags)
        {
            return type.GetConstructors(flags);
        }

        /// <summary>
        /// Gets the constructors for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructors</returns>
        public static ConstructorInfo[] GetConstructors(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetConstructors(flags);
        }

        /// <summary>
        /// Gets the parameterized constructor for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructor</returns>
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

        /// <summary>
        /// Gets the parameterized constructor for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructor</returns>
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

        /// <summary>
        /// Gets the parameterized constructor for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="parameterTypes">The parameter types</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructor</returns>
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

        /// <summary>
        /// Gets the constructor for the type and the attribute.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="attributeType">The attribute</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The constructor</returns>
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

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The instance</returns>
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

        /// <summary>
        /// Gets the properties for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The properties</returns>
        public static PropertyInfo[] GetProperties(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetProperties(flags);
        }

        /// <summary>
        /// Gets the fields for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublic">Non public</param>
        /// <returns>The fields</returns>
        public static FieldInfo[] GetFields(Type type, bool nonPublic = true)
        {
            var flags = GetFlags(nonPublic);
            return type.GetFields(flags);
        }

        /// <summary>
        /// Checks if the type is enumerable type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if enumerable type</returns>
        public static bool IsEnumerableType(Type type)
        {
#if NETSTANDARD2_0
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type);
#else
            return typeof(IEnumerable).IsAssignableFrom(type);
#endif
        }

        /// <summary>
        /// Checks if the type is dictionary type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if dictionary type</returns>
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

        /// <summary>
        /// Checks if the type is generic dictionary type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if dictionary type</returns>
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

        /// <summary>
        /// Gets the generic arguments for the dictionary.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The arguments</returns>
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

        /// <summary>
        /// Checks if the type is generic list type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if list type</returns>
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

        /// <summary>
        /// Gets the generic arguments for the list.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The arguments</returns>
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

        /// <summary>
        /// Checks if the type is list type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if list type</returns>
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

        /// <summary>
        /// Checks if the type is command type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if command type</returns>
        public static bool IsCommandType(Type type)
        {
            var isCommandType = typeof(ICommand).IsAssignableFrom(type);
            return isCommandType;
        }

        /// <summary>
        /// Checks if the type is delegate type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if delegate type</returns>
        public static bool IsDelegateType(Type type)
        {
            var isDelegateType = typeof(Delegate).IsAssignableFrom(type);
            return isDelegateType;
        }

        /// <summary>
        /// Checks if the type is action type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if action type</returns>
        public static bool IsActionType(Type type)
        {
            var isActionType = type == typeof(Action) || type.IsGenericType && actionTypes.Contains(type.GetGenericTypeDefinition());
            return isActionType;
        }

        /// <summary>
        /// Checks if the type is func type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if func type</returns>
        public static bool IsFuncType(Type type)
        {
            var isFuncType = type.IsGenericType && funcTypes.Contains(type.GetGenericTypeDefinition());
            return isFuncType;
        }
    }
}
