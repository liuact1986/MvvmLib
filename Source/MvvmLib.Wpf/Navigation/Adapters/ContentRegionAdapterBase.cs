using System;

namespace MvvmLib.Navigation
{
    public abstract class ContentRegionAdapterBase<T> : IContentRegionAdapter
    {
        public Type TargetType => typeof(T);

        public abstract object GetContent(T control);

        public abstract void OnNavigate(T control, object view);

        public abstract void OnGoBack(T control, object previousView);

        public abstract void OnGoForward(T control, object nextView);

        public void OnNavigate(object control, object view)
        {
            this.OnNavigate((T)control, view);
        }

        public void OnGoBack(object control, object previousView)
        {
            this.OnGoBack((T)control, previousView);
        }

        public void OnGoForward(object control, object nextView)
        {
            this.OnGoForward((T)control, nextView);
        }

        public object GetContent(object control)
        {
            return this.GetContent((T)control);
        }
    }
}