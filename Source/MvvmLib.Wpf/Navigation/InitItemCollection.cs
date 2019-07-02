using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Collection that allows to init <see cref="SharedSource{T}"/> with items and parameters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InitItemCollection<T> : Collection<KeyValuePair<T, object>>
    {
        /// <summary>
        /// Adds an item with the parameter.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter</param>
        public void Add(T item, object parameter)
        {
            this.Items.Add(new KeyValuePair<T, object>(item, parameter));
        }
    }
}
