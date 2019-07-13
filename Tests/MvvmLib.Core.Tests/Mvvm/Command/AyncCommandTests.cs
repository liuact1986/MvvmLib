using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Commands;
using System;
using System.Threading.Tasks;

namespace MvvmLib.Core.Tests.Mvvm
{
    [TestClass]
    public class AyncCommandTests
    {
        [TestMethod]
        public async Task TestCommand()
        {
            bool isCalled = false;

            var command = new AsyncCommand(async () =>
            {
                await Task.Delay(500);

                isCalled = true;
            });

            await command.ExecuteAsync();

            Assert.IsTrue(isCalled);
        }


        [TestMethod]
        public async Task TestCommand_With_Condition()
        {
            bool isCalled = false;
            bool canExecute = false;

            var command = new AsyncCommand(async () =>
            {
                await Task.Delay(500);

                isCalled = true;
            }, () => canExecute);

            await command.ExecuteAsync();
            Assert.AreEqual(false, command.CanExecute(null));
            Assert.AreEqual(false, isCalled);

            canExecute = true;

            await command.ExecuteAsync();
            Assert.AreEqual(true, command.CanExecute(null));
            Assert.AreEqual(true, isCalled);

        }

        [TestMethod]
        public async Task TestCommand_Handle_Exception()
        {
            bool isCalled = false;
            bool isExceptionCalled = false;

            var command = new AsyncCommand(async () =>
            {
                await Task.Delay(500);

                isCalled = true;

                throw new Exception("ex");

            }, (ex) => isExceptionCalled = true);

            await command.ExecuteAsync();
            Assert.AreEqual(true, command.CanExecute(null));
            Assert.AreEqual(true, isCalled);
            Assert.AreEqual(true, isExceptionCalled);
        }

        [TestMethod]
        public async Task TestCommand_Cancel()
        {
            bool isCalled = false;

            var command = new AsyncCommand(async () =>
            {
                await Task.Delay(2000);

                isCalled = true;
            });

            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync();

            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, command.IsCancellationRequested);
        }

        [TestMethod]
        public async Task TestCommand_Chains_Cancel()
        {
            bool isCalled = false;

            var command = new AsyncCommand(async () =>
            {
                await Task.Delay(2000);

                isCalled = true;
            });

            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync();
            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, command.IsCancellationRequested);

            command.CancellationTokenSource = new System.Threading.CancellationTokenSource();
            Assert.AreEqual(false, command.IsCancellationRequested);
            isCalled = false;
            await command.ExecuteAsync();
            Assert.IsTrue(isCalled);
            Assert.AreEqual(false, command.IsCancellationRequested);


            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync();
            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, command.IsCancellationRequested);
        }
    }

    [TestClass]
    public class AyncCommandGenericTests
    {
        [TestMethod]
        public async Task TestCommand()
        {
            bool isCalled = false;
            string parameter = null;

            var command = new AsyncCommand<string>(async (p) =>
            {
                await Task.Delay(500);

                isCalled = true;
                parameter = p;
            });

            await command.ExecuteAsync("A");

            Assert.IsTrue(isCalled);
            Assert.AreEqual("A", parameter);
        }


        [TestMethod]
        public async Task TestCommand_With_Condition()
        {
            bool isCalled = false;
            bool canExecute = false;
            string parameter = null;

            var command = new AsyncCommand<string>(async (p) =>
            {
                await Task.Delay(500);

                isCalled = true;
                parameter = p;
            }, (p) => canExecute);

            await command.ExecuteAsync("A");
            Assert.AreEqual(false, command.CanExecute(null));
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(null, parameter);

            canExecute = true;

            await command.ExecuteAsync("A");
            Assert.AreEqual(true, command.CanExecute(null));
            Assert.AreEqual(true, isCalled);
            Assert.AreEqual("A", parameter);

        }

        [TestMethod]
        public async Task TestCommand_Handle_Exception()
        {
            bool isCalled = false;
            bool isExceptionCalled = false;
            string parameter = null;

            var command = new AsyncCommand<string>(async (p) =>
            {
                await Task.Delay(500);

                isCalled = true;

                parameter = p;

                throw new Exception("ex");

            }, (ex) => isExceptionCalled = true);

            await command.ExecuteAsync("A");
            Assert.AreEqual(true, command.CanExecute(null));
            Assert.AreEqual(true, isCalled);
            Assert.AreEqual("A", parameter);
            Assert.AreEqual(true, isExceptionCalled);
        }

        [TestMethod]
        public async Task TestCommand_Cancel()
        {
            bool isCalled = false;
            string parameter = null;

            var command = new AsyncCommand<string>(async (p) =>
            {
                await Task.Delay(2000);

                isCalled = true;
                parameter = p;
            });

            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync("A");

            Assert.IsTrue(isCalled);
            Assert.AreEqual("A", parameter);
            Assert.AreEqual(true, command.IsCancellationRequested);
        }

        [TestMethod]
        public async Task TestCommand_Chains_Cancel()
        {
            bool isCalled = false;
            string parameter = null;

            var command = new AsyncCommand<string>(async (p) =>
            {
                await Task.Delay(2000);

                isCalled = true;
                parameter = p;
            });

            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync("A");
            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, command.IsCancellationRequested);

            command.CancellationTokenSource = new System.Threading.CancellationTokenSource();
            Assert.AreEqual(false, command.IsCancellationRequested);
            isCalled = false;
            await command.ExecuteAsync("B");
            Assert.IsTrue(isCalled);
            Assert.AreEqual(false, command.IsCancellationRequested);


            command.CancellationTokenSource.CancelAfter(500);
            await command.ExecuteAsync("C");
            Assert.IsTrue(isCalled);
            Assert.AreEqual(true, command.IsCancellationRequested);
        }
    }
}
