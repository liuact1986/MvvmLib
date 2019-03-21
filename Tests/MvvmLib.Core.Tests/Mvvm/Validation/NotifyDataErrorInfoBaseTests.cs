using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{
    [TestClass]
    public class NotifyDataErrorInfoBaseTests
    {

        [TestMethod]
        public void AddError()
        {
            var service = new NotifyDataErrorInfoBase();

            service.AddError("p1", "e1");

            Assert.IsTrue(service.HasErrors);
            Assert.IsTrue(service.ContainErrors("p1"));
            Assert.IsTrue(service.ContainError("p1", "e1"));

            var errors = service.GetErrors("p1").Cast<string>().ToList();

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("e1", errors[0]);


            service.AddError("p1", "e2");

            Assert.IsTrue(service.ContainError("p1", "e2"));

            var errors2 = service.GetErrors("p1").Cast<string>().ToList();
            Assert.AreEqual(2, errors2.Count);
            Assert.AreEqual("e1", errors2[0]);
            Assert.AreEqual("e2", errors2[1]);

            service.ClearErrors("p1");
            Assert.IsFalse(service.HasErrors);
            Assert.IsFalse(service.ContainErrors("p1"));
            Assert.IsFalse(service.ContainError("p1", "e1"));
        }

        [TestMethod]
        public void ErrorsChanged()
        {
            bool isNotified = false;
            string propertyName = null;
            var service = new NotifyDataErrorInfoBase();
            service.ErrorsChanged += (s, e) =>
             {
                 isNotified = true;
                 propertyName = e.PropertyName;
             };

            service.AddError("p1", "e1");

            Assert.IsTrue(isNotified);
            Assert.AreEqual("p1", propertyName);
        }
    }
}
