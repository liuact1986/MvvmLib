//using MvvmLib.Commands;
//using System;

//namespace MvvmLib.Navigation
//{
//    /// <summary>
//    /// Provides commands for <see cref="NavigationSource"/>.
//    /// </summary>
//    public class NavigationSourceCommandProvider
//    {
//        private NavigationSource navigationSource;
//        /// <summary>
//        /// The navigation source.
//        /// </summary>
//        public NavigationSource NavigationSource
//        {
//            get { return navigationSource; }
//        }

//        private IDelegateCommand navigateCommand;
//        /// <summary>
//        /// Allows to navigate to the source with the source type provided.
//        /// </summary>
//        public IDelegateCommand NavigateCommand
//        {
//            get
//            {
//                if (navigateCommand == null)
//                    navigateCommand = new DelegateCommand<Type>(ExecuteNavigateCommand);
//                return navigateCommand;
//            }
//        }

//        private IDelegateCommand moveToFirstCommand;
//        /// <summary>
//        /// Allows to move to the first source.
//        /// </summary>
//        public IDelegateCommand MoveToFirstCommand
//        {
//            get
//            {
//                if (moveToFirstCommand == null)
//                    moveToFirstCommand = new DelegateCommand(ExecuteMoveToFirstCommand, CanExecuteMoveToFirstCommand);
//                return moveToFirstCommand;
//            }
//        }

//        private IDelegateCommand moveToPreviousCommand;
//        /// <summary>
//        /// Allows to move to the previous source.
//        /// </summary>
//        public IDelegateCommand MoveToPreviousCommand
//        {
//            get
//            {
//                if (moveToPreviousCommand == null)
//                    moveToPreviousCommand = new DelegateCommand(ExecuteMoveToPreviousCommand, CanExecuteMoveToPreviousCommand);
//                return moveToPreviousCommand;
//            }
//        }

//        private IDelegateCommand moveToNextCommand;
//        /// <summary>
//        /// Allows to move to the next source.
//        /// </summary>
//        public IDelegateCommand MoveToNextCommand
//        {
//            get
//            {
//                if (moveToNextCommand == null)
//                    moveToNextCommand = new DelegateCommand(ExecuteMoveToNextCommand, CanExecuteMoveToNextCommand);
//                return moveToNextCommand;
//            }
//        }

//        private IDelegateCommand moveToLastCommand;
//        /// <summary>
//        /// Allows to move to the last source.
//        /// </summary>
//        public IDelegateCommand MoveToLastCommand
//        {
//            get
//            {
//                if (moveToLastCommand == null)
//                    moveToLastCommand = new DelegateCommand(ExecuteMoveToLastCommand, CanExecuteMoveToLastCommand);
//                return moveToLastCommand;
//            }
//        }

//        private IDelegateCommand moveToIndexCommand;
//        /// <summary>
//        /// Allows to move to the index.
//        /// </summary>
//        public IDelegateCommand MoveToIndexCommand
//        {
//            get
//            {
//                if (moveToIndexCommand == null)
//                    moveToIndexCommand = new DelegateCommand<object>(ExecuteMoveToIndexCommand);
//                return moveToIndexCommand;
//            }
//        }

//        private IDelegateCommand moveToCommand;
//        /// <summary>
//        /// Allows to move to the source.
//        /// </summary>
//        public IDelegateCommand MoveToCommand
//        {
//            get
//            {
//                if (moveToCommand == null)
//                    moveToCommand = new DelegateCommand<object>(ExecuteMoveToCommand);
//                return moveToCommand;
//            }
//        }

//        #region Commands

//        /// <summary>
//        /// Creates the <see cref="NavigationSourceCommandProvider"/>.
//        /// </summary>
//        /// <param name="navigationSource">The navigation source</param>
//        public NavigationSourceCommandProvider(NavigationSource navigationSource)
//        {
//            if (navigationSource == null)
//                throw new ArgumentNullException(nameof(navigationSource));

//            this.navigationSource = navigationSource;
//            this.navigationSource.CanMoveToPreviousChanged += OnCanMoveToPreviousChanged;
//            this.navigationSource.CanMoveToNextChanged += OnCanMoveToNextChanged;
//        }

//        protected virtual void OnCanMoveToNextChanged(object sender, CanMoveToEventArgs e)
//        {
//            moveToLastCommand?.RaiseCanExecuteChanged();
//            moveToNextCommand?.RaiseCanExecuteChanged();
//        }

//        protected virtual void OnCanMoveToPreviousChanged(object sender, CanMoveToEventArgs e)
//        {
//            moveToFirstCommand?.RaiseCanExecuteChanged();
//            moveToPreviousCommand?.RaiseCanExecuteChanged();
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="NavigateCommand"/>.
//        /// </summary>
//        /// <param name="sourceType">The source type</param>
//        protected virtual void ExecuteNavigateCommand(Type sourceType)
//        {
//            this.navigationSource.Navigate(sourceType, null);
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToFirstCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToFirstCommand()
//        {
//           this.navigationSource.MoveToFirst();
//        }

//        /// <summary>
//        /// The method invoked to check if the <see cref="MoveToFirstCommand"/> can be executed.
//        /// </summary>
//        protected virtual bool CanExecuteMoveToFirstCommand()
//        {
//            return this.navigationSource.CanMoveToPrevious;
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToPreviousCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToPreviousCommand()
//        {
//            this.navigationSource.MoveToPrevious();
//        }

//        /// <summary>
//        /// The method invoked to check if the <see cref="MoveToPreviousCommand"/> can be executed.
//        /// </summary>
//        protected virtual bool CanExecuteMoveToPreviousCommand()
//        {
//            return this.navigationSource.CanMoveToPrevious;
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToNextCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToNextCommand()
//        {
//            this.navigationSource.MoveToNext();
//        }

//        /// <summary>
//        /// The method invoked to check if the <see cref="MoveToNextCommand"/> can be executed.
//        /// </summary>
//        protected virtual bool CanExecuteMoveToNextCommand()
//        {
//            return this.navigationSource.CanMoveToNext;
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToLastCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToLastCommand()
//        {
//            this.navigationSource.MoveToLast();
//        }

//        /// <summary>
//        /// The method invoked to check if the <see cref="MoveToLastCommand"/> can be executed.
//        /// </summary>
//        protected virtual bool CanExecuteMoveToLastCommand()
//        {
//            return this.navigationSource.CanMoveToNext;
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToIndexCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToIndexCommand(object args)
//        {
//            if (args != null)
//            {
//                if (int.TryParse(args.ToString(), out int position))
//                {
//                    this.navigationSource.MoveTo(position);
//                }
//            }
//        }

//        /// <summary>
//        /// The method invoked by the <see cref="MoveToCommand"/>.
//        /// </summary>
//        protected virtual void ExecuteMoveToCommand(object args)
//        {
//            this.navigationSource.MoveTo(args);
//        }

//        #endregion // Commands 
//    }
//}
