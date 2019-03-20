using System.Collections.Generic;

namespace MvvmLib.Mvvm
{
    public class CircularReferenceManager
    {
        private readonly Dictionary<object, object> sourceByClonedInstance;

        public CircularReferenceManager()
        {
            sourceByClonedInstance = new Dictionary<object, object>();
        }

        public bool IsInstanceRegistered(object source)
        {
            return sourceByClonedInstance.ContainsKey(source);
        }

        public void AddInstance(object source, object instance)
        {
            if (!IsInstanceRegistered(source))
            {
                sourceByClonedInstance[source] = instance;
            }
        }

        public object TryGetInstance(object source)
        {
            if (IsInstanceRegistered(source))
            {
                return sourceByClonedInstance[source];
            }

            return null;
        }

        public void Clear()
        {
            sourceByClonedInstance.Clear();
        }
    }

}
