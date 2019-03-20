using System;

namespace MvvmLib.Navigation
{
    public interface IItemsRegionAdapter
    {
        Type TargetType { get; }

        void OnInsert(object control, object view, int index);
        void OnRemoveAt(object control, int index);
        void OnClear(object control);
    }
}