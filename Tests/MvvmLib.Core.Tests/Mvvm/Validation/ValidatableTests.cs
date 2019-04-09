using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MvvmLib;
using MvvmLib.Mvvm;
using System.Linq;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{


    [TestClass]
    public class ValidatableTests
    {

        [TestMethod]
        public void TestCanValidateOnChange_WithOnChange()
        {

            var model = new UserValidatable
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

            var model = new UserValidatable
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

            var model = new UserValidatable
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

            var user = new UserValidatable
            {
                FirstName ="Marie",
                LastName = "Bellin"
            };

            user.ValidateProperty("FirstName", user.FirstName);
            var errors = user.GetErrors("FirstName");
            Assert.IsFalse(user.HasErrors);
            Assert.IsNull(errors);
        }

        [TestMethod]
        public void TestHasError()
        {
            var user = new UserValidatable
            {
                FirstName = "M",
                LastName = "Bellin"
            };

            user.ValidateProperty("FirstName", user.FirstName);

            var errors = user.GetErrors("FirstName");
            var r = errors.Cast<string>().ToList();

            Assert.IsTrue(user.HasErrors);
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("FirstName too short", r[0]);
        }

        [TestMethod]
        public void TestValidateAll()
        {
            var model = new UserValidatable
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
            var model = new UserValidatable
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
            var user = new UserValidatable
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
    }

    public class UserValidatable : Validatable
    {
        private string firstName;

        [Required(ErrorMessage = "FirstName required")]
        [MinLength(2, ErrorMessage = "FirstName too short")]
        public string FirstName
        {
            get { return firstName; }
            set { this.SetProperty(ref firstName, value); }
        }

        private string lastName;
        [Required(ErrorMessage = "LastName required")]
        public string LastName
        {
            get { return lastName; }
            set { this.SetProperty(ref lastName, value); }
        }

        private int? age;
        public int? Age
        {
            get { return age; }
            set { this.SetProperty(ref age, value); }
        }
    }

}
