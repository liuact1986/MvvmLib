﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;

namespace MvvmLib.Core.Tests.Commands
{

    [TestClass]
    public class DelegateCommandTests
    {
        [TestMethod]
        public void TestDelegateCommand()
        {
            bool isCalled = false;

            var command = new DelegateCommand(() =>
            {
                isCalled = true;
            });

            command.Execute(null);

            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void WithCondition_IsNot_Executed()
        {
            bool isCalled = false;
            bool isChecked = false;

            var command = new DelegateCommand(() =>
            {
                isCalled = true;
            }, () =>
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
        public void WithCondition_Is_Executed()
        {
            bool isCalled = false;
            bool isChecked = false;

            var command = new DelegateCommand(() =>
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

    }

    [TestClass]
    public class DelegateCommandGenericTests
    {
        [TestMethod]
        public void TestDelegateCommand()
        {
            bool isCalled = false;
            string result = "";

            var command = new DelegateCommand<string>((value) =>
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
        public void WithCondition_IsNot_Executed()
        {
            bool isCalled = false;
            bool isChecked = false;
            string result = "";
            string checkresult = "";

            var command = new DelegateCommand<string>((value) =>
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
        public void WithCondition_Is_Executed()
        {
            bool isCalled = false;
            bool isChecked = false;
            string result = "";
            string checkresult = "";

            var command = new DelegateCommand<string>((value) =>
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
