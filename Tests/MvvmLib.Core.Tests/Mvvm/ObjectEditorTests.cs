using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MvvmLib.Mvvm;
using System.Collections.Generic;

namespace MvvmLib.Core.Tests.Mvvm
{

    [TestClass]
    public class ObjectEditorTests
    {
        [TestMethod]
        public void TestEditAndRestore()
        {
            var editor = new ObjectEditor();

            var user = new UserEditable
            {
                FirstName = "Marie",
                LastName = "Bellin"
            };

            Assert.AreEqual(false, editor.CanRestore);
            editor.Store(user);
            Assert.AreEqual(true, editor.CanRestore);

            user.FirstName = "updated firstname";
            user.LastName = "updated lastname";

            editor.Restore();

            Assert.AreEqual("Marie", user.FirstName);
            Assert.AreEqual("Bellin", user.LastName);
        }

        [TestMethod]
        public void Ignore_Property()
        {
            var editor = new ObjectEditor(new List<string> { "FirstName" });

            var user = new UserEditable
            {
                FirstName = "Marie",
                LastName = "Bellin"
            };

            Assert.AreEqual(false, editor.CanRestore);
            editor.Store(user);
            Assert.AreEqual(true, editor.CanRestore);

            user.FirstName = "updated firstname";
            user.LastName = "updated lastname";

            editor.Restore();

            Assert.AreEqual("updated firstname", user.FirstName);
            Assert.AreEqual("Bellin", user.LastName);
        }

        [TestMethod]
        public void Restore_With_List()
        {
            var myItem = new MyItemRestore { MyStrings = new List<string> { "A", "B" }, MyArrayOfStrings = new string[] { "Y", "Z" }, MyIStrings = new List<string> { "M", "N" } };
            var editor = new ObjectEditor();

            Assert.AreEqual(false, editor.CanRestore);
            editor.Store(myItem);
            Assert.AreEqual(true, editor.CanRestore);

            myItem.MyStrings.Add("C");
            myItem.MyIStrings.Add("O");
            myItem.MyArrayOfStrings = new string[] { "X" };

            editor.Restore();

            Assert.AreEqual(2, myItem.MyStrings.Count);
            Assert.AreEqual("A", myItem.MyStrings[0]);
            Assert.AreEqual("B", myItem.MyStrings[1]);
            Assert.AreEqual(2, myItem.MyIStrings.Count);
            Assert.AreEqual("M", myItem.MyIStrings[0]);
            Assert.AreEqual("N", myItem.MyIStrings[1]);
            Assert.AreEqual(2, myItem.MyArrayOfStrings.Length);
            Assert.AreEqual("Y", myItem.MyArrayOfStrings[0]);
            Assert.AreEqual("Z", myItem.MyArrayOfStrings[1]);
        }

        [TestMethod]
        public void Restore_Child_Object()
        {
            var editor = new ObjectEditor();

            var item = new MyItemRestoreWithChild { Child = new SubChild { MyString = "Original" } };

            Assert.AreEqual(false, editor.CanRestore);
            editor.Store(item);
            Assert.AreEqual(true, editor.CanRestore);

            item.Child.MyString = "Updated";

            editor.Restore();

            Assert.AreEqual("Original", item.Child.MyString);
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

    public class MyItemRestore
    {
        public List<string> MyStrings { get; set; }
        public IList<string> MyIStrings { get; set; }
        public string[] MyArrayOfStrings { get; set; }
    }

    public class MyItemRestoreWithChild
    {
        public SubChild Child { get; set; }
    }

    public class SubChild
    {
        public string MyString { get; set; }
    }
}
