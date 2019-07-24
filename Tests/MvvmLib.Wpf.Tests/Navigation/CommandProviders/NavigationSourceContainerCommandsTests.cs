using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Navigation.CommandProviders
{
    [TestClass]
    public class NavigationSourceContainerCommandsTests
    {
        //[TestMethod]
        //public void Commands_With_ViewModels()
        //{
        //    var navigationSource = new NavigationSource();
        //    var commands = new NavigationSourceCommands(navigationSource);

        //    MyNavViewModelA.Reset();
        //    MyNavViewModelB.Reset();
        //    MyNavViewModelC.Reset();

        //    Assert.AreEqual(false, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToFirstCommand.CanExecute(null));

        //    navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelA));
        //    Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(false, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToNextCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToFirstCommand.CanExecute(null));

        //    MyNavViewModelA.Reset();
        //    MyNavViewModelB.Reset();
        //    MyNavViewModelC.Reset();

        //    navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelB));
        //    Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(true, navigationSource.MoveToFirstCommand.CanExecute(null));
        //    Assert.AreEqual(true, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToNextCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToLastCommand.CanExecute(null));

        //    MyNavViewModelA.Reset();
        //    MyNavViewModelB.Reset();
        //    MyNavViewModelC.Reset();

        //    navigationSource.MoveToPreviousCommand.Execute(null);
        //    Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(false, navigationSource.MoveToFirstCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(true, navigationSource.MoveToNextCommand.CanExecute(null));
        //    Assert.AreEqual(true, navigationSource.MoveToLastCommand.CanExecute(null));

        //    MyNavViewModelA.Reset();
        //    MyNavViewModelB.Reset();
        //    MyNavViewModelC.Reset();

        //    navigationSource.MoveToNextCommand.Execute(null);
        //    Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(true, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToNextCommand.CanExecute(null));

        //    MyNavViewModelA.Reset();
        //    MyNavViewModelB.Reset();
        //    MyNavViewModelC.Reset();

        //    navigationSource.MoveToFirstCommand.Execute(null);
        //    Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(false, navigationSource.MoveToFirstCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToPreviousCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToNextCommand.CanExecute(null));
        //    Assert.AreEqual(false, navigationSource.MoveToLastCommand.CanExecute(null));
        //}

        [TestMethod]
        public void Commands_With_Views()
        {
            var navigationSourceContainer = new NavigationSourceContainer();
            var n1 = new NavigationSource();
            var n2 = new NavigationSource();
            navigationSourceContainer.Register(n1);
            navigationSourceContainer.Register(n2);
            var commands = new NavigationSourceContainerCommands(navigationSourceContainer);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            Assert.AreEqual(0, n1.Sources.Count);
            Assert.AreEqual(0, n2.Sources.Count);
            Assert.AreEqual(-1, n1.CurrentIndex);
            Assert.AreEqual(-1, n2.CurrentIndex);

            commands.NavigateCommand.Execute(typeof(MyNavViewA)); // navigate
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            var c1 = n1.Current;
            var c2 = n2.Current;
            Assert.AreEqual(typeof(MyNavViewA), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewA), n2.Current.GetType());
            Assert.AreNotEqual(c1, c2);
            Assert.AreEqual(1, n1.Sources.Count);
            Assert.AreEqual(1, n2.Sources.Count);
            Assert.AreEqual(0, n1.CurrentIndex);
            Assert.AreEqual(0, n2.CurrentIndex);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.NavigateCommand.Execute(typeof(MyNavViewB));
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewB), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewB), n2.Current.GetType());
            Assert.AreEqual(2, n1.Sources.Count);
            Assert.AreEqual(2, n2.Sources.Count);
            Assert.AreEqual(1, n1.CurrentIndex);
            Assert.AreEqual(1, n2.CurrentIndex);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToPreviousCommand.Execute(null); // previous
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewA), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewA), n2.Current.GetType());
            Assert.AreEqual(2, n1.Sources.Count);
            Assert.AreEqual(2, n2.Sources.Count);
            Assert.AreEqual(0, n1.CurrentIndex);
            Assert.AreEqual(0, n2.CurrentIndex);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToNextCommand.Execute(null); // next
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewB), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewB), n2.Current.GetType());
            Assert.AreEqual(2, n1.Sources.Count);
            Assert.AreEqual(2, n2.Sources.Count);
            Assert.AreEqual(1, n1.CurrentIndex);
            Assert.AreEqual(1, n2.CurrentIndex);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToFirstCommand.Execute(null); // first
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewA), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewA), n2.Current.GetType());
            Assert.AreEqual(1, n1.Sources.Count);
            Assert.AreEqual(1, n2.Sources.Count);
            Assert.AreEqual(0, n1.CurrentIndex);
            Assert.AreEqual(0, n2.CurrentIndex);
        }


        [TestMethod]
        public void Commands_With_Views_To_Last()
        {
            var navigationSourceContainer = new NavigationSourceContainer();
            var n1 = new NavigationSource();
            var n2 = new NavigationSource();
            navigationSourceContainer.Register(n1);
            navigationSourceContainer.Register(n2);
            var commands = new NavigationSourceContainerCommands(navigationSourceContainer);

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            Assert.AreEqual(0, n1.Sources.Count);
            Assert.AreEqual(0, n2.Sources.Count);
            Assert.AreEqual(-1, n1.CurrentIndex);
            Assert.AreEqual(-1, n2.CurrentIndex);

            commands.NavigateCommand.Execute(typeof(MyNavViewA)); // navigate
            commands.NavigateCommand.Execute(typeof(MyNavViewB));
            commands.NavigateCommand.Execute(typeof(MyNavViewC));
            commands.MoveToPreviousCommand.Execute(null); // previous
            commands.MoveToPreviousCommand.Execute(null); // previous // A

            MyNavViewA.Reset();
            MyNavViewB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            commands.MoveToLastCommand.Execute(null); // next
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewC), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewC), n2.Current.GetType());
        }
    }

}
