using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The items region adapter contract.
    /// </summary>
    public interface IItemsRegionAdapter
    {
        /// <summary>
        /// The type of the control used by the region adapter.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Invoked on insert new content.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="content">The content</param>
        /// <param name="index">The index</param>
        void OnInsert(object control, object content, int index);

        /// <summary>
        /// Invoked on remove element.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="index">The index</param>
        void OnRemoveAt(object control, int index);

        /// <summary>
        /// Invoked on clear all elements.
        /// </summary>
        /// <param name="control">The control</param>
        void OnClear(object control);
    }
}