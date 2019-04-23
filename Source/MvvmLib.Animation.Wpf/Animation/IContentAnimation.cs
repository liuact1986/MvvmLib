using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public interface IContentAnimation
    {
        AnimationTimeline[] Animations { get; }
        Duration Duration { get; set; }
        IEasingFunction EasingFunction { get; set; }
        UIElement Element { get; set; }
        double From { get; set; }
        bool IsAnimating { get; }
        Action OnCancelled { get; set; }
        Action OnCompleted { get; set; }
        Action<ContentAnimationBase> OnPrepare { get; set; }
        Storyboard Storyboard { get; }
        double To { get; set; }

        void CancelAnimations();
        void HandleCompleted();
        void Run(UIElement element);
        void Run(UIElement element, Action onCompleted);
        void Run(UIElement element, Action onCompleted, Action onCancelled);
        void UnhandleCompleted();
    }
}