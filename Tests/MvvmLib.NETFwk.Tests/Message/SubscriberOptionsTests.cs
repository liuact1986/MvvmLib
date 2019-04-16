using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Message;
using System;
using System.Threading;

namespace MvvmLib.Core.Tests.Message
{
    [TestClass]
    public class SubscriberOptionsTests
    {
        SynchronizationContext synchronizationContext = new SynchronizationContext();

        [TestMethod]
        public void Init_And_Update()
        {
            var unsub = new Func<SubscriptionToken, bool>(_ => true);
            var action = new Action(() =>
            {

            });

            var token = new SubscriptionToken(unsub);
            var weakAction = new WeakDelegate(action);
            var subscription = new Subscriber(token, synchronizationContext, weakAction);
            var options = new SubscriberOptions(subscription);

            Assert.AreEqual(token, options.Token);
            //
            Assert.AreEqual(ExecutionStrategyType.PublisherThread, subscription.InvocationStrategy);
            options.WithExecutionStrategy(ExecutionStrategyType.UIThread);

            Assert.AreEqual(ExecutionStrategyType.UIThread, subscription.InvocationStrategy);
        }

        [TestMethod]
        public void Init_And_Update_With_Generic_Options()
        {
            var unsub = new Func<SubscriptionToken, bool>(_ => true);
            var action = new Action<string>((t) =>
            {

            });

            var token = new SubscriptionToken(unsub);
            var weakAction = new WeakDelegate(action);
            var subscription = new Subscriber<string>(token, synchronizationContext, weakAction);
            var options = new SubscriberOptions<string>(subscription);

            Assert.AreEqual(token, options.Token);
            //
            Assert.AreEqual(ExecutionStrategyType.PublisherThread, subscription.InvocationStrategy);
            options.WithExecutionStrategy(ExecutionStrategyType.UIThread);

            Assert.AreEqual(ExecutionStrategyType.UIThread, subscription.InvocationStrategy);

            //
            Assert.IsNotNull(subscription.Filter);

            var newFilter = new Func<string, bool>(_ => true);

            options.WithFilter(newFilter);

            Assert.AreEqual("MvvmLib.Core.Tests.Message.SubscriberOptionsTests+<>c", subscription.Filter.Target.GetType().FullName);
        }
    }

}
