using System.Collections;

namespace MvvmLib.Utils
{
    /// <summary>
    /// Enumerable utils.
    /// </summary>
    public class EnumerableHelper
    {
        /// <summary>
        /// Gets the element at the index.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="index">The index</param>
        /// <returns>The element or null</returns>
        public static object GetElementAt(IEnumerable source, int index)
        {
            if (source is IList)
            {
                return ((IList)source)[index];
            }

            int i = 0;
            foreach (var item in source)
            {
                if (i == index)
                    return item;

                i++;
            }

            return null;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>The number of items</returns>
        public static int Count(IEnumerable source)
        {
            if (source is IList)
            {
                return ((IList)source).Count;
            }

            int count = 0;
            foreach (var item in source)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="item">The item</param>
        /// <returns>The index</returns>
        public static int IndexOf(IEnumerable source, object item)
        {
            if (source is IList)
            {
                return ((IList)source).IndexOf(item);
            }

            int index = 0;
            foreach (var current in source)
            {
                if (Equals(item, current))
                    return index;

                index++;
            }

            return -1;
        }
    }
}
