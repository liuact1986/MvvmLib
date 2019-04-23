using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FXCornerEntranceAnimation : TransformAnimationBase
    {
        protected override Point DefaultRenderTransformOrigin => new Point(0, 1);

        protected override void InitTransformProperties()
        {
            scaleTransform.ScaleX = 0;
            scaleTransform.ScaleY = 0;
        }

        protected override AnimationTimeline[] CreateAnimations()
        {
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var delay = Convert.ToInt32(animationDurationMs / 2);
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseInOut };

            var scaleXAnimation = new DoubleAnimation(0, 1, Duration);
            scaleXAnimation.EasingFunction = easingFunction;
            scaleXAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, delay);

            var scaleYAnimation = new DoubleAnimation(0, 1, Duration);
            scaleYAnimation.EasingFunction = easingFunction;
            scaleYAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, delay);

            return new AnimationTimeline[] { scaleXAnimation, scaleYAnimation };
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            if (index == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            else if (index == 1)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
        }
    }


}
