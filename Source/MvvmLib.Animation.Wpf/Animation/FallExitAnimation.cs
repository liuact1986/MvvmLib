using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FallExitAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 1;
        protected override double DefaultTo => 0.5;
        protected override Duration DefaultDuration => new Duration(TimeSpan.FromMilliseconds(400));

        protected override AnimationTimeline[] CreateAnimations()
        {
            var scaleXAnimation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                scaleXAnimation.EasingFunction = EasingFunction;

            var scaleYAnimation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                scaleYAnimation.EasingFunction = EasingFunction;

            var opacityAnimation = new DoubleAnimation(1, 0, Duration);

            return new AnimationTimeline[] { scaleXAnimation, scaleYAnimation, opacityAnimation };
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            if (index == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            else if (index == 1)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            else if (index == 2)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));
        }
    }


}
