using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FxVScaleExitAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 20;
        protected override double DefaultTo => -60;
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 0.1);

        private ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
            protected set { scaleTransform = value; }
        }

        private TranslateTransform translateTransform;
        public TranslateTransform TranslateTransform
        {
            get { return translateTransform; }
            protected set { translateTransform = value; }
        }

        private DoubleAnimationUsingKeyFrames translateAnimation;
        public DoubleAnimationUsingKeyFrames TranslateAnimation
        {
            get { return translateAnimation; }
            protected set { translateAnimation = value; }
        }

        private DoubleAnimationUsingKeyFrames opacityAnimation;
        public DoubleAnimationUsingKeyFrames OpacityAnimation
        {
            get { return opacityAnimation; }
            protected set { opacityAnimation = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            /*50% { transform: translateY(25%) scale(1.1); opacity: 1;}
	          100% { transform: translateY(-75%) scale(0); opacity: 0;  }*/
            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new ExponentialEase { EasingMode = EasingMode.EaseOut };

            ScaleTransform = new ScaleTransform();
            TranslateTransform = new TranslateTransform();

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(ScaleTransform);
            transformGroup.Children.Add(TranslateTransform);

            Element.RenderTransform = transformGroup;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var scaleAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            translateAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            translateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(From, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            translateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(To, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));


            opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = Duration
            };
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs / 2)), easingFunction));
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(animationDurationMs)), easingFunction));

            return scaleAnimation;
        }

        protected override void BeginAnimation()
        {
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
            TranslateTransform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);
            Element.BeginAnimation(Control.OpacityProperty, opacityAnimation);
        }

        public override void CancelAnimation()
        {
            if (ScaleTransform != null && Element != null)
            {
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                TranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);
                Element.BeginAnimation(Control.OpacityProperty, null);
            }
        }
    }

}

