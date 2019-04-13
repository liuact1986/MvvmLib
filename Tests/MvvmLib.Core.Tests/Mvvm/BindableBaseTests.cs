using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MvvmLib;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm
{
    public class MyModel : BindableBase
    {
        public bool valueHasChanged = false;

        private string _firstName;
        public string FirstName
        {
            get { return this._firstName; }
            set
            {
                valueHasChanged = this.SetProperty(ref this._firstName, value); // set value and raise if value not equals old value
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return this._lastName; }
            set
            {
                this._lastName = value;
                this.RaisePropertyChanged(); // raise current property
                this.RaisePropertyChanged("FullName"); // raise other property
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged(() => Email); // expression
            }
        }

        public string FullName => $"t{FirstName} {LastName}";
    }

    [TestClass]
    public class BindableBaseTests
    {
        [TestMethod]
        public void TestSet()
        {
            string property = "";

            var model = new MyModel();
            model.PropertyChanged += (sender, e) =>
            {
                property = e.PropertyName;
            };

            model.FirstName = "new value";

            Assert.AreEqual("FirstName", property);
            Assert.AreEqual("new value", model.FirstName);
        }

        [TestMethod]
        public void Do_Not_Change_with_same_value()
        {

            var model = new MyModel();
            model.FirstName = "V1";
            Assert.IsTrue(model.valueHasChanged);


            model.valueHasChanged = false;
            model.FirstName = "V1";
            Assert.IsFalse(model.valueHasChanged);

            model.FirstName = "V2";
            Assert.IsTrue(model.valueHasChanged);
        }

        [TestMethod]
        public void TestRaise()
        {
            var properties = new List<string>();

            var model = new MyModel();
            model.PropertyChanged += (sender, e) =>
            {
                properties.Add(e.PropertyName);
            };

            model.LastName = "new value";

            Assert.AreEqual(2, properties.Count);
            Assert.IsTrue(properties.Contains("LastName"));
            Assert.IsTrue(properties.Contains("FullName"));
        }

        [TestMethod]
        public void TestRaise_With_Expression()
        {
            string property = "";

            var model = new MyModel();
            model.PropertyChanged += (sender, e) =>
            {
                property = e.PropertyName;
            };

            model.Email = "name@mail.com";

            Assert.AreEqual("Email", property);
            Assert.AreEqual("name@mail.com", model.Email);
        }
    }
}
