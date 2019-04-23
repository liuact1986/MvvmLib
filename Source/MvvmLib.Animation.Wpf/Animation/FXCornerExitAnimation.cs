using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FXCornerExitAnimation : TransformAnimationBase
    {
        protected override Point DefaultRenderTransformOrigin => new Point(1, 0);

        protected override AnimationTimeline[] CreateAnimations()
        {
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseInOut };

            var scaleYAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            return new AnimationTimeline[] { scaleYAnimation, scaleXAnimation };
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
