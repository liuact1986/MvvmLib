using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FxVScaleExitAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 20;
        protected override double DefaultTo => -60;
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 0.1);

        protected override AnimationTimeline[] CreateAnimations()
        {
            /*50% { transform: translateY(25%) scale(1.1); opacity: 1;}
	          100% { transform: translateY(-75%) scale(0); opacity: 0;  }*/
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new ExponentialEase { EasingMode = EasingMode.EaseOut };

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            var scaleYAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            var translateAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            translateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(From, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            translateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(To, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));


            var opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            return new AnimationTimeline[] { scaleXAnimation, scaleYAnimation, translateAnimation,  opacityAnimation };
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            if (index == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            else if (index == 1)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            else if (index == 2)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            else if (index == 3)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));
        }

    }
}
