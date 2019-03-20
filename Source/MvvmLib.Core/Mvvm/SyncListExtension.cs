using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Mvvm
{
    public static class SyncListExtension
    {
        public static void Sync<T>(this IList<T> oldCollection, IList<T> newItems) where T : ISyncItem<T>
        {
            if (oldCollection != null && newItems != null)
            {
                // updated items
                var oldCommonItems = oldCollection.Intersect(newItems).ToList();
                foreach (var oldItem in oldCommonItems)
                {
                    var newItem = newItems.First(ci => ci.Equals(oldItem));
                    if (oldItem.NeedSync(newItem))
                    {
                        // update old item
                        oldItem.Sync(newItem);
                    }
                }

                // deleted items
                var itemsToRemove = oldCollection.Except(newItems).ToList();
                foreach (var item in itemsToRemove)
                {
                    oldCollection.Remove(item);
                }

                // added items
                var itemsToAdd = newItems.Except(oldCollection).ToList();
                foreach (var item in itemsToAdd)
                {
                    var index = newItems.IndexOf(item);
                    if (IsOutOfRange(oldCollection, index))
                    {
                        oldCollection.Add(item);
                    }
                    else
                    {
                        oldCollection.Insert(index, item);
                    }
                }
            }
        }

        private static bool IsOutOfRange<T>(IList<T> list, int index) where T : ISyncItem<T>
        {
            return index > list.Count;
        }
    }
}
