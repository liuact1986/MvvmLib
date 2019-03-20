using System;

namespace MvvmLib.Navigation
{
    public abstract class ItemsRegionAdapterBase<T> : IItemsRegionAdapter
    {
        public Type TargetType => typeof(T);

        public abstract void OnInsert(T control, object view, int index);

        public abstract void OnClear(T control);

        public abstract void OnRemoveAt(T control, int index);

        public void OnInsert(object control, object view, int index)
        {
            this.OnInsert((T)control, view, index);
        }

        public void OnRemoveAt(object control, int index)
        {
            this.OnRemoveAt((T)control, index);
        }

        public void OnClear(object control)
        {
            this.OnClear((T)control);
        }
        
    }
}