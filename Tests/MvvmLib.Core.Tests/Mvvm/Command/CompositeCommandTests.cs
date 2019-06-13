using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;
using MvvmLib.Mvvm;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class CompositeCommandTests
    {
        [TestMethod]
        public void TestComposite_ExecuteAllCommands()
        {
            bool command1Called = false;
            bool command2Called = false;

            var composite = new CompositeCommand();

            composite.Add(new RelayCommand(() =>
            {
                command1Called = true;
            }));
            composite.Add(new RelayCommand(() =>
            {
                command2Called = true;
            }));

            composite.Execute(null);

            Assert.IsTrue(command1Called);
            Assert.IsTrue(command2Called);
        }

        [TestMethod]
        public void TestComposite_WithParameter_ExecuteAllCommands()
        {
            bool command1Called = false;
            bool command2Called = false;
            string command1Result = "";

            var composite = new CompositeCommand();

            composite.Add(new RelayCommand<string>((value) =>
            {
                command1Result = value;
                command1Called = true;
            }));
            composite.Add(new RelayCommand(() =>
            {
                command2Called = true;
            }));

            composite.Execute("Ok");

            Assert.IsTrue(command1Called);
            Assert.IsTrue(command2Called);
            Assert.AreEqual("Ok", command1Result);
        }

        [TestMethod]
        public void TestComposite_PassParameter()
        {
            bool command1Called = false;
            string command1Result = "";
            string checkValue = "";

            var composite = new CompositeCommand();

            composite.Add(new RelayCommand<string>((value) =>
            {
                command1Result = value;
                command1Called = true;
            },
            (value)=>
            {
                checkValue = value;
                return true;
            }));

            if (composite.CanExecute("Ok"))
            {
                composite.Execute("Ok");
            }

            Assert.IsTrue(command1Called);
            Assert.AreEqual("Ok", command1Result);
            Assert.AreEqual("Ok", checkValue);
        }

        [TestMethod]
        public void TestComposite_Check_ReturnsFalse()
        {
            bool command1Called = false;
            bool command2Called = false;
            bool isCheck1 = false;
            bool isCheck2 = false;

            var composite = new CompositeCommand();

            composite.Add(new RelayCommand(() =>
            {
                command1Called = true;
            }, () =>
             {
                 isCheck1 = true;
                 return false;
             }));
            composite.Add(new RelayCommand(() =>
            {
                command2Called = true;
            },()=>
            {
                isCheck2 = true;
                return false;
            }));

            if (composite.CanExecute(null))
            {
                composite.Execute(null);
            }

            Assert.IsFalse(command1Called);
            Assert.IsFalse(command2Called);
            Assert.IsTrue(isCheck1);
            Assert.IsFalse(isCheck2);
        }

        [TestMethod]
        public void TestComposite_Check_ReturnsTrue()
        {
            bool command1Called = false;
            bool command2Called = false;
            bool isCheck1 = false;
            bool isCheck2 = false;

            var composite = new CompositeCommand();

            composite.Add(new RelayCommand(() =>
            {
                command1Called = true;
            }, () =>
            {
                isCheck1 = true;
                return true;
            }));
            composite.Add(new RelayCommand(() =>
            {
                command2Called = true;
            }, () =>
            {
                isCheck2 = true;
                return true;
            }));

            if (composite.CanExecute(null))
            {
                composite.Execute(null);
            }

            Assert.IsTrue(command1Called);
            Assert.IsTrue(command2Called);
            Assert.IsTrue(isCheck1);
            Assert.IsTrue(isCheck2);
        }
    }
}
