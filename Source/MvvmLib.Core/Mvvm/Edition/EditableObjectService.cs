using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to clone and restore objects.
    /// </summary>
    public class EditableObjectService : IEditableObjectService
    {
        private object clonedSource;
        private readonly Cloner cloner;

        /// <summary>
        /// Creates the editable object service.
        /// </summary>
        public EditableObjectService()
        {
            this.cloner = new Cloner();
        }

        /// <summary>
        /// Clones and stores the cloned value.
        /// </summary>
        /// <param name="source">The value to store</param>
        public void Store(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.clonedSource = cloner.DeepClone(source);
        }

        private void DoRestore(object target, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var propertyValue = property.GetValue(this.clonedSource);
                    property.SetValue(target, propertyValue);
                }
            }
        }

        /// <summary>
        /// Restore the target with the cloned source.
        /// </summary>
        /// <param name="target">The target to restore</param>
        public void Restore(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (clonedSource == null)
                throw new ArgumentException("Clone is null. Unable to restore after clear invoked. Invoke Store again");
            if (!clonedSource.GetType().IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException("Source is not assignable from target");

            var properties = clonedSource.GetType().GetProperties();
            DoRestore(target, properties);
        }

        /// <summary>
        /// Restore the target with the cloned source.
        /// </summary>
        /// <param name="target">The target to restore</param>
        /// <param name="propertiesToIgnore">The properties to ignore</param>
        public void Restore(object target, IList<string> propertiesToIgnore)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (clonedSource == null)
                throw new ArgumentException("Clone is null. Unable to restore after clear invoked. Invoke Store again");
            if (!clonedSource.GetType().IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException("Source is not assignable from target");

            var properties = clonedSource.GetType().GetProperties().Where(p => !propertiesToIgnore.Contains(p.Name));
            DoRestore(target, properties);
        }

        /// <summary>
        /// Sets the clone to null value.
        /// </summary>
        public void Reset()
        {
            this.clonedSource = null;
        }

    }

}
