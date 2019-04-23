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

        /// <summary>
        /// Tries to create a delegate if weak reference target is alive.
        /// </summary>
        public Delegate Target => TryGetDelegate();

        /// <summary>
        /// Creates the weak delegate class.
        /// </summary>
        /// <param name="delegate">The delegate</param>
        public WeakDelegate(Delegate @delegate)
        {
            if (@delegate == null) { throw new ArgumentNullException(nameof(@delegate)); }

            weakReference = new WeakReference(@delegate.Target);
            method = @delegate.Method;
            delegateType = @delegate.GetType();
        }

        private Delegate TryGetDelegate()
        {
            if (method.IsStatic)
                return Delegate.CreateDelegate(delegateType, null, method);

            object target = weakReference.Target;
            if (target != null)
                return Delegate.CreateDelegate(delegateType, target, method);


            return null;
        }
    }
}
