using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Navigation.CommandProviders
{
    [TestClass]
    public class NavigationSourceCommandsTests
    {
        [TestMethod]
        public void Navigate_With_Commands_Update_CanExecute()
        {
            var navigationSource = new NavigationSource();
            var commands = new NavigationSourceCommands(navigationSource);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            commands.NavigateCommand.Execute(typeof(MyNavViewModelA)); // A
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.NavigateCommand.Execute(typeof(MyNavViewModelB)); //A => B
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToPreviousCommand.Execute(null); // B => A
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToLastCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToNextCommand.Execute(null); // A => B
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));


            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToFirstCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));
        }

        [TestMethod]
        public void Navigate_To_Last()
        {
            var navigationSource = new NavigationSource();
            var commands = new NavigationSourceCommands(navigationSource);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.NavigateCommand.Execute(typeof(MyNavViewModelA));
            commands.NavigateCommand.Execute(typeof(MyNavViewModelB));
            commands.NavigateCommand.Execute(typeof(MyNavViewModelC));
            commands.MoveToPreviousCommand.Execute(null);
            commands.MoveToPreviousCommand.Execute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            Assert.AreEqual(false, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToLastCommand.CanExecute(null));
            commands.MoveToLastCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, commands.MoveToFirstCommand.CanExecute(null));
            Assert.AreEqual(true, commands.MoveToPreviousCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToNextCommand.CanExecute(null));
            Assert.AreEqual(false, commands.MoveToLastCommand.CanExecute(null));
        }
    }

}
