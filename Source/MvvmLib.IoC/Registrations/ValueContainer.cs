using System;
using System.Collections;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Dictionary for Value Type, Nullable, Array, enumerable or Uri.
    /// </summary>
    public class ValueContainer : IDictionary<string, object>
    {
        private readonly Dictionary<string, object> keyValues = new Dictionary<string, object>();

        /// <summary>
        /// Get or set the value. The value is checked.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return keyValues[key]; }
            set
            {
                CheckValue(value);
                keyValues[key] = value;
            }
        }

        /// <summary>
        /// Creates the value container.
        /// </summary>
        public ValueContainer()
        { }

        /// <summary>
        /// Creates the value container.
        /// </summary>
        /// <param name="keyValues">The values injected at creation</param>
        public ValueContainer(Dictionary<string, object> keyValues)
        {
            CheckValues(keyValues);

            this.keyValues = keyValues;
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return keyValues.Keys; }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ICollection<object> Values
        {
            get { return keyValues.Values; }
        }

        /// <summary>
        /// Gets the count of values.
        /// </summary>
        public int Count => keyValues.Count;

        /// <summary>
        /// Checks if the dictionary is read only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Checks if the type is Value Type, Nullable, Array, enumerable or Uri.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if is Value Type, Nullable, Array, enumerable or Uri</returns>
        public static bool IsValueContainerType(Type type)
        {
            return type == typeof(string)
                || type.IsValueType
                || type.IsArray
                || typeof(IEnumerable).IsAssignableFrom(type)
                || type == typeof(Uri)
                || Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Checks the value.
        /// </summary>
        /// <param name="value">The value</param>
        public static void CheckValue(object value)
        {
            if (value != null && !IsValueContainerType(value.GetType()))
                throw new ArgumentException("Invalid type for value container. Value Types, Nullables, Array, enumerables and Uri allowed");
        }

        /// <summary>
        /// Checks the type of each value and throw a <see cref="ArgumentException"/> if invalid.
        /// </summary>
        /// <param name="valueContainer"></param>
        public static void CheckValues(Dictionary<string, object> valueContainer)
        {
            foreach (var value in valueContainer.Values)
                CheckValue(value);
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckValue(value);

            this.keyValues.Add(key, value);
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="item">The key value pair</param>
        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clear the values.
        /// </summary>
        public void Clear()
        {
            this.keyValues.Clear();
        }

        /// <summary>
        /// Checks if the dictionary contains the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True the dictinary contains the item</returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.keyValues.ContainsKey(item.Key) && this.keyValues[item.Key] == item.Value;
        }

        /// <summary>
        /// Checks if the dictionary contains the key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if the dictionary contains the key</returns>
        public bool ContainsKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return this.keyValues.ContainsKey(key);
        }

        /// <summary>
        /// Copies the values to the array.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="arrayIndex">The index</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.keyValues.GetEnumerator();
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if removed</returns>
        public bool Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return keyValues.Remove(key);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if removed</returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Tries to get the value.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>True if found</returns>
        public bool TryGetValue(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return keyValues.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.keyValues.GetEnumerator();
        }
    }
}
