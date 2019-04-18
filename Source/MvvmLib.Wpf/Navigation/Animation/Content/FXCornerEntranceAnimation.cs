using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FXCornerEntranceAnimation : ContentAnimationBase
    {
        private ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
            protected set { scaleTransform = value; }
        }

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                IsAnimating = false;
            }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var delay = Convert.ToInt32(animationDurationMs / 2);
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseInOut };

            ScaleTransform = new ScaleTransform { ScaleX = 0, ScaleY = 0 };
            Element.RenderTransform = ScaleTransform;
            Element.RenderTransformOrigin = new Point(0, 1);

            var animation = new DoubleAnimation(0, 1, Duration);
            animation.EasingFunction = easingFunction;
            animation.BeginTime = new TimeSpan(0, 0, 0, 0, delay);
            return animation;
        }

        protected override void BeginAnimation()
        {
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
        }
    }



}
