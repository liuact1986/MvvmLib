using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Provides commands for <see cref="NavigationSource"/>.
    /// </summary>
    public class NavigationSourceCommands : NavigationCommandProvider
    {
        private NavigationSource navigationSource;
        /// <summary>
        /// The navigation source.
        /// </summary>
        public NavigationSource NavigationSource
        {
            get { return navigationSource; }
        }  

        /// <summary>
        /// Creates the <see cref="NavigationSourceCommands"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        public NavigationSourceCommands(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            this.navigationSource = navigationSource;
            this.navigationSource.CanMoveToPreviousChanged += OnCanMoveToPreviousChanged;
            this.navigationSource.CanMoveToNextChanged += OnCanMoveToNextChanged;
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
        /// The method invoked by the NavigateCommand.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        protected override void ExecuteNavigateCommand(Type sourceType)
        {
            this.navigationSource.Navigate(sourceType, null);
        }

        /// <summary>
        /// The method invoked by the MoveToFirstCommand.
        /// </summary>
        protected override void ExecuteMoveToFirstCommand()
        {
            this.navigationSource.MoveToFirst();
        }

        /// <summary>
        /// The method invoked to check if the MoveToFirstCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToFirstCommand()
        {
            return this.navigationSource.CanMoveToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveToPreviousCommand.
        /// </summary>
        protected override void ExecuteMoveToPreviousCommand()
        {
            this.navigationSource.MoveToPrevious();
        }

        /// <summary>
        /// The method invoked to check if the MoveToPreviousCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToPreviousCommand()
        {
            return this.navigationSource.CanMoveToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveToNextCommand.
        /// </summary>
        protected override void ExecuteMoveToNextCommand()
        {
            this.navigationSource.MoveToNext();
        }

        /// <summary>
        /// The method invoked to check if the MoveToNextCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToNextCommand()
        {
            return this.navigationSource.CanMoveToNext;
        }

        /// <summary>
        /// The method invoked by the MoveToLastCommand.
        /// </summary>
        protected override void ExecuteMoveToLastCommand()
        {
            this.navigationSource.MoveToLast();
        }

        /// <summary>
        /// The method invoked to check if the MoveToLastCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToLastCommand()
        {
            return this.navigationSource.CanMoveToNext;
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
                    this.navigationSource.MoveTo(index);
                }
            }
        }

        /// <summary>
        /// The method invoked by the MoveToCommand.
        /// </summary>
        protected override void ExecuteMoveToCommand(object args)
        {
            this.navigationSource.MoveTo(args);
        }

        #endregion // Commands 
    }
}
