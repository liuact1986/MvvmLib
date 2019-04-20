using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region adapter base class.
    /// </summary>
    /// <typeparam name="T">The type of the control used by the region adapter</typeparam>
    public abstract class ItemsRegionAdapterBase<T> : IItemsRegionAdapter
    {
        /// <summary>
        /// The type of the control used by the region adapter.
        /// </summary>
        public Type TargetType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Invoked on insert new content.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="content">The content</param>
        /// <param name="index">The index</param>
        public abstract void OnInsert(T control, object content, int index);

        /// <summary>
        /// Invoked on remove element.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="index">The index</param>
        public abstract void OnRemoveAt(T control, int index);

        /// <summary>
        /// Invoked on clear all elements.
        /// </summary>
        /// <param name="control">The control</param>
        public abstract void OnClear(T control);

        /// <summary>
        /// Invoked on insert new content.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="content">The content</param>
        /// <param name="index">The index</param>
        public void OnInsert(object control, object content, int index)
        {
            this.OnInsert((T)control, content, index);
        }

        /// <summary>
        /// Invoked on remove element.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="index">The index</param>
        public void OnRemoveAt(object control, int index)
        {
            this.OnRemoveAt((T)control, index);
        }

        /// <summary>
        /// Invoked on clear all elements.
        /// </summary>
        /// <param name="control">The control</param>
        public void OnClear(object control)
        {
            this.OnClear((T)control);
        }

    }
}