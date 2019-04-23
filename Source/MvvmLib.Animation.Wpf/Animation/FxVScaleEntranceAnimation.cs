using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FxVScaleEntranceAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 60;
        protected override double DefaultTo => 0;
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 1);

        protected override AnimationTimeline[] CreateAnimations()
        {
            /* from {  transform: translateY(75%) scale(0); }
            to { transform: translateY(0) scale(1);  opacity: 1; } */

            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseIn };

            var translateAnimation = new DoubleAnimation(From, To, Duration);

            var scaleXAnimation = new DoubleAnimation(0, 1, Duration);
            scaleXAnimation.EasingFunction = EasingFunction;

            var scaleYAnimation = new DoubleAnimation(0, 1, Duration);
            scaleYAnimation.EasingFunction = EasingFunction;

            var opacityAnimation = new DoubleAnimation(0, 1, Duration);

            return new AnimationTimeline[] { translateAnimation, scaleXAnimation, scaleYAnimation, opacityAnimation };
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            if (index == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            else if (index == 1)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            else if (index == 2)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            else if (index == 3)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));

        }
    }
}
