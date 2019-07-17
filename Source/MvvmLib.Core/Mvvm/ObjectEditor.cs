using MvvmLib.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to restore objects with original values.
    /// </summary>
    public class ObjectEditor
    {
        private object originalSource;
        private Dictionary<string, TrackedProperty> trackedProperties;
        private readonly Cloner cloner;
        private readonly List<string> propertiesToIgnore;

        /// <summary>
        /// Checks if a original source is provided and can restore.
        /// </summary>
        public bool CanRestore
        {
            get { return originalSource != null; }
        }

        /// <summary>
        /// Creates the <see cref="ObjectEditor"/>.
        /// </summary>
        /// <param name="propertiesToIgnore">The properties to ignore</param>
        public ObjectEditor(List<string> propertiesToIgnore)
        {
            this.cloner = new Cloner();
            this.propertiesToIgnore = propertiesToIgnore;
        }

        /// <summary>
        /// Creates the <see cref="ObjectEditor"/>.
        /// </summary>
        public ObjectEditor()
            : this(null)
        { }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        /// <summary>
        /// Stores the original values.
        /// </summary>
        /// <param name="originalSource">The original source</param>
        public void Store(object originalSource)
        {
            if (originalSource == null)
                throw new ArgumentNullException(nameof(originalSource));

            this.originalSource = originalSource;
            this.trackedProperties = new Dictionary<string, TrackedProperty>();

            var type = originalSource.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite && (propertiesToIgnore == null || !propertiesToIgnore.Contains(property.Name)))
                {
                    var propertyType = property.PropertyType;
                    if (IsValueTypeExtended(propertyType))
                    {
                        var value = property.GetValue(originalSource);
                        this.trackedProperties[property.Name] = new TrackedProperty(property, value);
                    }
                    else if (ReflectionUtils.IsEnumerableType(propertyType))
                    {
                        var value = property.GetValue(originalSource);
                        var clonedValue = value == null ? null : this.cloner.DeepClone(value);
                        this.trackedProperties[property.Name] = new TrackedProperty(property, clonedValue);
                    }
                    else
                    {
                        var value = property.GetValue(originalSource);
                        if (!(ReflectionUtils.IsCommandType(propertyType)))
                        {
                            var clonedValue = value == null ? null : this.cloner.DeepClone(value);
                            this.trackedProperties[property.Name] = new TrackedProperty(property, clonedValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores the <see cref="originalSource"/> with the original values.
        /// </summary>
        public void Restore()
        {
            if (this.originalSource == null)
                throw new InvalidOperationException("No original source provided. Call \"Store\" method to set the orinal source to track.");

            foreach (var trackedProperty in trackedProperties)
            {
                trackedProperty.Value.RestoreValue(this.originalSource);
            }
        }

        /// <summary>
        /// Sets the clone to null value.
        /// </summary>
        public void Clean()
        {
            this.originalSource = null;
            this.trackedProperties.Clear();
        }
    }

    /// <summary>
    /// A class with property and original value.
    /// </summary>
    public class TrackedProperty
    {
        private PropertyInfo property;
        /// <summary>
        /// The property.
        /// </summary>
        public PropertyInfo Property
        {
            get { return property; }
        }

        private object clonedValue;
        /// <summary>
        /// The cloned value.
        /// </summary>
        public object ClonedValue
        {
            get { return clonedValue; }
        }

        /// <summary>
        /// Creates the <see cref="TrackedProperty"/>.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="clonedValue">The cloned value</param>
        public TrackedProperty(PropertyInfo property, object clonedValue)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            this.property = property;
            this.clonedValue = clonedValue;
        }

        /// <summary>
        /// Restores the object with the cloned value.
        /// </summary>
        /// <param name="obj">The object</param>
        public void RestoreValue(object obj)
        {
            Property.SetValue(obj, ClonedValue);
        }
    }
}
