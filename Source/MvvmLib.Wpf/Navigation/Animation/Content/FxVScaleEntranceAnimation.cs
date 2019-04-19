using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FxVScaleEntranceAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 60;
        protected override double DefaultTo => 0;
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 1);

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

        private AnimationTimeline translateAnimation;
        public AnimationTimeline TranslateAnimation
        {
            get { return translateAnimation; }
            protected set { translateAnimation = value; }
        }

        private AnimationTimeline opacityAnimation;
        public AnimationTimeline OpacityAnimation
        {
            get { return opacityAnimation; }
            protected set { opacityAnimation = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {

            /* from {  transform: translateY(75%) scale(0); }
            to { transform: translateY(0) scale(1);  opacity: 1; } */

            var animationDurationMs = Duration.TimeSpan.TotalMilliseconds;
            var easingFunction = EasingFunction ?? new CubicEase { EasingMode = EasingMode.EaseIn };

            ScaleTransform = new ScaleTransform();
            TranslateTransform = new TranslateTransform();

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(ScaleTransform);
            transformGroup.Children.Add(TranslateTransform);

            Element.RenderTransform = transformGroup;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            translateAnimation = new DoubleAnimation(From, To, Duration);

            var scaleAnimation = new DoubleAnimation(0, 1, Duration);
            scaleAnimation.EasingFunction = EasingFunction;

            opacityAnimation = new DoubleAnimation(0, 1, Duration);

            return scaleAnimation;
        }

        protected override void BeginAnimation()
        {
            TranslateTransform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
            Element.BeginAnimation(Control.OpacityProperty, opacityAnimation);
        }

        public override void CancelAnimation()
        {
            if (ScaleTransform != null && Element != null)
            {
                TranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                Element.BeginAnimation(Control.OpacityProperty, null);
            }
        }
    }

}

