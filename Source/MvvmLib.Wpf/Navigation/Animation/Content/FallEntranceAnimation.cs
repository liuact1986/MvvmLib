using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class FallEntranceAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 300;
        protected override double DefaultTo => 0;
        protected override Duration DefaultDuration => new Duration(TimeSpan.FromMilliseconds(400));
        protected override Point DefaultRenderTransformOrigin => new Point(0.5, 0.5);

        private TranslateTransform translateTransform;
        public TranslateTransform TranslateTransform
        {
            get { return translateTransform; }
            protected set { translateTransform = value; }
        }

        private AnimationTimeline opacityAnimation;
        public AnimationTimeline OpacityAnimation
        {
            get { return opacityAnimation; }
            protected set { opacityAnimation = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            // => translate Y 150% => 0 + opactiy 0 => 1

            translateTransform = new TranslateTransform();
            Element.RenderTransform = translateTransform;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            opacityAnimation = new DoubleAnimation(0, 1, Duration);

            return animation;
        }

        protected override void BeginAnimation()
        {
            translateTransform.BeginAnimation(TranslateTransform.YProperty, Animation);
            Element.BeginAnimation(Control.OpacityProperty, opacityAnimation);
        }

        public override void CancelAnimation()
        {
            if (translateTransform != null && Element != null)
            {
                translateTransform.BeginAnimation(TranslateTransform.YProperty, null);
                Element.BeginAnimation(Control.OpacityProperty, null);
            }
        }
    }

}

