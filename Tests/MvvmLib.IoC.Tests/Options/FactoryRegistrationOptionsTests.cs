using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.IoC.Registrations;
using System;
using System.Reflection;
using System.Text;

namespace MvvmLib.IoC.Tests.Options
{
    [TestClass]
    public class FactoryRegistrationOptionsTests
    {
        [TestMethod]
        public void Set_OnResolved()
        {
            var registration = new FactoryRegistration(typeof(Item), "item", () => new Item());

            var service = Activator.CreateInstance(typeof(FactoryRegistrationOptions),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { registration }, null) as FactoryRegistrationOptions;


            Assert.AreEqual(null, registration.OnResolved);

            var action = new Action<ContainerRegistration, object>((c, p) => { });

            service.OnResolved(action);

            Assert.AreEqual(action, registration.OnResolved);
        }
    }
}
