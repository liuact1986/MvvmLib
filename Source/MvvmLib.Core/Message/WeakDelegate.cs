using System;
using System.Reflection;

namespace MvvmLib.Message
{
    public interface IWeakDelegate
    {
        Delegate Target { get; }
    }

    public class WeakDelegate : IWeakDelegate
    {
        private readonly WeakReference weakReference;
        private readonly MethodInfo method;
        private readonly Type delegateType;

        public Delegate Target => TryGetDelegate();

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
