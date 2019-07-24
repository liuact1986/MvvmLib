using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides commands for <see cref="IBrowsableSource"/>.
    /// </summary>
    public class SharedSourceCommands : BrowsableCommandProvider
    {
        private IBrowsableSource source;
        /// <summary>
        /// The source.
        /// </summary>
        public IBrowsableSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Creates the <see cref="SharedSourceCommands"/>.
        /// </summary>
        /// <param name="source">The shared source</param>
        public SharedSourceCommands(IBrowsableSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.source = source;
            this.source.CanMoveToPreviousChanged += OnCanMoveToPreviousChanged;
            this.source.CanMoveToNextChanged += OnCanMoveToNextChanged;
        }

        #region Commands

        /// <summary>
        /// Invoked on CanMoveToNext changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected virtual void OnCanMoveToNextChanged(object sender, CanMoveToEventArgs e)
        {
            moveToLastCommand?.RaiseCanExecuteChanged();
            moveToNextCommand?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Invoked on CanMoveToPrevious changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected virtual void OnCanMoveToPreviousChanged(object sender, CanMoveToEventArgs e)
        {
            moveToFirstCommand?.RaiseCanExecuteChanged();
            moveToPreviousCommand?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// The method invoked by the MoveToFirstCommand.
        /// </summary>
        protected override void ExecuteMoveToFirstCommand()
        {
            this.source.MoveToFirst();
        }

        /// <summary>
        /// The method invoked to check if the MoveToFirstCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToFirstCommand()
        {
            return this.source.CanMoveToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveToPreviousCommand.
        /// </summary>
        protected override void ExecuteMoveToPreviousCommand()
        {
            this.source.MoveToPrevious();
        }

        /// <summary>
        /// The method invoked to check if the MoveToPreviousCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToPreviousCommand()
        {
            return this.source.CanMoveToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveToNextCommand.
        /// </summary>
        protected override void ExecuteMoveToNextCommand()
        {
            this.source.MoveToNext();
        }

        /// <summary>
        /// The method invoked to check if the MoveToNextCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToNextCommand()
        {
            return this.source.CanMoveToNext;
        }

        /// <summary>
        /// The method invoked by the MoveToLastCommand.
        /// </summary>
        protected override void ExecuteMoveToLastCommand()
        {
            this.source.MoveToLast();
        }

        /// <summary>
        /// The method invoked to check if the MoveToLastCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToLastCommand()
        {
            return this.source.CanMoveToNext;
        }

        /// <summary>
        /// The method invoked by the MoveToIndexCommand.
        /// </summary>
        protected override void ExecuteMoveToIndexCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int index))
                {
                    this.source.MoveTo(index);
                }
            }
        }

        /// <summary>
        /// The method invoked by the MoveToCommand.
        /// </summary>
        protected override void ExecuteMoveToCommand(object args)
        {
            this.source.MoveTo(args);
        }

        #endregion // Commands 
    }
}
