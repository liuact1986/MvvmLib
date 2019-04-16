using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmLib.Core.Tests.Message
{
    [TestClass]
    public class PublisherEventTests
    {
        [TestMethod]
        public void Subscriber_is_notified()
        {
            bool isNotifed = false;
            var sub = new Action(() =>
            {
                isNotifed = true;
            });

            var p = new MyEmptyEvent();
            var o = p.Subscribe(sub);

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.Token);

            p.Publish();

            Assert.IsTrue(isNotifed);
        }

        [TestMethod]
        public void Publish_On_UIThread()
        {
            bool isNotifed = false;
            var sub = new Action(() =>
            {
                isNotifed = true;
            });

            var p = new MyEmptyEvent();

            var s = new TestableSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(s);
            p.SynchronizationContext = s;

            p.Subscribe(sub).WithExecutionStrategy(ExecutionStrategyType.UIThread);

            p.Publish();

            Assert.IsTrue(isNotifed);
        }

        [TestMethod]
        public async Task Publish_On_BackgroundThread()
        {
            bool isNotifed = false;
            var sub = new Action(() =>
            {
                isNotifed = true;
            });

            var p = new MyEmptyEvent();
            p.Subscribe(sub).WithExecutionStrategy(ExecutionStrategyType.BackgroundThread);

            p.Publish();

            await Task.Delay(1);

            Assert.IsTrue(isNotifed);
        }

        [TestMethod]
        public void Subscribers_are_notified()
        {
            int i = 0;
            var sub = new Action(() =>
            {
                i++;
            });

            var sub2 = new Action(() =>
            {
                i++;
            });

            var sub3 = new Action(() =>
            {
                i++;
            });

            var p = new MyEmptyEvent();
            p.Subscribe(sub);
            p.Subscribe(sub2);
            p.Subscribe(sub3);

            p.Publish();

            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void UnSubscribe()
        {
            bool isNotifed = false;
            var sub = new Action(() =>
            {
                isNotifed = true;
            });

            var p = new MyEmptyEvent();
            var o = p.Subscribe(sub);

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.Token);

            p.Publish();

            Assert.IsTrue(isNotifed);

            isNotifed = false;

            Assert.IsTrue(p.Unsubscribe(o.Token));

            p.Publish();

            Assert.IsFalse(isNotifed);
        }

        [TestMethod]
        public void Subscriber_is_notified_With_Generic_Event()
        {
            bool isNotifed = false;
            string r = null;
            var sub = new Action<string>((param) =>
            {
                isNotifed = true;
                r = param;
            });

            var p = new MyGenericEvent();
            var o = p.Subscribe(sub);

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.Token);

            p.Publish("a");

            Assert.IsTrue(isNotifed);
            Assert.AreEqual("a", r);
        }

        [TestMethod]
        public void Filter_With_Generic_Event()
        {
            bool isNotifed = false;
            string r = null;
            var sub = new Action<string>((param) =>
            {
                isNotifed = true;
                r = param;
            });

            var p = new MyGenericEvent();
            var o = p.Subscribe(sub).WithFilter((a) => a != "a");

            p.Publish("a");

            Assert.IsFalse(isNotifed);
            Assert.AreEqual(null, r);

            p.Publish("b");

            Assert.IsTrue(isNotifed);
            Assert.AreEqual("b", r);
        }

        [TestMethod]
        public void Publish_On_UIThread_With_Generic_Event()
        {
            bool isNotifed = false;
            string r = null;
            var sub = new Action<string>(_ =>
            {
                isNotifed = true;
                r = _;
            });

            var p = new MyGenericEvent();

            var s = new TestableSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(s);
            p.SynchronizationContext = s;

            p.Subscribe(sub).WithExecutionStrategy(ExecutionStrategyType.UIThread);

            p.Publish("a");

            Assert.IsTrue(isNotifed);
            Assert.AreEqual("a", r);
        }

        [TestMethod]
        public async Task Publish_On_BackgroundThread_With_Generic_Event()
        {
            bool isNotifed = false;
            string r = null;
            var sub = new Action<string>(_ =>
            {
                isNotifed = true;
                r = _;
            });

            var p = new MyGenericEvent();
            p.Subscribe(sub).WithExecutionStrategy(ExecutionStrategyType.BackgroundThread);

            p.Publish("a");

            await Task.Delay(1);

            Assert.IsTrue(isNotifed);
            Assert.AreEqual("a", r);
        }

        [TestMethod]
        public void Subscribers_are_notified_With_Generic_Event()
        {
            int i = 0;
            string r = null, r2 = null, r3 = null;
            var sub = new Action<string>((param) =>
            {
                i++;
                r = param;
            });

            var sub2 = new Action<string>((param) =>
            {
                i++;
                r2 = param;
            });

            var sub3 = new Action<string>((param) =>
            {
                i++;
                r3 = param;
            });

            var p = new MyGenericEvent();
            p.Subscribe(sub);
            p.Subscribe(sub2);
            p.Subscribe(sub3);

            p.Publish("a");

            Assert.AreEqual(3, i);
            Assert.AreEqual("a", r);
            Assert.AreEqual("a", r2);
            Assert.AreEqual("a", r3);
        }

        [TestMethod]
        public void UnSubscribe_With_Generic_Event()
        {
            bool isNotifed = false;
            string r = null;
            var sub = new Action<string>(_ =>
            {
                isNotifed = true;
                r = _;
            });

            var p = new MyGenericEvent();
            var o = p.Subscribe(sub);

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.Token);

            p.Publish("a");

            Assert.IsTrue(isNotifed);
            Assert.AreEqual("a", r);

            isNotifed = false;

            Assert.IsTrue(p.Unsubscribe(o.Token));

            p.Publish("b");

            Assert.IsFalse(isNotifed);
            Assert.AreEqual("a", r);
        }
    }

    public class MyGenericEvent : ParameterizedEvent<string>
    {

    }

    public class MyGenericEvent2 : ParameterizedEvent<string>
    {

    }

    public class MyEmptyEvent : EmptyEvent
    {

    }

    public class MyEmptyEvent2 : EmptyEvent
    {

    }

    public sealed class TestableSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            d(state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }
    }

}
