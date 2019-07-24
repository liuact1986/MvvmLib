using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Navigation.CommandProviders
{
    [TestClass]
    public class SharedSourceCommandsTests
    {
        [TestMethod]
        public void Navigate_With_Commands_Update_CanExecute()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            var commands = new SharedSourceCommands(sharedSource);

            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            var itemA = new MySharedItem();
            sharedSource.Add(itemA);

            Assert.AreEqual(true, itemA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            var itemB = new MySharedItem();
            sharedSource.Add(itemB);

            Assert.AreEqual(true, itemB.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            commands.MoveToPreviousCommand.Execute(null); // B => A
            Assert.AreEqual(itemA, sharedSource.SelectedItem);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToLastCommand.CanExecute(null));

            commands.MoveToNextCommand.Execute(null); // A => B
            Assert.AreEqual(itemB, sharedSource.SelectedItem);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            commands.MoveToFirstCommand.Execute(null);
            Assert.AreEqual(itemA, sharedSource.SelectedItem);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToNextCommand.CanExecute(null)); // not clear backstack
            Assert.AreEqual(true, commands.MoveToLastCommand.CanExecute(null));

            commands.MoveToLastCommand.Execute(null);
            Assert.AreEqual(itemB, sharedSource.SelectedItem);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));
        }

    }

}
