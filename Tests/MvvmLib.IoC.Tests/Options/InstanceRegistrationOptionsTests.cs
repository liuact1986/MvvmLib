using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace MvvmLib.IoC.Tests.Options
{
    [TestClass]
    public class InstanceRegistrationOptionsTests
    {
        [TestMethod]
        public void Set_OnResolved()
        {
            var registration = new InstanceRegistration(typeof(Item), "item", new Item());

            var service = Activator.CreateInstance(typeof(InstanceRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration }, null) as InstanceRegistrationOptions;

            Assert.AreEqual(null, registration.OnResolved);

            var action = new Action<ContainerRegistration, object>((c, p) => { });

            service.OnResolved(action);

            Assert.AreEqual(action, registration.OnResolved);
        }
    }
}
