using System.Collections.Generic;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to manage circular references.
    /// </summary>
    public class CircularReferenceManager
    {
        private readonly Dictionary<object, object> sourceByClonedInstance;

        /// <summary>
        /// Creates the circular reference manager.
        /// </summary>
        public CircularReferenceManager()
        {
            sourceByClonedInstance = new Dictionary<object, object>();
        }

        /// <summary>
        /// Checks if the source is registered.
        /// </summary>
        /// <param name="source">The object source</param>
        /// <returns>True if registered</returns>
        public bool IsInstanceRegistered(object source)
        {
            return sourceByClonedInstance.ContainsKey(source);
        }

        /// <summary>
        /// Adds an instance.
        /// </summary>
        /// <param name="source">The object source</param>
        /// <param name="instance">The instance</param>
        public void AddInstance(object source, object instance)
        {
            if (!IsInstanceRegistered(source))
                sourceByClonedInstance[source] = instance;
        }

        /// <summary>
        /// Tries to get a stored instance.
        /// </summary>
        /// <param name="source">The object source</param>
        /// <returns>The instance found or null</returns>
        public object TryGetInstance(object source)
        {
            if (IsInstanceRegistered(source))
                return sourceByClonedInstance[source];

            return null;
        }

        /// <summary>
        /// Clears the registered instances.
        /// </summary>
        public void Clear()
        {
            sourceByClonedInstance.Clear();
        }
    }

}
