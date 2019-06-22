using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class NavigationSourceContainerTests
    {
        [TestMethod]
        public void Commands_With_ViewModels()
        {
            var navigationSource = new NavigationSource();

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            Assert.AreEqual(false, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(false, navigationSource.NavigateToRootCommand.CanExecute(null));

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelA));
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(false, navigationSource.GoForwardCommand.CanExecute(null));
            Assert.AreEqual(true, navigationSource.NavigateToRootCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelB));
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(false, navigationSource.GoForwardCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoBackCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(true, navigationSource.GoForwardCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoForwardCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(false, navigationSource.GoForwardCommand.CanExecute(null));

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.RedirectCommand.Execute(typeof(MyNavViewModelC));
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatedToInvoked);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.NavigateToRootCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, navigationSource.GoBackCommand.CanExecute(null));
            Assert.AreEqual(false, navigationSource.GoForwardCommand.CanExecute(null));
        }

        [TestMethod]
        public void Commands_With_Views()
        {
            var navigationSourceContainer = new NavigationSourceContainer();
            var n1 = new NavigationSource();
            var n2 = new NavigationSource();
            navigationSourceContainer.Register(n1);
            navigationSourceContainer.Register(n2);

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

            navigationSourceContainer.NavigateCommand.Execute(typeof(MyNavViewA));
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

            navigationSourceContainer.NavigateCommand.Execute(typeof(MyNavViewB));
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

            navigationSourceContainer.GoBackCommand.Execute(null);
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

            navigationSourceContainer.GoForwardCommand.Execute(null);
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

            navigationSourceContainer.RedirectCommand.Execute(typeof(MyNavViewC));
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewC), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewC), n2.Current.GetType());
            var c3 = n1.Current;
            var c4 = n2.Current;
            Assert.AreNotEqual(c3, c4);
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

            navigationSourceContainer.NavigateToRootCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(typeof(MyNavViewA), n1.Current.GetType());
            Assert.AreEqual(typeof(MyNavViewA), n2.Current.GetType());
            Assert.AreEqual(1, n1.Sources.Count);
            Assert.AreEqual(1, n2.Sources.Count);
            Assert.AreEqual(0, n1.CurrentIndex);
            Assert.AreEqual(0, n2.CurrentIndex);
        }
    }

}
