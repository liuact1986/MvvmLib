using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{

    [TestClass]
    public class EditableObjectServiceTests
    {
        [TestMethod]
        public void TestEditAndRestore()
        {
            var service = new EditableObjectService();

            var user = new UserEditable
            {
                FirstName = "Marie",
                LastName = "Bellin"
            };

            service.Store(user);

            user.FirstName = "updated firstname";
            user.LastName = "updated lastname";

            service.Restore(user);

            Assert.AreEqual("Marie",user.FirstName);
            Assert.AreEqual("Bellin", user.LastName);
        }
    }

    public class UserValidatableAndEditable : Validatable
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
