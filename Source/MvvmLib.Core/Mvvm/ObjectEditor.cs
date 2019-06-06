using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to clone and restore objects.
    /// </summary>
    public class ObjectEditor
    {
        private readonly Cloner cloner;
        private object originalSource;
        private object trackedValue;
        private readonly IEnumerable<PropertyInfo> properties;


        /// <summary>
        /// Creates the editable object service.
        /// </summary>
        /// <param name="type">The type of original source</param>
        /// <param name="propertiesToIgnore">The properties to ignore</param>
        public ObjectEditor(Type type, List<string> propertiesToIgnore)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (propertiesToIgnore == null)
                throw new ArgumentNullException(nameof(propertiesToIgnore));

            this.cloner = new Cloner();

            var properties = type.GetProperties()
                .Where(p => p.CanRead && p.CanWrite && !propertiesToIgnore.Contains(p.Name));
            this.properties = properties;
        }

        /// <summary>
        /// Creates the editable object service.
        /// </summary>
        /// <param name="type">The type of original source</param>
        public ObjectEditor(Type type)
            : this(type, new List<string>())
        { }


        /// <summary>
        /// Clones and stores the cloned value.
        /// </summary>
        /// <param name="originalSource">The value to store</param>
        public void Store(object originalSource)
        {
            if (originalSource == null)
                throw new ArgumentNullException(nameof(originalSource));

            this.trackedValue = originalSource;
            this.originalSource = this.cloner.DeepClone(originalSource);
        }

        /// <summary>
        /// Restore the target with the cloned source.
        /// </summary>
        public void Restore()
        {
            if (this.originalSource == null)
                throw new InvalidOperationException("No original source provided. Call \"Store\" method to set the orinal source to track.");

            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(this.originalSource);
                    property.SetValue(this.trackedValue, value);
                }
            }
        }

        /// <summary>
        /// Sets the clone to null value.
        /// </summary>
        public void Clean()
        {
            this.originalSource = null;
            this.trackedValue = null;
        }
    }

}
