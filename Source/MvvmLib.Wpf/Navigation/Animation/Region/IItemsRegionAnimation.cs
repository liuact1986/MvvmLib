using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    public interface IItemsRegionAnimation
    {
        IContentAnimation EntranceAnimation { get; set; }
        IContentAnimation ExitAnimation { get; set; }

        void DoOnEnter(object newContent, Action onEnterCompleted);
        void DoOnLeave(object oldContent, Action onLeaveCompleted);
        void Reset(UIElement element);
    }
}