using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// A dictionary that allows to manage easily navigation parameters.
    /// </summary>
    public class NavigationParameters : INavigationParameters
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        /// <summary>
        /// Creates the navigation parameter dictionary.
        /// </summary>
        /// <param name="parameters">Tuples of Name and value</param>
        public NavigationParameters(params (string Name, object Value)[] parameters)
        {
            foreach (var (Name, Value) in parameters)
            {
                this.parameters.Add(Name, Value);
            }
        }

        /// <summary>
        /// Returns the element with the key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The element</returns>
        public object this[string key] => parameters[key];

        /// <summary>
        /// The dictionary elements count.
        /// </summary>
        public int Count => parameters.Count;

        /// <summary>
        /// The keys.
        /// </summary>
        public IEnumerable<string> Keys => parameters.Keys;

        /// <summary>
        /// Adds a navigation parameter.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            parameters.Add(key, value);
        }

        /// <summary>
        /// Checks if the key is already in use.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>true if in use</returns>
        public bool ContainsKey(string key)
        {
            return parameters.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        /// <summary>
        /// Gets the value of the navigation parameter.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        public T GetValue<T>(string key)
        {
            return (T)Convert.ChangeType(parameters[key], typeof(T));
        }

        /// <summary>
        /// Returns a enumerator.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The enumerable</returns>
        public IEnumerable<T> GetValues<T>(string key)
        {
            return parameters.Where(x => x.Key == key).Select(x => (T)x.Value);
        }


        /// <summary>
        /// remove the navigation parameter.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>true if removed</returns>
        public bool Remove(string key)
        {
            return parameters.Remove(key);
        }

        /// <summary>
        /// A formetted string with keys and values.
        /// </summary>
        /// <returns>Navigation paremeters string</returns>
        public override string ToString()
        {
            return string.Join(",", parameters.Select(p => $"({p.Key}:{p.Value})"));
        }

        /// <summary>
        /// Tries to get the parameter value for the key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <param name="value"></param>
        /// <returns>True if found</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(parameters[key], typeof(T));
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }
    }

}
