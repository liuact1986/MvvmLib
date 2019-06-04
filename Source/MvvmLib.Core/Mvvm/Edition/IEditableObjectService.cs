using System.Collections.Generic;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to clone and restore objects.
    /// </summary>
    public interface IEditableObjectService
    {
        /// <summary>
        /// Clones and stores the cloned value.
        /// </summary>
        /// <param name="source">The value to store</param>
         void Store(object source);

        /// <summary>
        /// Restore the target with the cloned source.
        /// </summary>
        /// <param name="target">The target to restore</param>
        void Restore(object target);

        /// <summary>
        /// Restore the target with the cloned source.
        /// </summary>
        /// <param name="target">The target to restore</param>
        /// <param name="propertiesToIgnore">The properties to ignore</param>
        void Restore(object target, IList<string> propertiesToIgnore);

        /// <summary>
        /// Sets the clone to null value.
        /// </summary>
        void Reset();
    }

}
