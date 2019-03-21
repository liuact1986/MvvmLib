using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;

namespace MvvmLib.Core.Tests.Message
{
    public class MyUserWithDelegate
    {
        public Action Action { get; set; }

        public MyUserWithDelegate()
        {
            Action = () =>
            {

            };
        }
    }

    [TestClass]
    public class WeakDelegateTest
    {

        [TestMethod]
        public void TestResult_WithNoParameterAndResult_ReturnsFalse()
        {

            var user = new MyUserWithDelegate();

            var service = new WeakDelegate(user.Action);

            Assert.IsTrue(service.IsAlive);

            user = null;

            service.Kill();

            GC.Collect();

            Assert.IsFalse(service.IsAlive);
        }

        //// check parameter type

        //[TestMethod]
        //public void TestResult_WithNoParameterAndResult_ReturnsFalse()
        //{
        //    var action = new Action(() =>
        //    {
        //    });

        //    var weakSubscriber = new WeakDelegate(action);
        //    var result = weakSubscriber.CheckResult("Ok");
        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public void TestResult_WithNoParameterAndNoResult_ReturnsTrue()
        //{
        //    var action = new Action(() =>
        //    {
        //    });

        //    var weakSubscriber = new WeakDelegate(action);
        //    var result = weakSubscriber.CheckResult();
        //    Assert.IsTrue(result);
        //}


        //[TestMethod]
        //public void TestResult_WithParameterAndNoResult_ReturnsFalse()
        //{
        //    var action = new Action<string>((value) =>
        //    {
        //    });

        //    var weakSubscriber = new WeakDelegate(action, typeof(string));
        //    var result = weakSubscriber.CheckResult();
        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public void TestResult_WithParameterAndInvalidResultType_ReturnsFalse()
        //{
        //    var action = new Action<string>((value) =>
        //    {
        //    });

        //    var weakSubscriber = new WeakDelegate(action, typeof(string));
        //    var result = weakSubscriber.CheckResult(10);
        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public void TestResult_WithParameterAndValidResult_ReturnsTrue()
        //{
        //    var action = new Action<string>((value) =>
        //    {
        //    });

        //    var weakSubscriber = new WeakDelegate(action, typeof(string));
        //    var result = weakSubscriber.CheckResult("Ok");
        //    Assert.IsTrue(result);
        //}

        //// invoke

        //[TestMethod]
        //public void TestInvoke()
        //{
        //    bool success = false;
        //    var action = new Action(() =>
        //    {
        //        success = true;
        //    });

        //    var weakSubscriber = new WeakDelegate(action);
        //    weakSubscriber.Invoke();
        //    Assert.IsTrue(success);
        //}

        //[TestMethod]
        //public void TestInvoke_WithActionAndParameter()
        //{
        //    bool success = false;
        //    var result = "";
        //    var action = new Action<string>((value) =>
        //    {
        //        success = true;
        //        result = value;
        //    });

        //    var weakSubscriber = new WeakDelegate(action, typeof(string));
        //    weakSubscriber.Invoke("Ok");
        //    Assert.IsTrue(success);
        //    Assert.AreEqual("Ok",result);
        //}

        //[TestMethod]
        //public void TestInvoke_WithActionAndParameterObject()
        //{
        //    bool success = false;
        //    Result result = null;
        //    var action = new Action<Result>((value) =>
        //    {
        //        success = true;
        //        result = value;
        //    });

        //    var weakSubscriber = new WeakDelegate(action, typeof(Result));
        //    weakSubscriber.Invoke(new Result("Ok"));
        //    Assert.IsTrue(success);
        //    Assert.AreEqual("Ok", result.result);
        //}

        //[TestMethod]
        //public void TestInvoke_WithInstanceAndParameter()
        //{
        //    var sub = new Sub1();

        //    var weakSubscriber = new WeakDelegate(new Action<string>(sub.GetResult), typeof(string));
        //    weakSubscriber.Invoke("Ok");
        //    Assert.AreEqual("Ok", sub.result);
        //}

        //// set dead

        //[TestMethod]
        //public void TestSetDead()
        //{
        //    var action = new Action(() =>
        //    {

        //    });

        //    var weakSubscriber = new WeakDelegate(action);
        //    Assert.IsTrue(weakSubscriber.IsAlive);

        //    weakSubscriber.Kill();

        //    Assert.IsFalse(weakSubscriber.IsAlive);

        //}

        //[TestMethod]
        //public void Test_Alive()
        //{
        //    var sub = new Sub1();

        //    var weakSubscriber = new WeakSubscriber(new Action<string>(sub.GetResult), typeof(string));

        //    sub = null;

        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.WaitForFullGCComplete();
        //    GC.Collect();

        //    // weakSubscriber.SetDead();

        //    var result = weakSubscriber.IsAlive();

        //    Assert.AreEqual(false, result);
        //}

        //[TestMethod]
        //public void Test_Alive()
        //{
        //    var a = new Action(() =>
        //    {

        //    });

        //    var v = new WeakReference((Delegate)a);

        //    Assert.AreEqual(true, v.IsAlive);

        //    a = null;

        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.WaitForFullGCComplete();
        //    GC.Collect();

        //    Assert.AreEqual(false, v.IsAlive);
        //}

    }

}


