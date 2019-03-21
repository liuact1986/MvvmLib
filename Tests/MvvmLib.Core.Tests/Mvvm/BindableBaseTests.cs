using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MvvmLib;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm
{
    public class Observed : BindableBase
    {
        private string _firstName;
        public string FirstName
        {
            get
            {
                return this._firstName;
            }
            set
            {
                this.SetProperty(ref this._firstName, value);
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return this._lastName;
            }
            set
            {
                this._lastName = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("FullName");
            }
        }

        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }
    }

    [TestClass]
    public class BindableBaseTests
    {
        [TestMethod]
        public void TestSet()
        {
            string property = "";

            var obs = new Observed();
            obs.PropertyChanged += (sender,e) =>
            {
                property = e.PropertyName;
            };

            obs.FirstName = "new value";

            Assert.AreEqual("FirstName", property);
            Assert.AreEqual("new value", obs.FirstName);
        }

        [TestMethod]
        public void TestRaise()
        {
            var properties = new List<string>();

            var obs = new Observed();
            obs.PropertyChanged += (sender, e) =>
            {
                properties.Add(e.PropertyName);
            };

            obs.LastName = "new value";

            Assert.AreEqual(2, properties.Count);
            Assert.IsTrue(properties.Contains("LastName"));
            Assert.IsTrue(properties.Contains("FullName"));
        }
    }
}
