using System;
using System.Reflection;

namespace MvvmLib.Message
{
    /// <summary>
    /// Use Weak references to store and create delegates.
    /// </summary>
    public interface IWeakDelegate
    {
        /// <summary>
        /// Tries to create a delegate if weak reference target is alive.
        /// </summary>
        Delegate Target { get; }
    }

    /// <summary>
    /// Use Weak references to store and create delegates.
    /// </summary>
    public class WeakDelegate : IWeakDelegate
    {
        private readonly WeakReference weakReference;
        private readonly MethodInfo method;
        private readonly Type delegateType;
        private readonly bool keepAlive;
        private Delegate @delegate;

        /// <summary>
        /// Tries to create a delegate if weak reference target is alive.
        /// </summary>
        public Delegate Target
        {
            get { return TryGetDelegate(); }
        }

        /// <summary>
        /// Creates the weak delegate class.
        /// </summary>
        /// <param name="delegate">The delegate</param>
        /// <param name="keepAlive">Allows to keep alive the reference</param>
        public WeakDelegate(Delegate @delegate, bool keepAlive)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate)); 

            if (keepAlive)
            {
                this.keepAlive = true;
                this.@delegate = @delegate;
            }
            else
            {
                weakReference = new WeakReference(@delegate.Target);
                method = @delegate.Method;
                delegateType = @delegate.GetType();
            }
        }

        private Delegate TryGetDelegate()
        {
            if (keepAlive)
            {
                return @delegate;
            }
            else
            {
                if (method.IsStatic)
                    return Delegate.CreateDelegate(delegateType, null, method);

                object target = weakReference.Target;
                if (target != null)
                    return Delegate.CreateDelegate(delegateType, target, method);
            }

            return null;
        }
    }
}
