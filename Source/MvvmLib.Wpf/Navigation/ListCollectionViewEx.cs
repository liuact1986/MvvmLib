using MvvmLib.Commands;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// ListCollectionView with commands and shortcuts.
    /// </summary>
    public class ListCollectionViewEx : ListCollectionView
    {
        private int rank;
        /// <summary>
        /// The rank (index + 1).
        /// </summary>
        public int Rank
        {
            get { return rank; }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.CurrentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.CurrentPosition < this.Count - 1; }
        }

        private IDelegateCommand moveCurrentToFirstCommand;
        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        public IDelegateCommand MoveCurrentToFirstCommand
        {
            get
            {
                if (moveCurrentToFirstCommand == null)
                    moveCurrentToFirstCommand = new DelegateCommand(ExecuteMoveCurrentToFirstCommand, CanExecuteMoveCurrentToFirstCommand);

                return moveCurrentToFirstCommand;
            }
        }

        private IDelegateCommand moveCurrentToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        public IDelegateCommand MoveCurrentToPreviousCommand
        {
            get
            {
                if (moveCurrentToPreviousCommand == null)
                    moveCurrentToPreviousCommand = new DelegateCommand(ExecuteMoveCurrentToPreviousCommand, CanExecuteMoveToPreviousCommand);
                return moveCurrentToPreviousCommand;
            }
        }

        private IDelegateCommand moveCurrentToNextCommand;
        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        public IDelegateCommand MoveCurrentToNextCommand
        {
            get
            {
                if (moveCurrentToNextCommand == null)
                    moveCurrentToNextCommand = new DelegateCommand(ExecuteMoveCurrentToNextCommand, CanExecuteMoveToNextCommand);
                return moveCurrentToNextCommand;
            }
        }

        private IDelegateCommand moveCurrentToLastCommand;
        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        public IDelegateCommand MoveCurrentToLastCommand
        {
            get
            {
                if (moveCurrentToLastCommand == null)
                    moveCurrentToLastCommand = new DelegateCommand(ExecuteMoveCurrentToLastCommand, CanExecuteMoveCurrentToLastCommand);
                return moveCurrentToLastCommand;
            }
        }

        private IDelegateCommand moveCurrentToPositionCommand;
        /// <summary>
        /// Allows to move to the position.
        /// </summary>
        public IDelegateCommand MoveCurrentToPositionCommand
        {
            get
            {
                if (moveCurrentToPositionCommand == null)
                    moveCurrentToPositionCommand = new DelegateCommand<object>(ExecuteMoveCurrentToPositionCommand);
                return moveCurrentToPositionCommand;
            }
        }

        private IDelegateCommand moveCurrentToRankCommand;
        /// <summary>
        /// Allows to move to the rank (index + 1).
        /// </summary>
        public IDelegateCommand MoveCurrentToRankCommand
        {
            get
            {
                if (moveCurrentToRankCommand == null)
                    moveCurrentToRankCommand = new DelegateCommand<object>(ExecuteMoveCurrentToRankCommand);
                return moveCurrentToRankCommand;
            }
        }

        private IDelegateCommand moveCurrentToCommand;
        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        public IDelegateCommand MoveCurrentToCommand
        {
            get
            {
                if (moveCurrentToCommand == null)
                    moveCurrentToCommand = new DelegateCommand<object>(ExecuteMoveCurrentToCommand);
                return moveCurrentToCommand;
            }
        }

        private IDelegateCommand addNewCommand;
        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        public IDelegateCommand AddNewCommand
        {
            get
            {
                if (addNewCommand == null)
                    addNewCommand = new DelegateCommand(ExecuteAddCommand);
                return addNewCommand;
            }
        }

        private IDelegateCommand editCommand;
        /// <summary>
        /// Allows to begin edit the current item.
        /// </summary>
        public IDelegateCommand EditCommand
        {
            get
            {
                if (editCommand == null)
                    editCommand = new DelegateCommand(ExecuteEditCommand);
                return editCommand;
            }
        }

        private IDelegateCommand deleteCommand;
        /// <summary>
        /// Allows to delete the current item.
        /// </summary>
        public IDelegateCommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new DelegateCommand(ExecuteDeleteCommand);
                return deleteCommand;
            }
        }

        private IDelegateCommand saveCommand;
        /// <summary>
        /// Allows to save changes for current item.
        /// </summary>
        public IDelegateCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new DelegateCommand(ExecuteSaveCommand);
                return saveCommand;
            }
        }

        private IDelegateCommand cancelCommand;
        /// <summary>
        /// Allows to cancel add new or edit item.
        /// </summary>
        public IDelegateCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new DelegateCommand(ExecuteCancelCommand);
                return cancelCommand;
            }
        }

        private IDelegateCommand toggleGroupByCommand;
        /// <summary>
        /// Allows to toggle grouping by property name.
        /// </summary>
        public IDelegateCommand ToggleGroupByCommand
        {
            get
            {
                if (toggleGroupByCommand == null)
                    toggleGroupByCommand = new DelegateCommand<string>(ExecuteToggleGroupByCommand);
                return toggleGroupByCommand;
            }
        }

        private IDelegateCommand groupByCommand;
        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        public IDelegateCommand GroupByCommand
        {
            get
            {
                if (groupByCommand == null)
                    groupByCommand = new DelegateCommand<string>(ExecuteGroupByCommand);
                return groupByCommand;
            }
        }

        private IDelegateCommand sortByCommand;
        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        public IDelegateCommand SortByCommand
        {
            get
            {
                if (sortByCommand == null)
                    sortByCommand = new DelegateCommand<string>(ExecuteSortByCommand);
                return sortByCommand;
            }
        }

        private IDelegateCommand sortByDescendingCommand;
        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        public IDelegateCommand SortByDescendingCommand
        {
            get
            {
                if (sortByDescendingCommand == null)
                    sortByDescendingCommand = new DelegateCommand<string>(ExecuteSortByDescendingCommand);
                return sortByDescendingCommand;
            }
        }

        /// <summary>
        /// Creates the <see cref="ListCollectionViewEx"/>.
        /// </summary>
        /// <param name="list">The source collection</param>
        public ListCollectionViewEx(IList list)
            : base(list)
        {
            this.CurrentChanged += OnCollectionViewCurrentChanged;
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            rank = this.CurrentPosition + 1;
            OnPropertyChanged(nameof(Rank));

            moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
            moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
            moveCurrentToNextCommand?.RaiseCanExecuteChanged();
            moveCurrentToLastCommand?.RaiseCanExecuteChanged();
        }

        #region Events

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Events

        #region Commands

        private void ExecuteMoveCurrentToFirstCommand()
        {
            this.MoveCurrentToFirst();
        }

        private bool CanExecuteMoveCurrentToFirstCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        private void ExecuteMoveCurrentToPreviousCommand()
        {
            this.MoveCurrentToPrevious();
        }

        private bool CanExecuteMoveToPreviousCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        private void ExecuteMoveCurrentToNextCommand()
        {
            this.MoveCurrentToNext();
        }

        private bool CanExecuteMoveToNextCommand()
        {
            return CanMoveCurrentToNext;
        }

        private void ExecuteMoveCurrentToLastCommand()
        {
            this.MoveCurrentToLast();
        }

        private bool CanExecuteMoveCurrentToLastCommand()
        {
            return CanMoveCurrentToNext;
        }

        private void ExecuteMoveCurrentToCommand(object args)
        {
            MoveCurrentTo(args);
        }

        private void ExecuteMoveCurrentToPositionCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int position))
                {
                    MoveCurrentToPosition(position);
                }
            }
        }

        private void ExecuteMoveCurrentToRankCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int rank))
                {
                    MoveCurrentToPosition(rank - 1);
                }
            }
        }

        private void ExecuteToggleGroupByCommand(string propertyName)
        {
            if (RemovePropertyDescription(propertyName))
                return;

            GroupBy(propertyName);
        }

        private bool RemovePropertyDescription(string propertyName)
        {
            foreach (var groupdDescription in this.GroupDescriptions)
            {
                if (groupdDescription is PropertyGroupDescription)
                {
                    var propertyGroupDescription = groupdDescription as PropertyGroupDescription;
                    if (propertyGroupDescription.PropertyName == propertyName)
                    {
                        return this.GroupDescriptions.Remove(propertyGroupDescription);
                    }
                }
            }
            return false;
        }

        private void ExecuteGroupByCommand(string propertyName)
        {
            this.GroupDescriptions.Clear();
            GroupBy(propertyName);
        }

        private void ExecuteSortByCommand(string args)
        {
            SortBy(args, true);
        }

        private void ExecuteSortByDescendingCommand(string args)
        {
            SortByDescending(args, true);
        }

        private void ExecuteAddCommand()
        {
            AddNew();
        }

        private void ExecuteEditCommand()
        {
            EditCurrentItem();
        }

        private void ExecuteDeleteCommand()
        {
            if (this.CurrentItem != null)
                Remove(this.CurrentItem);
        }

        private void ExecuteSaveCommand()
        {
            Save();
        }

        private void ExecuteCancelCommand()
        {
            Cancel();
        }

        #endregion // Commands

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <param name="itemType">The item type</param>
        /// <returns>The item created</returns>
        public object CreateNew(Type itemType)
        {
            var item = SourceResolver.CreateInstance(itemType);
            return item;
        }

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The item created</returns>
        public T CreateNew<T>()
        {
            return (T)CreateNew(typeof(T));
        }

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void GroupBy(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            this.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
        }

        /// <summary>
        /// Allows to define a filter with generics.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter</param>
        public void FilterBy<T>(Predicate<T> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            this.Filter = new Predicate<object>(p => filter((T)p));
        }

        private void SortBy(string propertyName, ListSortDirection sortDirection, bool clearSortDescriptions)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (clearSortDescriptions)
                this.SortDescriptions.Clear();

            this.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortByDescending(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Descending, clearSortDescriptions);
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortBy(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Ascending, clearSortDescriptions);
        }

        /// <summary>
        /// Clears the filter.
        /// </summary>
        public void ClearFilter()
        {
            this.Filter = null;
        }

        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        public void EditCurrentItem()
        {
            if (this.CurrentItem != null)
                this.EditItem(this.CurrentItem);
        }

        /// <summary>
        /// Ends the edition.
        /// </summary>
        public void Save()
        {
            if (this.IsAddingNew)
                this.CommitNew();
            else if (this.IsEditingItem)
                this.CommitEdit();
        }

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        public void Cancel()
        {
            if (this.IsAddingNew)
                    this.CancelNew();
            else if (this.IsEditingItem && this.CanCancelEdit)
                this.CancelEdit();
        }
    }
}
