using System;
using System.Reflection;

namespace MvvmLib.Message
{
    public class WeakDelegate
    {
        private readonly WeakReference weakReference;
        private readonly MethodInfo method;
        private readonly Type delegateType;

        public bool IsAlive => this.weakReference.IsAlive && this.weakReference.Target != null;

        public object Target => this.weakReference.Target;

        public WeakDelegate(Delegate @delegate)
        {
            this.weakReference = new WeakReference(@delegate.Target);
            this.method = @delegate.GetMethodInfo();
            this.delegateType = @delegate.GetType();
        }

        private Delegate CreateDelegate()
        {
            return this.method.CreateDelegate(this.delegateType, weakReference.Target);
        }

        public void Kill()
        {
            this.weakReference.Target = null;
        }

        public Delegate TryGetDelegate()
        {
            if (method.IsStatic)
            {
                return Delegate.CreateDelegate(delegateType, null, method);
            }
            var target = this.weakReference.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(delegateType, target, method);
            }
            return null;
        }
    }

}
