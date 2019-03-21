using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;

namespace MvvmLib.Core.Tests.Message
{
    [TestClass]
    public class MessengerTests
    {

        public Messenger GetService()
        {
            return new Messenger();
        }

        // get event

        [TestMethod]
        public void GetEvent_CreateAnEvent()
        {
            var service = GetService();

            service.GetEvent<MyEvent>();

            Assert.IsTrue(service.IsEventRegistered<MyEvent>());
        }

        [TestMethod]
        public void GetEvent_ReturnCreatedEvent()
        {
            var service = GetService();

            var sub1 = new Action(() => { });
            var sub2 = new Action(() => { });

            service.GetEvent<MyEvent>().Subscribe(sub1);
            service.GetEvent<MyEvent>().Subscribe(sub2);

            var _event = service.GetEvent<MyEvent>();

            Assert.AreEqual(2, _event.SubcribersCount);
        }

        [TestMethod]
        public void PubSub_WithEmptyEvent()
        {
            var service = GetService();

            bool isNotified = false;

            service.GetEvent<MyEvent>().Subscribe(() =>
            {
                isNotified = true;
            });

            service.GetEvent<MyEvent>().Publish();

            Assert.AreEqual(isNotified, true);
        }


        [TestMethod]
        public void PubSub_WithEmptyEvent_NotifyAll()
        {
            var service = GetService();

            int count = 0;

            service.GetEvent<MyEvent>().Subscribe(() =>
            {
                count++;
            });


            service.GetEvent<MyEvent>().Subscribe(() =>
            {
                count++;
            });

            service.GetEvent<MyEvent>().Subscribe(() =>
            {
                count++;
            });

            service.GetEvent<MyEvent>().Publish();

            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void PubSub_WithEvent()
        {
            bool isNotified = false;
            string result = null;

            var service = GetService();

            var parameter = "my parameter";

            service.GetEvent<MyStringEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            });

            service.GetEvent<MyStringEvent>().Publish(parameter);

            Assert.IsTrue(isNotified);
            Assert.AreEqual(parameter, result);
        }

        [TestMethod]
        public void PubSub_WithEvent_NotifyAll()
        {
            var service = GetService();

            int count = 0;

            service.GetEvent<MyStringEvent>().Subscribe((r) =>
          {
              count++;
          });


            service.GetEvent<MyStringEvent>().Subscribe((r) =>
          {
              count++;
          });

            service.GetEvent<MyStringEvent>().Subscribe((r) =>
            {
                count++;
            });

            service.GetEvent<MyStringEvent>().Publish("result");

            Assert.AreEqual(3, count);
        }
        [TestMethod]
        public void PubSub_WithObject()
        {
            bool isNotified = false;
            User result = null;

            var service = GetService();

            var parameter = new User
            {
                Id = 1,
                UserName = "Marie"
            };

            service.GetEvent<MyUserEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            });

            service.GetEvent<MyUserEvent>().Publish(parameter);

            Assert.IsTrue(isNotified);
            Assert.AreEqual(parameter, result);
        }

        [TestMethod]
        public void PubSub_WithFilter()
        {
            bool isNotified = false;
            bool isFilterCalled = false;
            string filterParameter = null;
            string result = null;

            var service = GetService();

            var parameter = "my parameter";

            service.GetEvent<MyStringEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            }, (r2) =>
             {
                 isFilterCalled = true;
                 filterParameter = r2;
                 return true;
             });

            service.GetEvent<MyStringEvent>().Publish(parameter);

            Assert.IsTrue(isNotified);
            Assert.IsTrue(isFilterCalled);
            Assert.AreEqual(parameter, result);
            Assert.AreEqual(parameter, filterParameter);
        }

        [TestMethod]
        public void PubSub_WithFilter_Cancel()
        {
            bool isNotified = false;
            bool isFilterCalled = false;
            string filterParameter = null;
            string result = null;

            var service = GetService();

            var parameter = "my parameter";

            service.GetEvent<MyStringEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            }, (r2) =>
            {
                isFilterCalled = true;
                filterParameter = r2;
                return false;
            });

            service.GetEvent<MyStringEvent>().Publish(parameter);

            Assert.IsTrue(isFilterCalled);
            Assert.AreEqual(parameter, filterParameter);

            Assert.IsFalse(isNotified);
            Assert.IsNull(result);
        }


        [TestMethod]
        public void PubSub_WithObjectAndFilter()
        {
            bool isNotified = false;
            User result = null;

            var service = GetService();

            var users = new List<User>
            {
                new User  { Id = 1,  UserName = "Marie" },
                new User  { Id = 2,  UserName = "Pat" },
                new User  { Id = 3,  UserName = "Deb" }
            };

            service.GetEvent<MyUserEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            }, (r2) =>
            {
                return r2.Id == 2;
            });

            service.GetEvent<MyUserEvent>().Publish(users[1]);

            Assert.IsTrue(isNotified);
            Assert.AreEqual(users[1], result);
        }

        [TestMethod]
        public void PubSub_WithObjectAndFilter_Cancel()
        {
            bool isNotified = false;
            User result = null;

            var service = GetService();

            var users = new List<User>
            {
                new User  { Id = 1,  UserName = "Marie" },
                new User  { Id = 2,  UserName = "Pat" },
                new User  { Id = 3,  UserName = "Deb" }
            };

            service.GetEvent<MyUserEvent>().Subscribe((r) =>
            {
                isNotified = true;
                result = r;
            }, (user) => user.Id == 1);

            service.GetEvent<MyUserEvent>().Publish(new User { Id = 2, UserName = "Pat" });

            Assert.IsFalse(isNotified);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Unsubscribe()
        {
            var service = GetService();

            var sub1 = new Action(() => { });

            var subscription = service.GetEvent<MyEvent>().Subscribe(sub1);

            subscription.Unsubscribe();

            Assert.AreEqual(0, service.GetEvent<MyEvent>().SubcribersCount);
        }

        [TestMethod]
        public void UnsubscribeAll()
        {
            var service = GetService();

            var sub1 = new Action(() => { });
            var sub2 = new Action(() => { });

            var _event = service.GetEvent<MyEvent>();

            _event.Subscribe(sub1);
            _event.Subscribe(sub2);

            Assert.AreEqual(2, _event.SubcribersCount);

            _event.UnsubscribeAll();

            Assert.AreEqual(0, _event.SubcribersCount);
        }

        [TestMethod]
        public async Task PubSub_WithCurrentContext()
        {
            var service = GetService();

            bool isNotified = false;

            service.GetEvent<MyEvent>().UseCurrentSynchronizationContext = true;

            service.GetEvent<MyEvent>().Subscribe(() =>
            {
                isNotified = true;
            });

            service.GetEvent<MyEvent>().Publish();

            await Task.Delay(1);

            Assert.AreEqual(isNotified, true);
        }

        [TestMethod]
        public void PubSub_WithCallback()
        {
            var service = GetService();
            string received = null;
            string _response = null;

            service.GetEvent<MyCallbackEvent>().Subscribe((r) =>
            {
                received = r.Message;
                r.InvokeCallback("my response");
            });


            service.GetEvent<MyCallbackEvent>().Publish(new MyCallbackResult("first message", (response) =>
             {
                // receive "my response"
                _response = response;
             }));


            Assert.AreEqual("first message", received);
            Assert.AreEqual("my response", _response);
        }
        [TestMethod]
        public void PubSub_WithClass()
        {
            var service = GetService();

            var sub1 = new Sub1();

            service.GetEvent<MyEvent>().Subscribe(sub1.Receive);

            service.GetEvent<MyEvent>().Publish();

            Assert.IsTrue(sub1.IsNotified);
        }

        [TestMethod]
        public void PubSub_WithClassAndParameter()
        {
            var service = GetService();

            var sub = new Sub2();

            service.GetEvent<MyStringEvent>().Subscribe(sub.Receive);

            service.GetEvent<MyStringEvent>().Publish("my parameter");

            Assert.IsTrue(sub.IsNotified);
            Assert.AreEqual("my parameter", sub.Parameter);
        }

        [TestMethod]
        public void PubSub_WithClassAndParameterAndFilter()
        {
            var service = GetService();

            var sub = new Sub2();

            service.GetEvent<MyStringEvent>().Subscribe(sub.Receive, sub.Filter);

            service.GetEvent<MyStringEvent>().Publish("my parameter");

            Assert.IsTrue(sub.IsNotified);
            Assert.AreEqual("my parameter", sub.Parameter);
        }

        [TestMethod]
        public void PubSub_WithClassBoolParameter()
        {
            var service = GetService();

            var sub = new Sub3();

            service.GetEvent<MyBoolEvent>().Subscribe(sub.Receive);

            service.GetEvent<MyBoolEvent>().Publish(true);

            Assert.IsTrue(sub.IsNotified);
            Assert.AreEqual(true, sub.Parameter);
        }

        [TestMethod]
        public void PubSub_WithClassBoolParameterAndFilter()
        {
            var service = GetService();

            var sub = new Sub3();

            service.GetEvent<MyBoolEvent>().Subscribe(sub.Receive, sub.Filter);

            service.GetEvent<MyBoolEvent>().Publish(true);

            Assert.IsTrue(sub.IsNotified);
            Assert.AreEqual(true, sub.Parameter);
            Assert.AreEqual(true, sub.FilterParameter);
        }


    }

    public class Sub1
    {
        public bool IsNotified { get; set; }

        public void Receive()
        {
            IsNotified = true;
        }
    }

    public class Sub2
    {
        public bool IsNotified { get; set; }
        public string Parameter { get; set; }
        public string FilterParameter { get; set; }

        public void Receive(string parameter)
        {
            IsNotified = true;
            Parameter = parameter;
        }

        public bool Filter(string parameter)
        {
            FilterParameter = parameter;
            return true;
        }
    }

    public class Sub3
    {
        public bool IsNotified { get; set; }
        public bool Parameter { get; set; }
        public bool FilterParameter { get; set; }

        public void Receive(bool parameter)
        {
            IsNotified = true;
            Parameter = parameter;
        }

        public bool Filter(bool parameter)
        {
            FilterParameter = parameter;
            return true;
        }
    }

    public class MyEvent : EmptyEvent
    {

    }

    public class MyStringEvent : ParameterizedEvent<string>
    {

    }

    public class MyBoolEvent : ParameterizedEvent<bool>
    {

    }

    public class MyUserEvent : ParameterizedEvent<User>
    {

    }

    public class MyCallbackEvent : ParameterizedEvent<MyCallbackResult>
    {

    }

    public class MyCallbackResult : ResultWithCallback<string>
    {
        public string Message { get; set; }

        public MyCallbackResult(string message, Action<string> callback)
            : base(callback)
        {
            this.Message = message;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
