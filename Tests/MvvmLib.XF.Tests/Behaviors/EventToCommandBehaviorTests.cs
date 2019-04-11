using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using Xamarin.Forms;

namespace MvvmLib.XF.Tests.Behaviors
{
    [TestClass]
    public class EventToCommandBehaviorTests
    {
        [TestMethod]
        public void CallCommand_OnItemSelected()
        {
            bool isCalled = false;

            var service = new EventToCommandBehavior();
            service.EventName = "ItemSelected";
            service.Command = new Command(() =>
            {
                isCalled = true;
            });

            var c = new ListView();
            c.Behaviors.Add(service);

            c.SelectedItem = "new item";

            Assert.IsTrue(isCalled);
        }
    }
}
