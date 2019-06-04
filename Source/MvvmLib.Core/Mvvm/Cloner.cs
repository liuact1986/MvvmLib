using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to clone values or objects.
    /// </summary>
    public class Cloner
    {
        private readonly CircularReferenceManager circularReferenceManager;
        private readonly MethodInfo cloneGenericDictionaryMethod;
        private readonly MethodInfo cloneGenericListOrCollectionMethod;

        private bool nonPublicConstructors;
        /// <summary>
        /// Allows to include non public constructors.
        /// </summary>
        public bool NonPublicConstructors
        {
            get { return nonPublicConstructors; }
            set { nonPublicConstructors = value; }
        }

        private bool nonPublicProperties;
        /// <summary>
        /// Allows to include non public properties.
        /// </summary>
        public bool NonPublicProperties
        {
            get { return nonPublicProperties; }
            set { nonPublicProperties = value; }
        }

        private bool nonPublicFields;
        /// <summary>
        /// Allows to include non public fields.
        /// </summary>
        public bool NonPublicFields
        {
            get { return nonPublicFields; }
            set { nonPublicFields = value; }
        }

        private bool includeFields;
        /// <summary>
        /// Allows to include fields.
        /// </summary>
        public bool IncludeFields
        {
            get { return includeFields; }
            set { includeFields = value; }
        }

        /// <summary>
        /// Creates the cloner.
        /// </summary>
        public Cloner()
        {
            NonPublicConstructors = true;
            NonPublicProperties = true;
            NonPublicFields = true;
            IncludeFields = false;

            circularReferenceManager = new CircularReferenceManager();

            this.cloneGenericDictionaryMethod = typeof(Cloner).GetMethod("CloneGenericDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
            this.cloneGenericListOrCollectionMethod = typeof(Cloner).GetMethod("CloneGenericListOrCollection", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private object CreateInstance(Type type)
        {
            var instance = ReflectionUtils.CreateInstance(type, nonPublicConstructors);
            return instance;
        }

        private object CreateInstanceFast(Type type)
        {
            var instance = Activator.CreateInstance(type, nonPublicConstructors);
            return instance;
        }

        private IDictionary CloneDictionary(IDictionary items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IDictionary;

            foreach (DictionaryEntry entry in items)
            {
                var clonedKey = DoDeepClone(entry.Key);
                var clonedValue = DoDeepClone(entry.Value);

                result.Add(clonedKey, clonedValue);
            }
            return result;
        }

        private IDictionary<K, V> CloneGenericDictionary<K, V>(IDictionary<K, V> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IDictionary<K, V>;

            foreach (var entry in items)
            {
                var clonedKey = DoDeepClone(entry.Key);
                var clonedValue = DoDeepClone(entry.Value);

                result.Add((K)clonedKey, (V)clonedValue);
            }
            return result;
        }

        private IList CloneListOrCollection(IList items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IList;

            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.Add(clonedValue);
            }

            return result;
        }

        private IList<T> CloneGenericListOrCollection<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IList<T>;

            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.Add((T)clonedValue);
            }

            return result;
        }

        private Array CloneArray(Array items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var elementType = type.GetElementType();
            var result = Array.CreateInstance(elementType, items.Length);

            int index = 0;
            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.SetValue(clonedValue, index);
                index++;
            }

            return result;
        }

        private object CloneObject(object instance)
        {
            if (instance == null)
                return null;

            var type = instance.GetType();
            // circular
            var clonedInstanceRegistered = circularReferenceManager.TryGetInstance(instance);
            if (clonedInstanceRegistered != null)
                return clonedInstanceRegistered;

            var clonedInstance = CreateInstance(type);
            // circular
            circularReferenceManager.AddInstance(instance, clonedInstance);

            // properties
            var properties = ReflectionUtils.GetProperties(type, NonPublicProperties);
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var propertyValue = property.GetValue(instance);
                    if (propertyValue != null)
                    {
                        var clonedPropertyValue = DoDeepClone(propertyValue);
                        property.SetValue(clonedInstance, clonedPropertyValue);
                    }
                }
            }

            // fields
            if (includeFields)
            {
                var fields = ReflectionUtils.GetFields(type, NonPublicFields);
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(instance);
                    var clonedFieldValue = DoDeepClone(fieldValue);

                    field.SetValue(clonedInstance, clonedFieldValue);
                }
            }

            return clonedInstance;
        }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        private void ClearCircularReferences()
        {
            circularReferenceManager.Clear();
        }

        private object DoDeepClone(object value)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            if (IsValueTypeExtended(type))
                return value;
            else if (ReflectionUtils.IsEnumerableType(type))
            {
                if (ReflectionUtils.IsGenericDictionaryType(type))
                {
                    // IDictionary<K,V>
                    var args = ReflectionUtils.GetGenericDictionaryArguments(type);
                    var genericMethod = cloneGenericDictionaryMethod.MakeGenericMethod(args[0], args[1]);
                    return genericMethod.Invoke(this, new object[] { value });
                }
                else if (ReflectionUtils.IsDictionaryType(type))
                {
                    // IDictionary
                    return CloneDictionary((IDictionary)value);
                }
                else if (type.IsArray)
                {
                    return CloneArray((Array)value);
                }
                else if (ReflectionUtils.IsGenericListOrCollectionType(type))
                {
                    // IList<T> or ICollection<T>
                    var args = ReflectionUtils.GetGenericListOrCollectionArguments(type);
                    var genericMethod = cloneGenericListOrCollectionMethod.MakeGenericMethod(args[0]);
                    return genericMethod.Invoke(this, new object[] { value });
                }
                else if (ReflectionUtils.IsListOrCollectionType(type))
                {
                    // IList or ICollection
                    return CloneListOrCollection((IList)value);
                }
            }
            else
            {
                return CloneObject(value);
            }
            throw new NotSupportedException($"Unable to clone type {value.GetType().Name}");
        }

        /// <summary>
        /// Clones the value.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        /// <returns>The clone</returns>
        public T DeepClone<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            ClearCircularReferences();

            return (T)DoDeepClone(value);
        }
    }

}
