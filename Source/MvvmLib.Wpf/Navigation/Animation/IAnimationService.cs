using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public interface IAnimationService
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

        DoubleAnimation CreateDoubleAnimation(double from, double to, int milliseconds, IEasingFunction ease = null, EventHandler onCompleteCallback = null);
        void FadeIn(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null);
        void FadeIn(UIElement elementToAnimate, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null);
        void FadeOut(UIElement elementToAnimate, EventHandler onCompleteCallback = null);
        void FadeOut(UIElement elementToAnimate, int milliseconds, EventHandler onCompleteCallback = null);
        void SlideInFromBottom(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInFromLeft(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInFromRight(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInFromTop(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInX(UIElement elementToAnimate, double from, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInX(UIElement elementToAnimate, double from, int milliseconds, IEasingFunction easingFunction, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInY(UIElement elementToAnimate, double from, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideInY(UIElement elementToAnimate, double from, int milliseconds, IEasingFunction easingFunction, Action setContentCallback, EventHandler onCompleteCallback = null);
        void SlideOutToBottom(UIElement elementToAnimate, EventHandler onCompleteCallback = null);
        void SlideOutToLeft(UIElement elementToAnimate, EventHandler onCompleteCallback = null);
        void SlideOutToRight(UIElement elementToAnimate, EventHandler onCompleteCallback = null);
        void SlideOutToTop(UIElement elementToAnimate, EventHandler onCompleteCallback = null);
        void SlideOutX(UIElement elementToAnimate, double to, int milliseconds, EventHandler onCompleteCallback = null);
        void SlideOutX(UIElement elementToAnimate, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null);
        void SlideOutY(UIElement elementToAnimate, double to, int milliseconds, EventHandler onCompleteCallback = null);
        void SlideOutY(UIElement elementToAnimate, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null);
        void SlideX(UIElement elementToAnimate, double from, double to, int milliseconds, IEasingFunction ease = null, EventHandler onCompleteCallback = null);
        void SlideY(UIElement elementToAnimate, double from, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null);
        void TranslateX(UIElement elementToAnimate, DoubleAnimation animation);
        void TranslateY(UIElement elementToAnimate, DoubleAnimation animation);
    }
}