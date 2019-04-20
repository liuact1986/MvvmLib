using System;
using System.Collections.Generic;
using System.Windows;

namespace MvvmLib.Navigation
{
    public interface IItemsRegionNavigationAnimation
    {
        FrameworkElement Control { get; }
        IContentAnimation EntranceAnimation { get; set; }
        IContentAnimation ExitAnimation { get; set; }
        bool IsAnimating { get; }
        bool IsEntering { get; }
        bool IsLeaving { get; }
        IItemsRegionAdapter ItemsRegionAdapter { get; }
        Queue<NavigationQueueItem> OnEnterQueue { get; }
        Queue<NavigationQueueItem> OnLeaveQueue { get; }

        void DoOnEnter(object newContent, int index, Action onEnterCompleted);
        void DoOnLeave(object oldContent, int index, Action onEnterCompleted);
        void Reset(UIElement element);
    }
}