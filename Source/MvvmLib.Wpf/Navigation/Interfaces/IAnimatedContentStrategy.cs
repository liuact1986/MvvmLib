using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public interface IAnimatedContentStrategy
    {
        int DefaultAnimationDuration { get; set; }
        IEasingFunction DefaultEaseFunction { get; set; }
        double DefaultFromBottomValue { get; set; }
        double DefaultFromLeftValue { get; set; }
        double DefaultFromRightValue { get; set; }
        double DefaultFromTopValue { get; set; }
        double DefaultToBottomValue { get; set; }
        double DefaultToLeftValue { get; set; }
        double DefaultToRightValue { get; set; }
        double DefaultToTopValue { get; set; }

        void OnEnter(FrameworkElement view, Action setContentCallback, EntranceTransitionType entranceTransitionType, Action cb = null);
        void OnLeave(FrameworkElement view, ExitTransitionType exitTransitionType, Action cb = null);
    }
}