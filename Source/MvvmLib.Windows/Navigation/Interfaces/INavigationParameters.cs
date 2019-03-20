using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// A dictionary that allows to manage easily navigation parameters.
    /// </summary>
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Adds a navigation parameter.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks if the key is already in use.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>true if in use</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// The dictionary elements count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The keys.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the value of the navigation parameter.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Returns the values.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The enumerable</returns>
        IEnumerable<T> GetValues<T>(string key);

        /// <summary>
        /// Tries to get the parameter value for the key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <param name="value"></param>
        /// <returns>True if found</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Returns the element with the key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The element</returns>
        object this[string key] { get; }

        /// <summary>
        /// remove the navigation parameter.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>true if removed</returns>
        bool Remove(string key);
    }

}
