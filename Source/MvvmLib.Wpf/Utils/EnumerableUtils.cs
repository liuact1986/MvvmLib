using System.Collections;

namespace MvvmLib.Utils
{
    /// <summary>
    /// Enumerable utils.
    /// </summary>
    public class EnumerableUtils
    {
        /// <summary>
        /// Allows to find index of target item in collection.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="targetItem">The target item</param>
        /// <returns>The index or -1</returns>
        public static int FindIndex(IEnumerable items, object targetItem)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (Equals(item, targetItem))
                    return i;

                i++;
            }
            return -1;
        }
    }
}
