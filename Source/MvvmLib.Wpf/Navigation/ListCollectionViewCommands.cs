using System;
using System.ComponentModel;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The implementation of <see cref="EditableCollectionCommandProvider"/> for <see cref="ListCollectionView"/>.
    /// </summary>
    public class ListCollectionViewCommands : EditableCollectionCommandProvider
    {
        /// <summary>
        /// The <see cref="ListCollectionView"/>.
        /// </summary>
        protected ListCollectionView collectionView;
        /// <summary>
        /// The <see cref="ListCollectionView"/>.
        /// </summary>
        public ListCollectionView CollectionView
        {
            get { return collectionView; }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.collectionView.CurrentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.collectionView.CurrentPosition < this.collectionView.Count - 1; }
        }

        /// <summary>
        /// Creates the <see cref="ListCollectionViewCommands"/>.
        /// </summary>
        /// <param name="collectionView">The ListCollectionView</param>
        public ListCollectionViewCommands(ListCollectionView collectionView)
        {
            if (collectionView == null)
                throw new ArgumentNullException(nameof(collectionView));

            this.collectionView = collectionView;
            this.collectionView.CurrentChanged += OnCurrentChanged;
        }

        /// <summary>
        /// Invoked on current changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnCurrentChanged(object sender, EventArgs e)
        {
            moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
            moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
            moveCurrentToNextCommand?.RaiseCanExecuteChanged();
            moveCurrentToLastCommand?.RaiseCanExecuteChanged();
        }

        #region Commands

        /// <summary>
        /// The method invoked by the MoveCurrentToFirstCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToFirstCommand()
        {
            this.collectionView.MoveCurrentToFirst();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToFirstCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveCurrentToFirstCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToPreviousCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToPreviousCommand()
        {
            this.collectionView.MoveCurrentToPrevious();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToPreviousCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToPreviousCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToNextCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToNextCommand()
        {
            this.collectionView.MoveCurrentToNext();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToNextCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToNextCommand()
        {
            return CanMoveCurrentToNext;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToLastCommand.
        /// </summary>
        protected override void ExecuteMoveCurrentToLastCommand()
        {
            this.collectionView.MoveCurrentToLast();
        }

        /// <summary>
        /// The method invoked to check if the MoveCurrentToLastCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveCurrentToLastCommand()
        {
            return CanMoveCurrentToNext;
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteMoveCurrentToCommand(object args)
        {
            this.collectionView.MoveCurrentTo(args);
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
                    this.collectionView.MoveCurrentToPosition(position);
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
                    this.collectionView.MoveCurrentToPosition(rank - 1);
                }
            }
        }

        /// <summary>
        /// The method invoked by the AddNewCommand.
        /// </summary>
        protected override void ExecuteAddNewCommand()
        {
            this.collectionView.AddNew();
        }

        /// <summary>
        /// The method invoked by the EditCommand.
        /// </summary>
        protected override void ExecuteEditCommand()
        {
            EditCurrentItem();
        }

        /// <summary>
        /// The method invoked by the DeleteCommand.
        /// </summary>
        protected override void ExecuteDeleteCommand()
        {
            var currentItem = this.collectionView.CurrentItem;
            if (currentItem != null)
                this.collectionView.Remove(currentItem);
        }

        /// <summary>
        /// The method invoked by the SaveCommand.
        /// </summary>
        protected override void ExecuteSaveCommand()
        {
            Save();
        }

        /// <summary>
        /// The method invoked by the CancelCommand.
        /// </summary>
        protected override void ExecuteCancelCommand()
        {
            Cancel();
        }


        /// <summary>
        /// The method invoked by the ToggleGroupByCommand.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected override void ExecuteToggleGroupByCommand(string propertyName)
        {
            if (RemovePropertyDescription(propertyName))
                return;

            GroupBy(propertyName);
        }

        /// <summary>
        /// Removes the property group description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if removed</returns>
        protected bool RemovePropertyDescription(string propertyName)
        {
            foreach (var groupdDescription in this.collectionView.GroupDescriptions)
            {
                if (groupdDescription is PropertyGroupDescription)
                {
                    var propertyGroupDescription = groupdDescription as PropertyGroupDescription;
                    if (propertyGroupDescription.PropertyName == propertyName)
                    {
                        return this.collectionView.GroupDescriptions.Remove(propertyGroupDescription);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// The method invoked by the GroupByCommand.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected override void ExecuteGroupByCommand(string propertyName)
        {
            this.collectionView.GroupDescriptions.Clear();
            GroupBy(propertyName);
        }

        /// <summary>
        /// The method invoked by the SortByCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteSortByCommand(string args)
        {
            SortBy(args, true);
        }

        /// <summary>
        /// The method invoked by the SortByDescendingCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected override void ExecuteSortByDescendingCommand(string args)
        {
            SortByDescending(args, true);
        }

        /// <summary>
        /// The method invoked by the ClearFilterCommand.
        /// </summary>
        protected override void ExecuteClearFilterCommand()
        {
            this.ClearFilter();
        }

        #endregion // Commands

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void GroupBy(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            this.collectionView.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="sortDirection">The sort direction</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        protected virtual void SortBy(string propertyName, ListSortDirection sortDirection, bool clearSortDescriptions)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (clearSortDescriptions)
                this.collectionView.SortDescriptions.Clear();

            this.collectionView.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        protected virtual void SortByDescending(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Descending, clearSortDescriptions);
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        protected virtual void SortBy(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Ascending, clearSortDescriptions);
        }

        /// <summary>
        /// Clears the filter.
        /// </summary>
        protected void ClearFilter()
        {
            this.collectionView.Filter = null;
        }

        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        protected virtual void EditCurrentItem()
        {
            var currentItem = this.collectionView.CurrentItem;
            if (currentItem != null)
                this.collectionView.EditItem(currentItem);
        }

        /// <summary>
        /// Ends the edition.
        /// </summary>
        protected virtual void Save()
        {
            if (this.collectionView.IsAddingNew)
                this.collectionView.CommitNew();
            else if (this.collectionView.IsEditingItem)
                this.collectionView.CommitEdit();
        }

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        protected virtual void Cancel()
        {
            if (this.collectionView.IsAddingNew)
                this.collectionView.CancelNew();
            else if (this.collectionView.IsEditingItem && this.collectionView.CanCancelEdit)
                this.collectionView.CancelEdit();
        }
    }

}
