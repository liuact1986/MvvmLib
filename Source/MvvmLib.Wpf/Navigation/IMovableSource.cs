﻿using MvvmLib.Commands;
using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Source for Models and ViewModels with a collection of Items and SelectedItem/SelectedIndex. 
    /// It supports Views but its not the target. 
    /// This is the source for ItemsControls, Selectors (ListBox, TabControl), etc.
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
        IRelayCommand MoveToFirstCommand { get; }

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        IRelayCommand MoveToPreviousCommand { get; }

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        IRelayCommand MoveToNextCommand { get; }

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        IRelayCommand MoveToLastCommand { get; }

        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        IRelayCommand MoveToIndexCommand { get; }

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        IRelayCommand MoveToCommand { get; }

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