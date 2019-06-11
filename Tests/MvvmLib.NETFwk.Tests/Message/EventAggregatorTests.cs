using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;
using System;
using System.Threading.Tasks;

namespace MvvmLib.Core.Tests.Message
{
    [TestClass]
    public class EventAggregatorTests
    {
        [TestMethod]
        public void Get_Or_Add_New_Event()
        {
            var ea = new EventAggregator();

            var e1 = ea.GetEvent<FakeEvent>();
            e1.Id = 1;

            var e2 = ea.GetEvent<FakeEvent>();

            Assert.AreEqual(e1, e2);
            Assert.AreEqual(1, e2.Id);
        }

        //[TestMethod]
        //public void Sets_The_SynchronizationContext()
        //{
        //    var ea = new EventAggregator();
        //    var e1 = ea.GetEvent<FakeEvent>();

        //    Assert.IsNotNull(e1.SynchronizationContext);
        //}

        [TestMethod]
        public void Get_Or_Add_New_Generic_Event()
        {
            var ea = new EventAggregator();

            var e1 = ea.GetEvent<FakeGenericEvent>();
            e1.Id = 1;

            var e2 = ea.GetEvent<FakeGenericEvent>();

            Assert.AreEqual(e1, e2);
            Assert.AreEqual(1, e2.Id);
        }

        //[TestMethod]
        //public void Sets_The_SynchronizationContext_With_Generic_Event()
        //{
        //    var ea = new EventAggregator();
        //    var e1 = ea.GetEvent<FakeGenericEvent>();

        //    Assert.IsNotNull(e1.SynchronizationContext);
        //}

        [TestMethod]
        public void Notify_Subscriber()
        {
            bool isNotified = false;
            var ea = new EventAggregator();
            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                isNotified = true;
            });

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(true, isNotified);
        }


        [TestMethod]
        public void Not_KeepAlive_Subscriber()
        {
            MySub.IsNotified = false;

            var ea = new EventAggregator();

            var sub = new MySub();

            ea.GetEvent<MyEmptyEvent>().Subscribe(sub.Execute, false);

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(true, MySub.IsNotified);

            MySub.IsNotified = false;
            sub = null;
            GC.Collect();

            ea.GetEvent<MyEmptyEvent>().Publish();
            Assert.AreEqual(false, MySub.IsNotified);
        }

        [TestMethod]
        public void KeepAlive_Subscriber()
        {
            MySub.IsNotified = false;

            var ea = new EventAggregator();

            var sub = new MySub();

            ea.GetEvent<MyEmptyEvent>().Subscribe(sub.Execute, true);

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(true, MySub.IsNotified);

            MySub.IsNotified = false;
            sub = null;
            GC.Collect();

            ea.GetEvent<MyEmptyEvent>().Publish();
            Assert.AreEqual(true, MySub.IsNotified);
        }

        [TestMethod]
        public void Not_KeepAlive_Parameterized_Subscriber()
        {
            MySubWithParameterString.IsNotified = false;
            MySubWithParameterString.MyString = null;

            var ea = new EventAggregator();

            var sub = new MySubWithParameterString();

            ea.GetEvent<MyGenericEvent>().Subscribe(sub.Execute, false);

            ea.GetEvent<MyGenericEvent>().Publish("A");

            Assert.AreEqual(true, MySubWithParameterString.IsNotified);
            Assert.AreEqual("A", MySubWithParameterString.MyString);

            MySubWithParameterString.IsNotified = false;
            MySubWithParameterString.MyString = null;

            sub = null;
            GC.Collect();

            ea.GetEvent<MyGenericEvent>().Publish("B");

            Assert.AreEqual(false, MySubWithParameterString.IsNotified);
            Assert.AreEqual(null, MySubWithParameterString.MyString);
        }

        [TestMethod]
        public void KeepAlive_Parameterized_Subscriber()
        {
            MySubWithParameterString.IsNotified = false;
            MySubWithParameterString.MyString = null;

            var ea = new EventAggregator();

            var sub = new MySubWithParameterString();

            ea.GetEvent<MyGenericEvent>().Subscribe(sub.Execute, true);

            ea.GetEvent<MyGenericEvent>().Publish("A");

            Assert.AreEqual(true, MySubWithParameterString.IsNotified);
            Assert.AreEqual("A", MySubWithParameterString.MyString);

            MySubWithParameterString.IsNotified = false;
            MySubWithParameterString.MyString = null;

            sub = null;
            GC.Collect();

            ea.GetEvent<MyGenericEvent>().Publish("B");

            Assert.AreEqual(true, MySubWithParameterString.IsNotified);
            Assert.AreEqual("B", MySubWithParameterString.MyString);
        }


        [TestMethod]
        public void Notify_Subscriber_On_UIThread()
        {
            bool isNotified = false;
            var ea = new EventAggregator
            {
                SynchronizationContext = new TestableSynchronizationContext()
            };
            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                isNotified = true;
            }).WithExecutionStrategy(ExecutionStrategyType.UIThread);

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(true, isNotified);
        }

        [TestMethod]
        public async Task Notify_Subscriber_On_BackgroundThread()
        {
            bool isNotified = false;
            var ea = new EventAggregator();
            ea.GetEvent<MyEmptyEvent>().Subscribe(()=>
            {
                isNotified = true;
            }).WithExecutionStrategy(ExecutionStrategyType.BackgroundThread);

            ea.GetEvent<MyEmptyEvent>().Publish();

            await Task.Delay(1);

            Assert.AreEqual(true, isNotified);
        }

        [TestMethod]
        public void Notify_All_Subscribers()
        {
            int i = 0;
            var ea = new EventAggregator();
            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                i++;
            });

            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                i++;
            });

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void Notify_Only_Subscribers_Of_Event_Type()
        {
            int i = 0;
            var ea = new EventAggregator();
            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                i++;
            });

            ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                i++;
            });

            ea.GetEvent<MyEmptyEvent2>().Subscribe(() =>
            {
                i++;
            });

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(2, i);

            i = 0;

            ea.GetEvent<MyEmptyEvent2>().Publish();
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void Unsubscribe()
        {
            bool isNotified = false;
            var ea = new EventAggregator();
            var options = ea.GetEvent<MyEmptyEvent>().Subscribe(() =>
            {
                isNotified = true;
            });

            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(true, isNotified);

            Assert.IsTrue(ea.GetEvent<MyEmptyEvent>().Unsubscribe(options.Token));

            isNotified = false;
            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(false, isNotified);
        }

        [TestMethod]
        public void Notify_Subscriber_With_Generic_Event()
        {
            bool isNotified = false;
            string r = null;
            var ea = new EventAggregator();
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                isNotified = true;
                r = _;
            });

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(true, isNotified);
            Assert.AreEqual("a", r);
        }

        [TestMethod]
        public void Filter_Subscriber_With_Generic_Event()
        {
            bool isNotified = false;
            string r = null;
            var ea = new EventAggregator();
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                isNotified = true;
                r = _;
            }).WithFilter(_=> _ != "a");

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(false, isNotified);
            Assert.AreEqual(null, r);

            ea.GetEvent<MyGenericEvent>().Publish("b");

            Assert.AreEqual(true, isNotified);
            Assert.AreEqual("b", r);
        }

        [TestMethod]
        public void Notify_Subscriber_On_UIThread_With_Generic_Event()
        {
            bool isNotified = false;
            string r = null;
            var ea = new EventAggregator
            {
                SynchronizationContext = new TestableSynchronizationContext()
            };
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                isNotified = true;
                r = _;
            }).WithExecutionStrategy(ExecutionStrategyType.UIThread);

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(true, isNotified);
            Assert.AreEqual("a", r);
        }


        [TestMethod]
        public async Task Notify_Subscriber_On_BackgroundThread_With_Generic_Event()
        {
            bool isNotified = false;
            string r = null;
            var ea = new EventAggregator();
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                isNotified = true;
                r = _;
            }).WithExecutionStrategy(ExecutionStrategyType.BackgroundThread);

            ea.GetEvent<MyGenericEvent>().Publish("a");

            await Task.Delay(1);

            Assert.AreEqual(true, isNotified);
            Assert.AreEqual("a", r);
        }

        [TestMethod]
        public void Notify_All_Subscribers_With_Generic_Event()
        {
            int i = 0;
            string r = null, r2 = null;
            var ea = new EventAggregator();
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                i++;
                r = _;
            });

            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                i++;
                r2 = _;
            });

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(2, i);
            Assert.AreEqual("a", r);
            Assert.AreEqual("a", r2);
        }

        [TestMethod]
        public void Notify_Only_Subscribers_Of_Event_Type_With_Generic_Event()
        {
            int i = 0;
            string r = null, r2 = null, r3 = null;
            var ea = new EventAggregator();
            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                i++;
                r = _;
            });

            ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                i++;
                r2 = _;
            });

            ea.GetEvent<MyGenericEvent2>().Subscribe(_ =>
            {
                i++;
                r3 = _;
            });

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(2, i);
            Assert.AreEqual("a", r);
            Assert.AreEqual("a", r2);
            Assert.AreEqual(null, r3);

            i = 0;
            r = null;
            r2 = null;

            ea.GetEvent<MyGenericEvent2>().Publish("b");
            Assert.AreEqual(1, i);
            Assert.AreEqual(null, r);
            Assert.AreEqual(null, r2);
            Assert.AreEqual("b", r3);
        }

        [TestMethod]
        public void Unsubscribe_With_Generic_Event()
        {
            bool isNotified = false;
            string r = null;
            var ea = new EventAggregator();
            var options =  ea.GetEvent<MyGenericEvent>().Subscribe(_ =>
            {
                isNotified = true;
                r = _;
            });

            ea.GetEvent<MyGenericEvent>().Publish("a");

            Assert.AreEqual(true, isNotified);
            Assert.AreEqual("a", r);

            Assert.IsTrue(ea.GetEvent<MyGenericEvent>().Unsubscribe(options.Token));

            isNotified = false;
            ea.GetEvent<MyEmptyEvent>().Publish();

            Assert.AreEqual(false, isNotified);
            Assert.AreEqual("a", r);
        }

    }

    public class MySub
    {
        public static bool IsNotified { get; set; }

        public void Execute()
        {
            IsNotified = true;
        }
    
    }

    public class MySubWithParameterString
    {
        public static bool IsNotified { get; set; }
        public static string MyString { get; set; }

        public void Execute(string v)
        {
            IsNotified = true;
            MyString = v;
        }
    }

    public class FakeEvent : EmptyEvent
    {
        public int Id { get; internal set; }
    }

    public class FakeGenericEvent : ParameterizedEvent<string>
    {
        public int Id { get; internal set; }
    }
}
