using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public interface IContentAnimation
    {
        AnimationTimeline Animation { get; }
        Duration Duration { get; set; }
        IEasingFunction EasingFunction { get; set; }
        UIElement Element { get; }
        double From { get; set; }
        bool IsAnimating { get; }
        Action OnCompleted { get; set; }
        Action OnPrepare { get; set; }
        double To { get; set; }

        void CancelAnimation();
        void HandleAnimationCompleted();
        void Prepare();
        void Start(UIElement element, Action onCompleted);
        void UnhandleAnimationCompleted();
    }



}
