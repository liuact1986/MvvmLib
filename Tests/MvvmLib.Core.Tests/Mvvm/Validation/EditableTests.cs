using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm.Validation
{
    [TestClass]
    public class EditableTests
    {
        [TestMethod]
        public void BeginEdit_And_Cancel()
        {
            var user = new UserEditable
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

    public class ItemEdit
    {
        public int MyValueType { get; set; }

    }

    public class UserEditable : Editable
    {
        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { this.SetProperty(ref firstName, value); }
        }

        private string lastName;
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
