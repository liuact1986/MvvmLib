using MvvmLib.Commands;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides commands to browse collections.
    /// </summary>
    public abstract class CollectionCommandProvider
    {
        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        protected IDelegateCommand moveCurrentToFirstCommand;
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

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        protected IDelegateCommand moveCurrentToPreviousCommand;
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

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        protected IDelegateCommand moveCurrentToNextCommand;
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

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        protected IDelegateCommand moveCurrentToLastCommand;
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

        /// <summary>
        /// Allows to move to the position.
        /// </summary>
        protected IDelegateCommand moveCurrentToPositionCommand;
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

        /// <summary>
        /// Allows to move to the rank (index + 1).
        /// </summary>
        protected IDelegateCommand moveCurrentToRankCommand;
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

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        protected IDelegateCommand moveCurrentToCommand;
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


        /// <summary>
        /// Allows to toggle grouping by property name.
        /// </summary>
        protected IDelegateCommand toggleGroupByCommand;
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

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        protected IDelegateCommand groupByCommand;
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

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        protected IDelegateCommand sortByCommand;
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

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        protected IDelegateCommand sortByDescendingCommand;
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
        /// Allows to clear the filter.
        /// </summary>
        protected IDelegateCommand clearFilterCommand;
        /// <summary>
        /// Allows to clear the filter.
        /// </summary>
        public IDelegateCommand ClearFilterCommand
        {
            get
            {
                if (sortByCommand == null)
                    sortByCommand = new DelegateCommand(ExecuteClearFilterCommand);
                return sortByCommand;
            }
        }

        /// <summary>
        /// The method invoked by the MoveCurrentToFirstCommand.
        /// </summary>
        protected abstract void ExecuteMoveCurrentToFirstCommand();

        /// <summary>
        /// The method invoked to check if the MoveCurrentToFirstCommand can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveCurrentToFirstCommand();

        /// <summary>
        /// The method invoked by the MoveCurrentToPreviousCommand.
        /// </summary>
        protected abstract void ExecuteMoveCurrentToPreviousCommand();

        /// <summary>
        /// The method invoked to check if the MoveCurrentToPreviousCommand can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToPreviousCommand();

        /// <summary>
        /// The method invoked by the MoveCurrentToNextCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveCurrentToNextCommand();

        /// <summary>
        /// The method invoked to check if the MoveCurrentToNextCommand can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToNextCommand();

        /// <summary>
        /// The method invoked by the MoveCurrentToLastCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveCurrentToLastCommand();

        /// <summary>
        /// The method invoked to check if the MoveCurrentToLastCommand can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveCurrentToLastCommand();

        /// <summary>
        /// The method invoked by the MoveCurrentToCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected abstract void ExecuteMoveCurrentToCommand(object args);

        /// <summary>
        /// The method invoked by the MoveCurrentToPositionCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected abstract void ExecuteMoveCurrentToPositionCommand(object args);

        /// <summary>
        /// The method invoked by the MoveCurrentToRankCommand.
        /// </summary>
        /// <param name="args">The argument</param>
        protected abstract void ExecuteMoveCurrentToRankCommand(object args);

        /// <summary>
        /// The method invoked by the <see cref="ToggleGroupByCommand"/>.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected abstract void ExecuteToggleGroupByCommand(string propertyName);

        /// <summary>
        /// The method invoked by the <see cref="GroupByCommand"/>.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected abstract void ExecuteGroupByCommand(string propertyName);

        /// <summary>
        /// The method invoked by the <see cref="SortByCommand"/>.
        /// </summary>
        /// <param name="args">The argument</param>
        protected abstract void ExecuteSortByCommand(string args);

        /// <summary>
        /// The method invoked by the <see cref="SortByDescendingCommand"/>.
        /// </summary>
        /// <param name="args">The argument</param>
        protected abstract void ExecuteSortByDescendingCommand(string args);

        /// <summary>
        /// The method invoked by the <see cref="ClearFilterCommand"/>.
        /// </summary>
        protected abstract void ExecuteClearFilterCommand();
    }


    /// <summary>
    /// Provides commands to browse and edit collections.
    /// </summary>
    public abstract class EditableCollectionCommandProvider : CollectionCommandProvider
    {   
        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        protected IDelegateCommand addNewCommand;
        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        public IDelegateCommand AddNewCommand
        {
            get
            {
                if (addNewCommand == null)
                    addNewCommand = new DelegateCommand(ExecuteAddNewCommand);
                return addNewCommand;
            }
        }

        /// <summary>
        /// Allows to begin edit the current item.
        /// </summary>
        protected IDelegateCommand editCommand;
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

        /// <summary>
        /// Allows to delete the current item.
        /// </summary>
        protected IDelegateCommand deleteCommand;
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

        /// <summary>
        /// Allows to save changes for current item.
        /// </summary>
        protected IDelegateCommand saveCommand;
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

        /// <summary>
        /// Allows to cancel add new or edit item.
        /// </summary>
        protected IDelegateCommand cancelCommand;
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
   
        /// <summary>
        /// The method invoked by the <see cref="AddNewCommand"/>.
        /// </summary>
        protected abstract void ExecuteAddNewCommand();

        /// <summary>
        /// The method invoked by the <see cref="EditCommand"/>.
        /// </summary>
        protected abstract void ExecuteEditCommand();

        /// <summary>
        /// The method invoked by the <see cref="DeleteCommand"/>.
        /// </summary>
        protected abstract void ExecuteDeleteCommand();

        /// <summary>
        /// The method invoked by the <see cref="SaveCommand"/>.
        /// </summary>
        protected abstract void ExecuteSaveCommand();

        /// <summary>
        /// The method invoked by the <see cref="CancelCommand"/>.
        /// </summary>
        protected abstract void ExecuteCancelCommand();
    }
}
