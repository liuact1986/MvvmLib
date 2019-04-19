using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FXCornerExitAnimation : ContentAnimationBase
    {
        private ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
            protected set { scaleTransform = value; }
        }

        public override void CancelAnimation()
        {
            if (ScaleTransform != null)
            {
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseInOut };

            ScaleTransform = new ScaleTransform();
            Element.RenderTransform = ScaleTransform;
            Element.RenderTransformOrigin = new Point(1, 0);

            var animation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));
            return animation;
        }

        protected override void BeginAnimation()
        {
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
        }
    }



}
