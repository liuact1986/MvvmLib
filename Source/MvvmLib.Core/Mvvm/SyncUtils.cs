using System;
using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Allows to synchronize lists and collections.
    /// </summary>
    public class SyncUtils
    {
        /// <summary>
        /// Synchronize old items with new items.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="oldItems">The old items</param>
        /// <param name="newItems">The new items</param>
        public static void Sync<T>(IList<T> oldItems, IList<T> newItems) where T : ISyncItem<T>
        {
            if (oldItems == null)
                throw new ArgumentNullException(nameof(oldItems));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            DoUpdateItems(oldItems, newItems);
            DoRemoveDeletedItems(oldItems, newItems);
            DoInsertAddedItems(oldItems, newItems);
        }

        private static void DoInsertAddedItems<T>(IList<T> oldItems, IList<T> newItems) // where T : ISyncItem<T>
        {
            var itemsToAdd = newItems.Except(oldItems).ToList();
            foreach (var item in itemsToAdd)
            {
                var index = newItems.IndexOf(item);
                // insert new item
                if (index > oldItems.Count)
                    oldItems.Add(item);
                else
                    oldItems.Insert(index, item);
            }
        }

        private static void DoRemoveDeletedItems<T>(IList<T> oldItems, IList<T> newItems) // where T : ISyncItem<T>
        {
            var itemsToRemove = oldItems.Except(newItems).ToList();
            foreach (var item in itemsToRemove)
                // remove item from old items
                oldItems.Remove(item);
        }

        private static void DoUpdateItems<T>(IList<T> oldItems, IList<T> newItems) where T : ISyncItem<T>
        {
            var oldCommonItems = oldItems.Intersect(newItems).ToList();
            foreach (var oldItem in oldCommonItems)
            {
                var newItem = newItems.First(ci => ci.Equals(oldItem));
                if (oldItem.NeedSync(newItem))
                    // update old item
                    oldItem.Sync(newItem);
            }
        }

        /// <summary>
        /// Inserts added items to old items.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="oldItems">The old items</param>
        /// <param name="newItems">The new items</param>
        public static void InsertAddedItems<T>(IList<T> oldItems, IList<T> newItems) // where T : ISyncItem<T>
        {
            if (oldItems == null)
                throw new ArgumentNullException(nameof(oldItems));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            DoInsertAddedItems(oldItems, newItems);
        }

        /// <summary>
        /// Removes deleted items from old items.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="oldItems">The old items</param>
        /// <param name="newItems">The new items</param>
        public static void RemoveDeletedItems<T>(IList<T> oldItems, IList<T> newItems) // where T : ISyncItem<T>
        {
            if (oldItems == null)
                throw new ArgumentNullException(nameof(oldItems));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            DoRemoveDeletedItems(oldItems, newItems);
        }

        /// <summary>
        /// Updates old items.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="oldItems">The old items</param>
        /// <param name="newItems">The new items</param>
        public static void UpdateItems<T>(IList<T> oldItems, IList<T> newItems) where T : ISyncItem<T>
        {
            if (oldItems == null)
                throw new ArgumentNullException(nameof(oldItems));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            DoUpdateItems(oldItems, newItems);
        }
    }
}
