using MvvmLib.Commands;
using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to move between items/ sources. 
    /// </summary>
    public interface IMovableSource
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
        /// Allows to move to the first item.
        /// </summary>
        IDelegateCommand MoveToFirstCommand { get; }

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        IDelegateCommand MoveToPreviousCommand { get; }

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        IDelegateCommand MoveToNextCommand { get; }

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        IDelegateCommand MoveToLastCommand { get; }

        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        IDelegateCommand MoveToIndexCommand { get; }

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        IDelegateCommand MoveToCommand { get; }

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
        void MoveToFirst();

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        void MoveToPrevious();

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        void MoveToNext();

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        void MoveToLast();

        /// <summary>
        /// Allows to move to the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        void MoveTo(int index);

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        /// <param name="item">The item</param>
        void MoveTo(object item);
    }

}
