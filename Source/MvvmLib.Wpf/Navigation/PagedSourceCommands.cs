using MvvmLib.Commands;
using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides commands for <see cref="PagedSource"/>.
    /// </summary>
    public class PagedSourceCommands : EditableCollectionCommandProvider
    {
        private bool oldCanMoveToPreviousPage;
        private bool oldCanMoveToNextPage;
        private bool oldCanMoveCurrentToPrevious;
        private bool oldCanMoveCurrentToNext;

        private PagedSource pagedSource;
        /// <summary>
        /// The pagedSource;
        /// </summary>
        public PagedSource PagedSource
        {
            get { return pagedSource; }
        }

        #region Paging

        private IDelegateCommand moveToFirstPageCommand;
        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        public IDelegateCommand MoveToFirstPageCommand
        {
            get
            {
                if (moveToFirstPageCommand == null)
                    moveToFirstPageCommand = new DelegateCommand(ExecuteMoveToFirstPageCommand, CanExecuteMoveToFirstPageCommand);

                return moveToFirstPageCommand;
            }
        }

        private IDelegateCommand moveToPreviousPageCommand;
        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        public IDelegateCommand MoveToPreviousPageCommand
        {
            get
            {
                if (moveToPreviousPageCommand == null)
                    moveToPreviousPageCommand = new DelegateCommand(ExecuteMoveToPreviousPageCommand, CanExecuteMoveToPreviousPageCommand);
                return moveToPreviousPageCommand;
            }
        }

        private IDelegateCommand moveToNextPageCommand;
        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        public IDelegateCommand MoveToNextPageCommand
        {
            get
            {
                if (moveToNextPageCommand == null)
                    moveToNextPageCommand = new DelegateCommand(ExecuteMoveToNextPageCommand, CanExecuteMoveToNextPageCommand);
                return moveToNextPageCommand;
            }
        }

        private IDelegateCommand moveToLastPageCommand;
        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        public IDelegateCommand MoveToLastPageCommand
        {
            get
            {
                if (moveToLastPageCommand == null)
                    moveToLastPageCommand = new DelegateCommand(ExecuteMoveToLastPageCommand, CanExecuteMoveToLastPageCommand);
                return moveToLastPageCommand;
            }
        }

        private IDelegateCommand moveToPageCommand;
        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        public IDelegateCommand MoveToPageCommand
        {
            get
            {
                if (moveToPageCommand == null)
                    moveToPageCommand = new DelegateCommand<object>(ExecuteMoveToPageCommand);
                return moveToPageCommand;
            }
        }

        #endregion // Paging

        /// <summary>
        /// Creates the <see cref="PagedSourceCommands"/>.
        /// </summary>
        /// <param name="pagedSource">The paged source</param>
        public PagedSourceCommands(PagedSource pagedSource)
        {
            if (pagedSource == null)
                throw new ArgumentNullException(nameof(pagedSource));

            this.pagedSource = pagedSource;
            this.pagedSource.PageChanged += OnPageChanged;
            this.pagedSource.CurrentChanged += OnCurrentChanged;
        }

        /// <summary>
        /// Invoked on page changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected virtual void OnPageChanged(object sender, PageChangeEventArgs e)
        {
            CheckCanMovePage();
        }

        /// <summary>
        /// Invoked on current item changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        protected virtual void OnCurrentChanged(object sender, EventArgs e)
        {
            CheckCanMoveCurrent();
        }

        #region Commands


        /// <summary>
        /// The method invoked by the SortByDescendingCommand.
        /// </summary>
        protected virtual void ExecuteMoveToFirstPageCommand()
        {
            this.pagedSource.MoveToFirstPage();
        }

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToFirstPageCommand"/> can be executed.
        /// </summary>
        protected virtual bool CanExecuteMoveToFirstPageCommand()
        {
            return this.pagedSource.CanMoveToPreviousPage;
        }

        /// <summary>
        /// The method invoked by the <see cref="MoveToPreviousPageCommand"/>.
        /// </summary>
        protected virtual void ExecuteMoveToPreviousPageCommand()
        {
            this.pagedSource.MoveToPreviousPage();
        }

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToPreviousPageCommand"/> can be executed.
        /// </summary>
        protected virtual bool CanExecuteMoveToPreviousPageCommand()
        {
            return this.pagedSource.CanMoveToPreviousPage;
        }

        /// <summary>
        /// The method invoked by the <see cref="MoveToPreviousPageCommand"/>.
        /// </summary>
        protected virtual void ExecuteMoveToNextPageCommand()
        {
            this.pagedSource.MoveToNextPage();
        }

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToNextPageCommand"/> can be executed.
        /// </summary>
        protected virtual bool CanExecuteMoveToNextPageCommand()
        {
            return this.pagedSource.CanMoveToNextPage;
        }

        /// <summary>
        /// The method invoked by the <see cref="MoveToLastPageCommand"/>.
        /// </summary>
        protected virtual void ExecuteMoveToLastPageCommand()
        {
            this.pagedSource.MoveToLastPage();
        }

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToLastPageCommand"/> can be executed.
        /// </summary>
        protected virtual bool CanExecuteMoveToLastPageCommand()
        {
            return this.pagedSource.CanMoveToNextPage;
        }

        /// <summary>
        /// The method invoked by the <see cref="MoveToPageCommand"/>.
        /// </summary>
        /// <param name="args">The argument</param>
        protected virtual void ExecuteMoveToPageCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int currentPage))
                {
                    this.pagedSource.MoveToPage(currentPage - 1);
                }
            }
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToFirstCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToFirstCommand()
        {
            this.pagedSource.MoveCurrentToFirst();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToFirstCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveCurrentToFirstCommand()
        {
            return this.pagedSource.CanMoveCurrentToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToPreviousCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToPreviousCommand()
        {
            this.pagedSource.MoveCurrentToPrevious();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToPreviousCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToPreviousCommand()
        {
            return this.pagedSource.CanMoveCurrentToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToNextCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToNextCommand()
        {
            this.pagedSource.MoveCurrentToNext();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToNextCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToNextCommand()
        {
            return this.pagedSource.CanMoveCurrentToNext;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToLastCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToLastCommand()
        {
            this.pagedSource.MoveCurrentToLast();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToLastCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveCurrentToLastCommand()
        {
            return this.pagedSource.CanMoveCurrentToNext;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteMoveCurrentToCommand(object args)
        {
            this.pagedSource.MoveCurrentTo(args);
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToPositionCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteMoveCurrentToPositionCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int position))
                {
                    this.pagedSource.MoveCurrentToPosition(position);
                }
            }
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToRankCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteMoveCurrentToRankCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int rank))
                {
                    this.pagedSource.MoveCurrentToPosition(rank - 1);
                }
            }
        }

        /// <summary>
        /// The method invoked by the AddNewCommand.
        /// </summary>
        protected override void ExecuteAddNewCommand()
        {
            this.pagedSource.AddNew();
        }

        /// <summary>
        /// The method invoked by the EditCommand.
        /// </summary>
        protected override void ExecuteEditCommand()
        {
            this.pagedSource.EditCurrentItem();
        }

        /// <summary>
        /// The method invoked by the DeleteCommand.
        /// </summary>
        protected override void ExecuteDeleteCommand()
        {
            int currentPosition = this.pagedSource.CurrentPosition;
            if (currentPosition != -1)
                this.pagedSource.RemoveAt(currentPosition);
        }

        /// <summary>
        /// The method invoked by the SaveCommand.
        /// </summary>
        protected override void ExecuteSaveCommand()
        {
            this.pagedSource.Save();
        }

        /// <summary>
        /// The method invoked by the CancelCommand.
        /// </summary>
        protected override void ExecuteCancelCommand()
        {
            this.pagedSource.Cancel();
        }

        /// <summary>
        /// The method invoked by the SortByCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteSortByCommand(string args)
        {
            this.pagedSource.SortBy(args, true);
        }

        /// <summary>
        /// The method invoked by the SortByDescendingCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteSortByDescendingCommand(string args)
        {
            this.pagedSource.SortByDescending(args, true);
        }

        /// <summary>
        /// The method invoked by the ClearFilterCommand.
        /// </summary>
        protected override void ExecuteClearFilterCommand()
        {
            this.ClearFilter();
        }

        /// <summary>
        /// The method invoked by the ToggleGroupByCommand.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected override void ExecuteToggleGroupByCommand(string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The method invoked by the GroupByCommand.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected override void ExecuteGroupByCommand(string propertyName)
        {
            throw new NotImplementedException();
        }

        #endregion // Commands

        /// <summary>
        /// Clears the filter.
        /// </summary>
        public void ClearFilter()
        {
            this.pagedSource.ClearFilter();
        }


        #region editing

        /// <summary>
        /// Ends the edition.
        /// </summary>
        public virtual void Save()
        {
            if (this.pagedSource.IsAddingNew)
                this.pagedSource.CommitNew();
            else if (this.pagedSource.IsEditingItem)
                this.pagedSource.CommitEdit();
        }

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        public virtual void Cancel()
        {
            if (this.pagedSource.IsAddingNew)
                this.pagedSource.CancelNew();
            else if (this.pagedSource.IsEditingItem)
                this.pagedSource.CancelEdit();
        }

        #endregion

        private void CheckCanMovePage()
        {
            var canMoveToPreviousPage = this.pagedSource.CanMoveToPreviousPage;
            var canMoveToNextPage = this.pagedSource.CanMoveToNextPage;
            if (canMoveToPreviousPage != oldCanMoveToPreviousPage)
            {
                moveToFirstPageCommand?.RaiseCanExecuteChanged();
                moveToPreviousPageCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveToPreviousPage = canMoveToPreviousPage;
            }
            if (canMoveToNextPage != oldCanMoveToNextPage)
            {
                moveToNextPageCommand?.RaiseCanExecuteChanged();
                moveToLastPageCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveToNextPage = canMoveToNextPage;
            }
        }

        private void CheckCanMoveCurrent()
        {
            var canMoveCurrentToPrevious = this.pagedSource.CanMoveCurrentToPrevious;
            var canMoveCurrentToNext = this.pagedSource.CanMoveCurrentToNext;
            if (canMoveCurrentToPrevious != oldCanMoveCurrentToPrevious)
            {
                moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
                moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveCurrentToPrevious = canMoveCurrentToPrevious;
            }
            if (canMoveCurrentToNext != oldCanMoveCurrentToNext)
            {
                moveCurrentToNextCommand?.RaiseCanExecuteChanged();
                moveCurrentToLastCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveCurrentToNext = canMoveCurrentToNext;
            }
        }    
    }

}
