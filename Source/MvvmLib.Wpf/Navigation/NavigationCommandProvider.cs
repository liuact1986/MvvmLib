using MvvmLib.Commands;
using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides commands for navigation.
    /// </summary>
    public abstract class BrowsableCommandProvider
    {
        /// <summary>
        /// Allows to move to the first source.
        /// </summary>
        protected IDelegateCommand moveToFirstCommand;
        /// <summary>
        /// Allows to move to the first source.
        /// </summary>
        public IDelegateCommand MoveToFirstCommand
        {
            get
            {
                if (moveToFirstCommand == null)
                    moveToFirstCommand = new DelegateCommand(ExecuteMoveToFirstCommand, CanExecuteMoveToFirstCommand);
                return moveToFirstCommand;
            }
        }

        /// <summary>
        /// Allows to move to the previous source.
        /// </summary>
        protected IDelegateCommand moveToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous source.
        /// </summary>
        public IDelegateCommand MoveToPreviousCommand
        {
            get
            {
                if (moveToPreviousCommand == null)
                    moveToPreviousCommand = new DelegateCommand(ExecuteMoveToPreviousCommand, CanExecuteMoveToPreviousCommand);
                return moveToPreviousCommand;
            }
        }

        /// <summary>
        /// Allows to move to the next source.
        /// </summary>
        protected IDelegateCommand moveToNextCommand;
        /// <summary>
        /// Allows to move to the next source.
        /// </summary>
        public IDelegateCommand MoveToNextCommand
        {
            get
            {
                if (moveToNextCommand == null)
                    moveToNextCommand = new DelegateCommand(ExecuteMoveToNextCommand, CanExecuteMoveToNextCommand);
                return moveToNextCommand;
            }
        }

        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        protected IDelegateCommand moveToLastCommand;
        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        public IDelegateCommand MoveToLastCommand
        {
            get
            {
                if (moveToLastCommand == null)
                    moveToLastCommand = new DelegateCommand(ExecuteMoveToLastCommand, CanExecuteMoveToLastCommand);
                return moveToLastCommand;
            }
        }

        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        protected IDelegateCommand moveToIndexCommand;
        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        public IDelegateCommand MoveToIndexCommand
        {
            get
            {
                if (moveToIndexCommand == null)
                    moveToIndexCommand = new DelegateCommand<object>(ExecuteMoveToIndexCommand);
                return moveToIndexCommand;
            }
        }

        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        protected IDelegateCommand moveToCommand;
        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        public IDelegateCommand MoveToCommand
        {
            get
            {
                if (moveToCommand == null)
                    moveToCommand = new DelegateCommand<object>(ExecuteMoveToCommand);
                return moveToCommand;
            }
        }

        /// <summary>
        /// The method invoked by the <see cref="MoveToFirstCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToFirstCommand();

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToFirstCommand"/> can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToFirstCommand();

        /// <summary>
        /// The method invoked by the <see cref="MoveToPreviousCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToPreviousCommand();

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToPreviousCommand"/> can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToPreviousCommand();

        /// <summary>
        /// The method invoked by the <see cref="MoveToNextCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToNextCommand();

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToNextCommand"/> can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToNextCommand();

        /// <summary>
        /// The method invoked by the <see cref="MoveToLastCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToLastCommand();

        /// <summary>
        /// The method invoked to check if the <see cref="MoveToLastCommand"/> can be executed.
        /// </summary>
        protected abstract bool CanExecuteMoveToLastCommand();

        /// <summary>
        /// The method invoked by the <see cref="MoveToIndexCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToIndexCommand(object args);

        /// <summary>
        /// The method invoked by the <see cref="MoveToCommand"/>.
        /// </summary>
        protected abstract void ExecuteMoveToCommand(object args);
    }


    /// <summary>
    /// Provides commands for navigation.
    /// </summary>
    public abstract class NavigationCommandProvider : BrowsableCommandProvider
    {
        /// <summary>
        /// Allows to navigate to the source with the source type provided.
        /// </summary>
        protected IDelegateCommand navigateCommand;
        /// <summary>
        /// Allows to navigate to the source with the source type provided.
        /// </summary>
        public IDelegateCommand NavigateCommand
        {
            get
            {
                if (navigateCommand == null)
                    navigateCommand = new DelegateCommand<Type>(ExecuteNavigateCommand);
                return navigateCommand;
            }
        }

        /// <summary>
        /// The method invoked by the <see cref="NavigateCommand"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        protected abstract void ExecuteNavigateCommand(Type sourceType);
    }
}
