using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{
    [TestClass]
    public class ValidatableAndEditableTests
    {
        [TestMethod]
        public void BeginEdit_And_Cancel()
        {
            var user = new UserValidatableAndEditable
            {
                FirstName = "Marie",
                LastName = "Bellin"
            };

            user.BeginEdit();

            user.FirstName = "updated firstname";
            user.LastName = "updated lastname";

            user.CancelEdit();

            Assert.AreEqual("Marie", user.FirstName);
            Assert.AreEqual("Bellin", user.LastName);
        }

    }

}
