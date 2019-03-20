using System;

namespace MvvmLib.Message
{
    public class ResultWithCallback
    {
        protected Delegate callback;

        public ResultWithCallback(Action callback)
        {
            this.callback = callback;
        }

        public virtual object InvokeCallback()
        {
            return this.callback.DynamicInvoke();
        }
    }
}
