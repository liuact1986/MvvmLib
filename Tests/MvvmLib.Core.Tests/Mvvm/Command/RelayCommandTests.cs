using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void TestRelayCommand()
        {
            bool isCalled = false;

            var command = new RelayCommand(() =>
            {
                isCalled = true;
            });

            command.Execute(null);

            Assert.IsTrue(isCalled);

        }

        [TestMethod]
        public void TestRelayCommand_WithCondition_DontExecute()
        {
            bool isCalled = false;
            bool isChecked = false;

            var command = new RelayCommand(() =>
            {
                isCalled = true;
            },()=>
            {
                isChecked = true;
                return false;
            });

            if (command.CanExecute(null))
            {
                command.Execute(null);
            }

            Assert.IsTrue(isChecked);
            Assert.IsFalse(isCalled);
        }

        [TestMethod]
        public void TestRelayCommand_WithCondition_Execute()
        {
            bool isCalled = false;
            bool isChecked = false;

            var command = new RelayCommand(() =>
            {
                isCalled = true;
            }, () =>
            {
                isChecked = true;
                return true;
            });

            if (command.CanExecute(null))
            {
                command.Execute(null);
            }

            Assert.IsTrue(isChecked);
            Assert.IsTrue(isCalled);

        }


        [TestMethod]
        public void TestRelayCommand_WithParameter()
        {
            bool isCalled = false;
            string result = "";

            var command = new RelayCommand<string>((value) =>
            {
                isCalled = true;
                result = value;
            });

            if (command.CanExecute("Ok"))
            {
                command.Execute("Ok");
            }

            Assert.IsTrue(isCalled);
            Assert.AreEqual("Ok", result);
        }

        [TestMethod]
        public void TestRelayCommand_WithParameterAndCondition_DontExecute()
        {
            bool isCalled = false;
            bool isChecked = false;
            string result = "";
            string checkresult = "";

            var command = new RelayCommand<string>((value) =>
            {
                isCalled = true;
                result = value;
            }, (value) =>
            {
                isChecked = true;
                checkresult = value;
                return false;
            });

            if (command.CanExecute("Ok"))
            {
                command.Execute("Ok");
            }

            Assert.IsTrue(isChecked);
            Assert.IsFalse(isCalled);
            Assert.AreEqual("", result);
            Assert.AreEqual("Ok", checkresult);

        }

        [TestMethod]
        public void TestRelayCommand_WithParameterAndCondition_Execute()
        {
            bool isCalled = false;
            bool isChecked = false;
            string result = "";
            string checkresult = "";

            var command = new RelayCommand<string>((value) =>
            {
                isCalled = true;
                result = value;
            }, (value) =>
            {
                isChecked = true;
                checkresult = value;
                return true;
            });

            if (command.CanExecute("Ok"))
            {
                command.Execute("Ok");
            }

            Assert.IsTrue(isChecked);
            Assert.IsTrue(isCalled);
            Assert.AreEqual("Ok", result);
            Assert.AreEqual("Ok", checkresult);

        }

 
    }
}
