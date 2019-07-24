using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to move between items/ sources. 
    /// </summary>
    public interface IBrowsableSource
    {
        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        bool CanMoveToPrevious { get; }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        bool CanMoveToNext { get; }

        /// <summary>
        /// Invoked on <see cref="CanMoveToPrevious"/> changed.
        /// </summary>
        event EventHandler<CanMoveToEventArgs> CanMoveToPreviousChanged;

        /// <summary>
        /// Invoked on <see cref="CanMoveToNext"/> changed.
        /// </summary>
        event EventHandler<CanMoveToEventArgs> CanMoveToNextChanged;

        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        /// <returns>True if navigation succeeds</returns>
        bool MoveToFirst();

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        /// <returns>True if navigation succeeds</returns>
        bool MoveToPrevious();

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        /// <returns>True if navigation succeeds</returns>
        bool MoveToNext();

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        /// <returns>True if navigation succeeds</returns>
        bool MoveToLast();

        /// <summary>
        /// Allows to move to the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>True if navigation succeeds</returns>
        bool MoveTo(int index);

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if navigation succeeds</returns>
        bool MoveTo(object item);
    }

}
