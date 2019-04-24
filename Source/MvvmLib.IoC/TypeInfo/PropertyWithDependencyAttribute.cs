using System;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Property with <see cref="DependencyAttribute"/>.
    /// </summary>
    public class PropertyWithDependencyAttribute
    {
        private readonly PropertyInfo property;
        /// <summary>
        /// The property.
        /// </summary>
        public PropertyInfo Property
        {
            get { return property; }
        }

        private readonly string name;
        /// <summary>
        /// The name / key.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Creates the property with dependency class.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="name">The name / key</param>
        public PropertyWithDependencyAttribute(PropertyInfo property, string name)
        {
            this.property = property ?? throw new ArgumentNullException(nameof(property));
            this.name = name;
        }
    }
}
