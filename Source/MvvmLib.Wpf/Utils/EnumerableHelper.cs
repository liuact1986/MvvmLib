using System.Collections;

namespace MvvmLib.Utils
{
    /// <summary>
    /// Enumerable utils.
    /// </summary>
    public class EnumerableHelper
    {
        /// <summary>
        /// Allows to find index of target item in collection.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="targetItem">The target item</param>
        /// <returns>The index or -1</returns>
        public static int FindIndex(IEnumerable source, object targetItem)
        {
            int i = 0;
            foreach (var item in source)
            {
                if (Equals(item, targetItem))
                    return i;

                i++;
            }
            return -1;
        }

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
