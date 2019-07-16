using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;
using System.Linq;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class ValidatableTests
    {

        [TestMethod]
        public void TestCanValidateOnChange_WithOnChange()
        {

            var model = new UserValidatableAndEditable
            {
                FirstName = "Marie",
                LastName = "Bellin",
                ValidationType = ValidationHandling.OnPropertyChange
            };

            Assert.AreEqual(ValidationHandling.OnPropertyChange, model.ValidationType);
            Assert.IsTrue(model.CanValidateOnPropertyChanged);
        }

        [TestMethod]
        public void TestCanValidateOnChange_WithOnSubmit()
        {

            var model = new UserValidatableAndEditable
            {
                ValidationType = ValidationHandling.OnSubmit,
                FirstName = "Marie",
                LastName = "Bellin"
            };

            Assert.AreEqual(ValidationHandling.OnSubmit, model.ValidationType);
            Assert.IsFalse(model.CanValidateOnPropertyChanged);

            model.ValidateAll();

            Assert.IsTrue(model.CanValidateOnPropertyChanged);
        }

        [TestMethod]
        public void TestCanValidateOnChange_WithExplicit()
        {

            var model = new UserValidatableAndEditable
            {
                ValidationType = ValidationHandling.Explicit,
                FirstName = "Marie",
                LastName = "Bellin"
            };

            Assert.AreEqual(ValidationHandling.Explicit, model.ValidationType);
            Assert.IsFalse(model.CanValidateOnPropertyChanged);

            model.ValidateAll();

            Assert.IsFalse(model.CanValidateOnPropertyChanged);
        }

        [TestMethod]
        public void TestHasNoError()
        {

            var user = new UserValidatableAndEditable
            {
                FirstName ="Marie",
                LastName = "Bellin"
            };

            user.ValidateProperty("FirstName");
            var errors = user.GetErrors("FirstName");
            Assert.IsFalse(user.HasErrors);
            Assert.IsNull(errors);
        }

        [TestMethod]
        public void TestHasError()
        {
            var user = new UserValidatableAndEditable
            {
                FirstName = "M",
                LastName = "Bellin"
            };

            user.ValidateProperty("FirstName");

            var errors = user.GetErrors("FirstName");
            var r = errors.Cast<string>().ToList();

            Assert.IsTrue(user.HasErrors);
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("FirstName too short", r[0]);
        }

        [TestMethod]
        public void TestValidateAll()
        {
            var model = new UserValidatableAndEditable
            {
                FirstName = "Marie",
                LastName = "Bellin"
            };

            model.ValidateAll();

            Assert.AreEqual(false, model.HasErrors);
        }


        [TestMethod]
        public void TestValidateAll_HasErrors()
        {
            var model = new UserValidatableAndEditable
            {
                FirstName = "M",
                LastName = ""
            };

            model.ValidateAll();

            var r1 = model.GetErrors("FirstName").Cast<string>().ToList();
            var r2 = model.GetErrors("LastName").Cast<string>().ToList();

            Assert.AreEqual(true, model.HasErrors);
            Assert.AreEqual(1, r1.Count);
            Assert.AreEqual("FirstName too short", r1[0]);
            Assert.AreEqual(1, r2.Count);
            Assert.AreEqual("LastName required", r2[0]);
        }

        [TestMethod]
        public void TestValidate_IsCalledOnPropertyChanged()
        {
            var user = new UserValidatableAndEditable
            {
                ValidationType = ValidationHandling.OnPropertyChange,
                FirstName = "M",
                LastName = "Bellin"
            };

            var r1 = user.GetErrors("FirstName").Cast<string>().ToList();

            Assert.AreEqual(1, r1.Count);

            user.FirstName = "Marie";

            var r2 = user.GetErrors("FirstName");
            Assert.IsNull(r2);
        }

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
