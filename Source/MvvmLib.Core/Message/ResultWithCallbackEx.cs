using System;

namespace MvvmLib.Message
{
    public class ResultWithCallback<T>
    {
        protected Action<T> callback;

        public ResultWithCallback(Action<T> callback)
        {
            this.callback = callback;
        }

        public virtual object InvokeCallback(T result)
        {
            return this.callback.DynamicInvoke(result);
        }
    }
}
