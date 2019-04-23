using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Updates the values only if required.
    /// </summary>
    public static class SyncListExtension
    {

        /// <summary>
        /// Synchronize the current to other value(s).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldValues">The old values</param>
        /// <param name="newValues">The new values</param>
        public static void Sync<T>(this IList<T> oldValues, IList<T> newValues) where T : ISyncItem<T>
        {
            if (oldValues == null)
                throw new System.ArgumentNullException(nameof(oldValues));
            if (newValues == null)
                throw new System.ArgumentNullException(nameof(newValues));

            if (oldValues != null && newValues != null)
            {
                // updated items
                var oldCommonValues = oldValues.Intersect(newValues).ToList();
                foreach (var oldItem in oldCommonValues)
                {
                    var newItem = newValues.First(ci => ci.Equals(oldItem));
                    if (oldItem.NeedSync(newItem))
                        // update old item
                        oldItem.Sync(newItem);
                }

                // deleted items
                var itemsToRemove = oldValues.Except(newValues).ToList();
                foreach (var item in itemsToRemove)
                    oldValues.Remove(item);

                // added items
                var itemsToAdd = newValues.Except(oldValues).ToList();
                foreach (var item in itemsToAdd)
                {
                    var index = newValues.IndexOf(item);
                    if (IsOutOfRange(oldValues, index))
                        oldValues.Add(item);
                    else
                        oldValues.Insert(index, item);
                }
            }
        }

        private static bool IsOutOfRange<T>(IList<T> list, int index) where T : ISyncItem<T>
        {
            return index > list.Count;
        }
    }
}
