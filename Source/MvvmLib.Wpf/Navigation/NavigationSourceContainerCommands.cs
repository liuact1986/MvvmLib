using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides commands for <see cref="NavigationSourceContainerCommands"/>.
    /// </summary>
    public class NavigationSourceContainerCommands : NavigationCommandProvider
    {
        private NavigationSourceContainer navigationSourceContainer;
        /// <summary>
        /// The navigation source container.
        /// </summary>
        public NavigationSourceContainer NavigationSourceContainer
        {
            get { return navigationSourceContainer; }
        }

        /// <summary>
        /// Creates the <see cref="NavigationSourceContainerCommands"/>.
        /// </summary>
        /// <param name="navigationSourceContainer">The navigation source container</param>
        public NavigationSourceContainerCommands(NavigationSourceContainer navigationSourceContainer)
        {
            if (navigationSourceContainer == null)
                throw new ArgumentNullException(nameof(navigationSourceContainer));

            this.navigationSourceContainer = navigationSourceContainer;
        }

        /// <summary>
        /// The method invoked by the NavigateCommand.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        protected override void ExecuteNavigateCommand(Type sourceType)
        {
            this.navigationSourceContainer.Navigate(sourceType);
        }

        /// <summary>
        /// The method invoked by the MoveToFirstCommand.
        /// </summary>
        protected override void ExecuteMoveToFirstCommand()
        {
            this.navigationSourceContainer.MoveToFirst();
        }

        /// <summary>
        /// The method invoked to check if the MoveToFirstCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToFirstCommand()
        {
            return true;
        }

        /// <summary>
        /// The method invoked by the MoveToPreviousCommand.
        /// </summary>
        protected override void ExecuteMoveToPreviousCommand()
        {
            this.navigationSourceContainer.MoveToPrevious();
        }

        /// <summary>
        /// The method invoked to check if the MoveToPreviousCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToPreviousCommand()
        {
            return true;
        }

        /// <summary>
        /// The method invoked by the MoveToNextCommand.
        /// </summary>
        protected override void ExecuteMoveToNextCommand()
        {
            this.navigationSourceContainer.MoveToNext();
        }

        /// <summary>
        /// The method invoked to check if the MoveToNextCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToNextCommand()
        {
            return true;
        }

        /// <summary>
        /// The method invoked by the MoveToLastCommand.
        /// </summary>
        protected override void ExecuteMoveToLastCommand()
        {
            this.navigationSourceContainer.MoveToLast();
        }

        /// <summary>
        /// The method invoked to check if the MoveToLastCommand can be executed.
        /// </summary>
        protected override bool CanExecuteMoveToLastCommand()
        {
            return true;
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
                    this.navigationSourceContainer.MoveTo(index);
                }
            }
        }

        /// <summary>
        /// The method invoked by the MoveToCommand.
        /// </summary>
        protected override void ExecuteMoveToCommand(object args)
        {
            this.navigationSourceContainer.MoveTo(args);
        }
    }
}
