using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.IoC
{
    internal class ReflectionUtils
    {
        public static ConstructorInfo GetConstructorWithAttribute(IEnumerable<ConstructorInfo> constructors, Type attributeType)
        {
            foreach (var constructor in constructors)
            {
                var attribute = constructor.GetCustomAttribute(attributeType);
                if (attribute != null)
                    return constructor;
            }
            return null;
        }

        private static BindingFlags GetFlags(bool nonPublic)
        {
            var flags = nonPublic ?
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
               : BindingFlags.Instance | BindingFlags.Public;
            return flags;
        }

        public static ConstructorInfo[] GetConstructors(Type type, bool nonPublic)
        {
            var flags = GetFlags(nonPublic);
            var constructors = type.GetConstructors(flags);
            return constructors;
        }

        public static ConstructorInfo GetEmptyConstructor(Type type, bool nonPublic)
        {
            var flags = GetFlags(nonPublic);
            var constructor = type.GetConstructor(flags, null, Type.EmptyTypes, null);
            return constructor;
        }

        public static PropertyInfo[] GetProperties(Type type, bool nonPublic)
        {
            var flags = GetFlags(nonPublic);
            return type.GetProperties(flags);
        }
    }
}
