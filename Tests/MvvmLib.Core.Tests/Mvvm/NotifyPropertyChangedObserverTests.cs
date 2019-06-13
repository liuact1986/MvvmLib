using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class NotifyPropertyChangedObserverTests
    {

        [TestMethod]
        public void IsNotified_On_Value_Changed()
        {
            bool isNotified = false; ;

            var u = new ObservedA();

            var o = new NotifyPropertyChangedObserver(u);
            o.SubscribeToPropertyChanged((p) =>
            {
                isNotified = true;
            });

            u.FirstName = "Marie";
            Assert.AreEqual(true, isNotified);

            isNotified = false;
            u.LastName = "Bellin";
            Assert.AreEqual(true, isNotified);
        }

        [TestMethod]
        public void Filter()
        {
            bool isNotified = false; ;

            var u = new ObservedA();

            var o = new FilterableNotifyPropertyChangedObserver(u, (s, p) =>
            {
                return p.PropertyName == "FirstName";
            });
            o.SubscribeToPropertyChanged((p) =>
             {
                 isNotified = true;
             });

            u.FirstName = "Marie";
            Assert.AreEqual(true, isNotified);

            isNotified = false;
            u.LastName = "Bellin";
            Assert.AreEqual(false, isNotified);
        }
    }


    public class ObservedA : BindableBase, IDisposable
    {
        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        public void Dispose()
        {

        }
    }
}
