using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmLib.Message
{
    /// <summary>
    /// Subscriber with no parameter.
    /// </summary>
    public sealed class Subscriber : SubscriberBase
    {
        internal WeakDelegate weakAction;
        /// <summary>
        /// The action to call.
        /// </summary>
        public Action Action
        {
            get { return (Action)weakAction.Target; }
        }

        /// <summary>
        /// Creates the <see cref="Subscriber"/>.
        /// </summary>
        /// <param name="subscriptionToken">The subscription token</param>
        /// <param name="synchronizationContext">The synchronization context</param>
        /// <param name="weakAction">The action</param>
        public Subscriber(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext, WeakDelegate weakAction)
            : base(subscriptionToken, synchronizationContext)
        {
            if (weakAction == null)
                throw new ArgumentNullException(nameof(weakAction));

            this.weakAction = weakAction;
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="action">The action</param>
        public void Invoke(Action action)
        {
            switch (ExecutionStrategy)
            {
                case ExecutionStrategyType.PublisherThread:
                    action();
                    break;
                case ExecutionStrategyType.UIThread:
                    if(this.SynchronizationContext == null)
                        throw new InvalidOperationException($"Cannot invoke the action with \"{ExecutionStrategy}\". The Synchronization context is null"); 

                    this.SynchronizationContext.Post(_ => action(), null);
                    break;
                case ExecutionStrategyType.BackgroundThread:
                    Task.Run(() => action());
                    break;
            }
        }
    }
}
