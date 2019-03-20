using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class AnimationService : IAnimationService
    {
        public int DefaultAnimationDuration { get; set; } = 300;

        public IEasingFunction DefaultEaseFunction { get; set; } = new CubicEase { EasingMode = EasingMode.EaseOut };

        public double DefaultFromLeftValue { get; set; } = -40;
        public double DefaultFromRightValue { get; set; } = 40;
        public double DefaultFromTopValue { get; set; } = -40;
        public double DefaultFromBottomValue { get; set; } = 40;

        public double DefaultToLeftValue { get; set; } = -40;
        public double DefaultToRightValue { get; set; } = 40;
        public double DefaultToTopValue { get; set; } = -40;
        public double DefaultToBottomValue { get; set; } = 40;


        public DoubleAnimation CreateDoubleAnimation(double from, double to, int milliseconds, IEasingFunction ease = null, EventHandler onCompleteCallback = null)
        {
            var animation = new DoubleAnimation(from, to, new Duration(TimeSpan.FromMilliseconds(milliseconds)));

            if (ease != null)
            {
                animation.EasingFunction = ease;
            }

            if (onCompleteCallback != null)
            {
                animation.Completed += onCompleteCallback;
            }

            return animation;
        }

        public void TranslateX(UIElement elementToAnimate, DoubleAnimation animation)
        {
            var transform = new TranslateTransform();
            elementToAnimate.RenderTransform = transform;
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        public void TranslateY(UIElement elementToAnimate, DoubleAnimation animation)
        {
            var transform = new TranslateTransform();
            elementToAnimate.RenderTransform = transform;
            transform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        public void FadeIn(UIElement elementToAnimate, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            // hide element (opacity 0)
            elementToAnimate.Opacity = 0;
            setContentCallback.Invoke();
            // animate opacity 0 to 1
            var animation = this.CreateDoubleAnimation(0, 1, milliseconds, null, onCompleteCallback);
            elementToAnimate.BeginAnimation(Control.OpacityProperty, animation);
        }

        public void FadeIn(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.FadeIn(elementToAnimate, DefaultAnimationDuration, setContentCallback, onCompleteCallback);
        }

        public void FadeOut(UIElement elementToAnimate, int milliseconds, EventHandler onCompleteCallback = null)
        {
            // animate opacity 1 to 0
            var animation = this.CreateDoubleAnimation(1, 0, milliseconds, null, onCompleteCallback);
            elementToAnimate.BeginAnimation(Control.OpacityProperty, animation);
        }

        public void FadeOut(UIElement elementToAnimate, EventHandler onCompleteCallback = null)
        {
            this.FadeOut(elementToAnimate, DefaultAnimationDuration, onCompleteCallback);
        }

        public void SlideX(UIElement elementToAnimate, double from, double to, int milliseconds, IEasingFunction ease = null, EventHandler onCompleteCallback = null)
        {
            var animation = this.CreateDoubleAnimation(from, to, milliseconds, null, onCompleteCallback);
            this.TranslateX(elementToAnimate, animation);
        }

        public void SlideOutX(UIElement elementToAnimate, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null)
        {
            this.SlideX(elementToAnimate, 0, to, milliseconds, easingFunction, onCompleteCallback);
        }

        public void SlideOutX(UIElement elementToAnimate, double to, int milliseconds, EventHandler onCompleteCallback = null)
        {
            this.SlideX(elementToAnimate, 0, to, milliseconds, DefaultEaseFunction, onCompleteCallback);
        }

        public void SlideOutToLeft(UIElement elementToAnimate, EventHandler onCompleteCallback = null)
        {
            this.SlideX(elementToAnimate, 0, DefaultToLeftValue, DefaultAnimationDuration, DefaultEaseFunction, onCompleteCallback);
        }

        public void SlideOutToRight(UIElement elementToAnimate, EventHandler onCompleteCallback = null)
        {
            this.SlideX(elementToAnimate, 0, DefaultToRightValue, DefaultAnimationDuration, DefaultEaseFunction, onCompleteCallback);
        }

        public void SlideInX(UIElement elementToAnimate, double from, int milliseconds, IEasingFunction easingFunction, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            // hide element, position, set content
            elementToAnimate.Visibility = Visibility.Collapsed;
            elementToAnimate.RenderTransform = new TranslateTransform { X = from };
            setContentCallback();
            elementToAnimate.Visibility = Visibility.Visible;
            // animate to 0
            this.SlideX(elementToAnimate, from, 0, milliseconds, easingFunction, onCompleteCallback);
        }

        public void SlideInX(UIElement elementToAnimate, double from, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInX(elementToAnimate, from, milliseconds, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideInFromLeft(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInX(elementToAnimate, DefaultFromLeftValue, DefaultAnimationDuration, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideInFromRight(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInX(elementToAnimate, DefaultFromRightValue, DefaultAnimationDuration, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideY(UIElement elementToAnimate, double from, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null)
        {
            var animation = this.CreateDoubleAnimation(from, to, milliseconds, easingFunction, onCompleteCallback);
            this.TranslateY(elementToAnimate, animation);
        }

        public void SlideInY(UIElement elementToAnimate, double from, int milliseconds, IEasingFunction easingFunction, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            // hide element, position, set content
            elementToAnimate.Visibility = Visibility.Collapsed;
            elementToAnimate.RenderTransform = new TranslateTransform { Y = from };
            setContentCallback();
            elementToAnimate.Visibility = Visibility.Visible;
            // animate to 0
            this.SlideY(elementToAnimate, from, 0, milliseconds, easingFunction, onCompleteCallback);
        }

        public void SlideInY(UIElement elementToAnimate, double from, int milliseconds, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInY(elementToAnimate, from, milliseconds, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideInFromTop(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInY(elementToAnimate, DefaultFromTopValue, DefaultAnimationDuration, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideInFromBottom(UIElement elementToAnimate, Action setContentCallback, EventHandler onCompleteCallback = null)
        {
            this.SlideInY(elementToAnimate, DefaultFromBottomValue, DefaultAnimationDuration, DefaultEaseFunction, setContentCallback, onCompleteCallback);
        }

        public void SlideOutY(UIElement elementToAnimate, double to, int milliseconds, IEasingFunction easingFunction, EventHandler onCompleteCallback = null)
        {
            this.SlideY(elementToAnimate, 0, to, milliseconds, easingFunction, onCompleteCallback);
        }

        public void SlideOutY(UIElement elementToAnimate, double to, int milliseconds, EventHandler onCompleteCallback = null)
        {
            this.SlideY(elementToAnimate, 0, to, milliseconds, DefaultEaseFunction, onCompleteCallback);
        }

        public void SlideOutToTop(UIElement elementToAnimate, EventHandler onCompleteCallback = null)
        {
            this.SlideY(elementToAnimate, 0, DefaultToTopValue, DefaultAnimationDuration, DefaultEaseFunction, onCompleteCallback);
        }

        public void SlideOutToBottom(UIElement elementToAnimate, EventHandler onCompleteCallback = null)
        {
            this.SlideY(elementToAnimate, 0, DefaultToBottomValue, DefaultAnimationDuration, DefaultEaseFunction, onCompleteCallback);
        }

    }

}
