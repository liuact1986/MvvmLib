using System;
using System.Reflection;

namespace MvvmLib.Utils
{ 
    /// <summary>
    /// Allows to resolve a property from a property path.
    /// </summary>
    public class PropertyPathHelper
    {
        /// <summary>
        /// Gets the property for the property path.
        /// </summary>
        /// <param name="ownerType">The type</param>
        /// <param name="propertyPath">The property path</param>
        /// <returns>The property</returns>
        public static PropertyInfo GetProperty(Type ownerType, string propertyPath)
        {
            var propertyNames = propertyPath.Split('.');
            if (propertyNames.Length == 1)
            {
                var property = ownerType.GetProperty(propertyPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property == null)
                    throw new ArgumentException($"Failed to find the property for the property path '{propertyPath}' and type '{ownerType.Name}'");
                return property;
            }
            else
            {
                Type currentType = ownerType;
                PropertyInfo property = null;
                foreach (string propertyName in propertyNames)
                {
                    property = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (property == null)
                        throw new ArgumentException($"Failed to find the property for the property path '{propertyPath}' and type '{ownerType.Name}'");
                    currentType = property.PropertyType;
                }
                return property;
            }
        }

        /// <summary>
        /// Gets the owner value.
        /// </summary>
        /// <param name="parent">The parent object</param>
        /// <param name="propertyPath">The property path</param>
        /// <returns>The owner resolved or null</returns>
        public static object GetOwner(object parent, string propertyPath)
        {
            var propertyNames = propertyPath.Split('.');
            if (propertyNames.Length == 1)
            {
                return parent;
            }
            else
            {
                int ownerIndex = propertyNames.Length - 1;
                Type currentType = parent.GetType();
                object current = parent;
                PropertyInfo property = null;
                for (int i = 0; i < ownerIndex; i++)
                {
                    var propertyName = propertyNames[i];
                    property = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (property == null)
                        throw new ArgumentException($"Failed to find the property for the property path '{propertyPath}' and type '{parent.GetType().Name}'");

                    var nextOwner = property.GetValue(current);
                    if (nextOwner == null)
                    {
                        // unable to set property
                        return null;
                    }
                    currentType = property.PropertyType;
                    current = nextOwner;
                }

                return current;
            }
        }
    }
}
