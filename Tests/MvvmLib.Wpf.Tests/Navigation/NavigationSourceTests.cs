using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.History;
using MvvmLib.Navigation;
using System;
using System.Linq;
using System.Windows.Controls;

namespace MvvmLib.Wpf.Tests.Navigation
{

    [TestClass]
    public class NavigationSourceTests
    {

        [TestMethod]
        public void Navigate_With_Source_Name()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();

            SourceResolver.RegisterTypeForNavigation<MyViewModelNavigationAwareAndGuardsStatic>("A");

            navigationSource.Navigate("A");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source);
            Assert.AreEqual(null, navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void Navigate_With_Source_Name_And_Parameter()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();

            SourceResolver.RegisterTypeForNavigation<MyViewModelNavigationAwareAndGuardsStatic>("B");

            navigationSource.Navigate("B", "p1");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p1", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source);
            Assert.AreEqual("p1", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void Navigate_With_ViewModel()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;

            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic), "p");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic), "p2");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source);
            Assert.AreEqual("p2", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            // can deactivate
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p3");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source); // history
            Assert.AreEqual("p2", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p4");
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual("p4", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            var nextSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(nextSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic2), navigationSource.History.Current.SourceType);
            Assert.AreEqual(nextSource, navigationSource.History.Current.Source); // history
            Assert.AreEqual("p4", navigationSource.History.Current.Parameter);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
        }

        [TestMethod]
        public void Navigate_With_View()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CActivate = false;

            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CActivate = false;
            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p2");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p2", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p2", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            navigationSource.Navigate(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), "p3");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanActivateInvoked);
            Assert.AreEqual("p3", MYVIEWWITHViewModelNavigationAwareAndGuards.PCanActivate);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanActivateInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.PCanActivate);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual("p3", MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            var currentSource = navigationSource.Current;
            Assert.IsNotNull(currentSource);
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), currentSource.GetType());
            Assert.AreEqual(0, navigationSource.CurrentIndex);
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source);
            Assert.AreEqual("p3", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);


            // can deactivate
            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MYVIEWWITHViewModelNavigationAwareAndGuards.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p3");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source); // history
            Assert.AreEqual("p3", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            MYVIEWWITHViewModelNavigationAwareAndGuards.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            MyViewModelNavigationAwareAndGuardsStatic.CDeactivate = false;
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p4");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MYVIEWWITHViewModelNavigationAwareAndGuards), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source); // history
            Assert.AreEqual("p3", navigationSource.History.Current.Parameter);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);

            MyViewModelNavigationAwareAndGuardsStatic.Reset();
            MyViewModelNavigationAwareAndGuardsStatic2.Reset();
            navigationSource.Navigate(typeof(MyViewModelNavigationAwareAndGuardsStatic2), "p5");
            Assert.AreEqual(true, MYVIEWWITHViewModelNavigationAwareAndGuards.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatingTo);
            Assert.AreEqual(false, MYVIEWWITHViewModelNavigationAwareAndGuards.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MYVIEWWITHViewModelNavigationAwareAndGuards.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatingTo);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyViewModelNavigationAwareAndGuardsStatic.POnNavigatedTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsCanActivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatingToInvoked);
            Assert.AreEqual("p5", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatingTo);
            Assert.AreEqual(true, MyViewModelNavigationAwareAndGuardsStatic2.IsOnNavigatedToInvoked);
            Assert.AreEqual("p5", MyViewModelNavigationAwareAndGuardsStatic2.POnNavigatedTo);
            var nextSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(nextSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyViewModelNavigationAwareAndGuardsStatic2), navigationSource.History.Current.SourceType);
            Assert.AreEqual(nextSource, navigationSource.History.Current.Source); // history
            Assert.AreEqual("p5", navigationSource.History.Current.Parameter);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
        }

        [TestMethod]
        public void SideNav()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(navigationSource.Current, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(1, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Current.SourceType);
            Assert.AreEqual(navigationSource.Current, navigationSource.History.Current.Source);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual("A", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());

            navigationSource.Navigate(typeof(MyNavViewB), "B");
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(navigationSource.Current, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Current.SourceType);
            Assert.AreEqual(navigationSource.Current, navigationSource.History.Current.Source);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual("B", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            navigationSource.Navigate(typeof(MyNavViewC), "C");
            var currentSource = navigationSource.Current;
            Assert.AreEqual(2, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(currentSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.History.Current.SourceType);
            Assert.AreEqual(currentSource, navigationSource.History.Current.Source);
            Assert.AreEqual(2, navigationSource.History.CurrentIndex);
            Assert.AreEqual("C", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Previous.SourceType);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            // go back C => B
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewB.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelB.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.GoBack();
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewB.PCanActivate);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanActivateInvoked);
            Assert.AreEqual("B", MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual("B", MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual("B", MyNavViewModelB.POnNavigatedTo);
            var prevSource = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(prevSource, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Current.SourceType);
            Assert.AreEqual(prevSource, navigationSource.History.Current.Source);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual("B", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.History.Next.SourceType);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Previous.SourceType);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());

            // go forward B => C
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewB.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(null, MyNavViewModelB.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelB.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoForward);
            navigationSource.GoForward();
            Assert.AreEqual(true, MyNavViewB.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelB.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelB.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelB.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewC.PCanActivate);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanActivateInvoked);
            Assert.AreEqual("C", MyNavViewModelC.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual("C", MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual("C", MyNavViewModelC.POnNavigatedTo);
            var s2 = navigationSource.Current;
            Assert.AreEqual(2, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s2, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s2, navigationSource.History.Current.Source);
            Assert.AreEqual(2, navigationSource.History.CurrentIndex);
            Assert.AreEqual("C", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Previous.SourceType);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex - 1).GetType());

            // move C => A
            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelC.CDeactivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewA.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(false, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual(null, MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            MyNavViewModelA.CActivate = false;
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelA.POnNavigatedTo);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();
            Assert.AreEqual(true, navigationSource.CanGoBack);
            navigationSource.MoveTo(0);
            Assert.AreEqual(true, MyNavViewC.IsCanDeactivateInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelC.IsCanDeactivateInvoked);
            Assert.AreEqual(true, MyNavViewModelC.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewModelC.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewModelC.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewA.PCanActivate);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatingTo);
            Assert.AreEqual(false, MyNavViewA.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, MyNavViewA.POnNavigatedTo);
            Assert.AreEqual(true, MyNavViewModelA.IsCanActivateInvoked);
            Assert.AreEqual("A", MyNavViewModelA.PCanActivate);
            Assert.AreEqual(false, MyNavViewModelA.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatingToInvoked);
            Assert.AreEqual("A", MyNavViewModelA.POnNavigatingTo);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            Assert.AreEqual("A", MyNavViewModelA.POnNavigatedTo);
            var s3 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s3, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s3, navigationSource.History.Current.Source);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual("A", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Next.SourceType);
            Assert.AreEqual(null, navigationSource.History.Previous);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());
        }

        [TestMethod]
        public void Navigate_Clear_Entries_And_Sources_After_CurrentIndex()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.GoBack();
            navigationSource.GoBack();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(s1, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 1).GetType());
            Assert.AreEqual(typeof(MyNavViewC), navigationSource.Sources.ElementAt(navigationSource.CurrentIndex + 2).GetType());
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual("A", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(typeof(MyNavViewB), navigationSource.History.Next.SourceType);
            Assert.AreEqual(null, navigationSource.History.Previous);

            navigationSource.Navigate(typeof(MyNavViewD), "D");
            // [A] D
            var s2 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(typeof(MyNavViewD), s2.GetType());
            Assert.AreEqual(s2, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewD), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s2, navigationSource.History.Current.Source);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual("D", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Previous.SourceType);
        }

        [TestMethod]
        public void NavigateToRoot()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.NavigateToRoot();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(s1, navigationSource.Sources.ElementAt(navigationSource.CurrentIndex));
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Sources.ElementAt(0).GetType());
            Assert.AreEqual(1, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual("A", navigationSource.History.Current.Parameter);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Root.SourceType);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);
        }

        [TestMethod]
        public void Clear()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");

            navigationSource.Clear();
            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(-1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(null, navigationSource.Current);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
        }

        [TestMethod]
        public void Redirect()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");

            navigationSource.Redirect(typeof(MyViewModelRedirect), "B");
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(1, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual("B", navigationSource.History.Current.Parameter);
            Assert.AreEqual(1, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Previous);
        }

        [TestMethod]
        public void Redirect_With_Entries_Before()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            navigationSource.Redirect(typeof(MyViewModelRedirect), "C");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual("C", navigationSource.History.Current.Parameter);
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Previous.SourceType);
        }

        [TestMethod]
        public void Redirect_With_Source_Name()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            SourceResolver.RegisterTypeForNavigation<MyViewModelRedirect>("Y");

            navigationSource.Redirect("Y");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual(null, navigationSource.History.Current.Parameter);
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Previous.SourceType);

            SourceResolver.ClearTypesForNavigation();
        }


        [TestMethod]
        public void Redirect_With_Source_Name_And_Parameter()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");

            SourceResolver.RegisterTypeForNavigation<MyViewModelRedirect>("Z");

            navigationSource.Redirect("Z", "C");
            var s1 = navigationSource.Current;
            Assert.AreEqual(1, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.Current.GetType());
            Assert.AreEqual(2, navigationSource.Sources.Count);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(typeof(MyViewModelRedirect), navigationSource.History.Current.SourceType);
            Assert.AreEqual(s1, navigationSource.History.Current.Source);
            Assert.AreEqual("C", navigationSource.History.Current.Parameter);
            Assert.AreEqual(2, navigationSource.History.Entries.Count);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Previous.SourceType);

            SourceResolver.ClearTypesForNavigation();
        }

        [TestMethod]
        public void MoveTo_With_Source()
        {
            var navigationSource = new NavigationSource();
            Assert.IsNull(navigationSource.Current);
            Assert.AreEqual(-1, navigationSource.CurrentIndex);
            Assert.AreEqual(0, navigationSource.Sources.Count);
            Assert.AreEqual(null, navigationSource.History.Current);
            Assert.AreEqual(-1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(0, navigationSource.History.Entries.Count);
            Assert.AreEqual(null, navigationSource.History.Root);
            Assert.AreEqual(null, navigationSource.History.Next);
            Assert.AreEqual(null, navigationSource.History.Previous);

            MyNavViewA.Reset();
            MyNavViewModelA.Reset();
            MyNavViewB.Reset();
            MyNavViewModelB.Reset();
            MyNavViewC.Reset();
            MyNavViewModelC.Reset();

            navigationSource.Navigate(typeof(MyNavViewA), "A");
            navigationSource.Navigate(typeof(MyNavViewB), "B");
            navigationSource.Navigate(typeof(MyNavViewC), "C");


            navigationSource.MoveTo(navigationSource.Sources.First());

            // A [B C]
            var s1 = navigationSource.Current;
            Assert.AreEqual(0, navigationSource.CurrentIndex); // sources
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.Current.GetType());
            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(0, navigationSource.History.CurrentIndex);
            Assert.AreEqual(typeof(MyNavViewA), navigationSource.History.Current.SourceType);
            Assert.AreEqual(navigationSource.Current, navigationSource.History.Current.Source);
            Assert.AreEqual("A", navigationSource.History.Current.Parameter);
            Assert.AreEqual(3, navigationSource.History.Entries.Count);
        }

        [TestMethod]
        public void Sync_With_History()
        {
            var navigationSource = new NavigationSource();
            var history = new NavigationHistory();
            var eA = new NavigationEntry(typeof(MyNavViewA), new MyNavViewA(), "A");
            var eB = new NavigationEntry(typeof(MyNavViewB), new MyNavViewB(), "B");
            var eC = new NavigationEntry(typeof(MyNavViewC), new MyNavViewC(), "C");
            history.Navigate(eA);
            history.Navigate(eB);
            history.Navigate(eC);
            history.GoBack();
            Assert.AreEqual(3, history.Entries.Count);
            Assert.AreEqual(eA, history.Entries.ElementAt(0));
            Assert.AreEqual(eB, history.Entries.ElementAt(1));
            Assert.AreEqual(eC, history.Entries.ElementAt(2));
            Assert.AreEqual(1, history.CurrentIndex);
            Assert.AreEqual(eB, history.Current);

            navigationSource.Sync(history);

            Assert.AreEqual(3, navigationSource.History.Entries.Count);
            Assert.AreEqual(eA.SourceType, navigationSource.History.Entries.ElementAt(0).SourceType);
            Assert.AreEqual(eA.SourceType, navigationSource.History.Entries.ElementAt(0).Source.GetType());
            Assert.AreEqual(eA.Parameter, navigationSource.History.Entries.ElementAt(0).Parameter);
            Assert.AreEqual(eB.SourceType, navigationSource.History.Entries.ElementAt(1).SourceType);
            Assert.AreEqual(eB.SourceType, navigationSource.History.Entries.ElementAt(1).Source.GetType());
            Assert.AreEqual(eB.Parameter, navigationSource.History.Entries.ElementAt(1).Parameter);
            Assert.AreEqual(eC.SourceType, navigationSource.History.Entries.ElementAt(2).SourceType);
            Assert.AreEqual(eC.SourceType, navigationSource.History.Entries.ElementAt(2).Source.GetType());
            Assert.AreEqual(eC.Parameter, navigationSource.History.Entries.ElementAt(2).Parameter);
            Assert.AreEqual(1, navigationSource.History.CurrentIndex);
            Assert.AreEqual(eB.SourceType, navigationSource.History.Current.SourceType);
            Assert.AreEqual(eB.SourceType, navigationSource.History.Current.Source.GetType());
            Assert.AreEqual(eB.Parameter, navigationSource.History.Current.Parameter);

            Assert.AreEqual(3, navigationSource.Sources.Count);
            Assert.AreEqual(navigationSource.History.Entries.ElementAt(0).Source, navigationSource.Sources.ElementAt(0));
            Assert.AreEqual(navigationSource.History.Entries.ElementAt(1).Source, navigationSource.Sources.ElementAt(1));
            Assert.AreEqual(navigationSource.History.Entries.ElementAt(2).Source, navigationSource.Sources.ElementAt(2));
            Assert.AreEqual(1, navigationSource.CurrentIndex);
            Assert.AreEqual(navigationSource.History.Entries.ElementAt(1).Source, navigationSource.Current);
        }

        [TestMethod]
        public void Commands()
        {
            var navigationSource = new NavigationSource();

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.NavigateToRootCommand.CanExecute(null);

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelA));
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);
            navigationSource.NavigateToRootCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.NavigateCommand.Execute(typeof(MyNavViewModelB));
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoBackCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelA.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

            MyNavViewModelA.Reset();
            MyNavViewModelB.Reset();
            MyNavViewModelC.Reset();

            navigationSource.GoForwardCommand.Execute(null);
            Assert.AreEqual(true, MyNavViewModelB.IsOnNavigatedToInvoked);
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);

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
            navigationSource.GoBackCommand.CanExecute(null);
            navigationSource.GoForwardCommand.CanExecute(null);
        }
    }

    public class MyViewModelRedirect
    {

    }

    public class MyNavViewD : UserControl { }

    public class MyNavViewA : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewA()
        {
            this.DataContext = new MyNavViewModelA();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyNavViewB : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewB()
        {
            this.DataContext = new MyNavViewModelB();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyNavViewC : UserControl, INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public MyNavViewC()
        {
            this.DataContext = new MyNavViewModelC();
        }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyNavViewModelA : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }

        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyNavViewModelB : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyNavViewModelC : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CActivate = true;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MyViewModelNavigationAwareAndGuardsStatic2 : INavigationAware, ICanActivate, ICanDeactivate
    {
        public static bool IsOnNavigatedToInvoked { get; private set; }
        public static object POnNavigatedTo { get; private set; }
        public static bool IsOnNavigatingFromInvoked { get; private set; }
        public static bool IsOnNavigatingToInvoked { get; private set; }
        public static object POnNavigatingTo { get; private set; }

        public static bool CActivate { get; set; }
        public static bool IsCanActivateInvoked { get; private set; }
        public static object PCanActivate { get; set; }

        public static bool CDeactivate { get; set; }
        public static bool IsCanDeactivateInvoked { get; private set; }


        public static void Reset()
        {
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            CActivate = true;
            IsCanActivateInvoked = false;
            PCanActivate = null;
            CDeactivate = true;
            IsCanDeactivateInvoked = false;
        }

        public void CanDeactivate(Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void CanActivate(object parameter, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            c(CActivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

}
