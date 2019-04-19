using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FallExitAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 1;
        protected override double DefaultTo => 0.5;
        protected override Duration DefaultDuration => new Duration(TimeSpan.FromMilliseconds(400));
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 0.5);

        private ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
            protected set { scaleTransform = value; }
        }

        private AnimationTimeline opacityAnimation;
        public AnimationTimeline OpacityAnimation
        {
            get { return opacityAnimation; }
            protected set { opacityAnimation = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            // => scale 0.5 + opactiy 0

            ScaleTransform = new ScaleTransform();
            Element.RenderTransform = ScaleTransform;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            opacityAnimation = new DoubleAnimation(1, 0, Duration);

            return animation;
        }

        protected override void BeginAnimation()
        {
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
            Element.BeginAnimation(Control.OpacityProperty, opacityAnimation);
        }

        public override void CancelAnimation()
        {
            if (ScaleTransform != null)
            {
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                Element.BeginAnimation(Control.OpacityProperty, null);
            }
        }
    }

}

