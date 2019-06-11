using System;
using System.Collections;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// A read only collection implementation.
    /// </summary>
    public class ReadOnlyCollection : ICollection
    {
        /// <summary>
        /// The inner list.
        /// </summary>
        protected IList list;

        /// <summary>
        /// Gets the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item found</returns>
        public object this[int index]
        {
            get
            {
                if (index < 0 || index > list.Count - 1)
                    throw new IndexOutOfRangeException();

                return list[index];
            }
        }

        /// <summary>
        /// Creates the read only collection.
        /// </summary>
        /// <param name="list">The inner list</param>
        public ReadOnlyCollection(IList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            this.list = list;
        }

        /// <summary>
        /// Gets the count of items.
        /// </summary>
        public virtual int Count
        {
            get { return list.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return list.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return list.SyncRoot; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            list.CopyTo(array, index);
        }

        /// <summary>
        /// Gets an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public virtual IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
