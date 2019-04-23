using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class FallEntranceAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 300;
        protected override double DefaultTo => 0;
        protected override Duration DefaultDuration => new Duration(TimeSpan.FromMilliseconds(400));

        private AnimationTimeline opacityAnimation;
        public AnimationTimeline OpacityAnimation
        {
            get { return opacityAnimation; }
            protected set { opacityAnimation = value; }
        }

        protected override AnimationTimeline[] CreateAnimations()
        {
            // => translate Y 150% => 0 + opactiy 0 => 1
            var translateAnimation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                translateAnimation.EasingFunction = EasingFunction;

            opacityAnimation = new DoubleAnimation(0, 1, Duration);

            return new AnimationTimeline[] { translateAnimation, opacityAnimation };
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            if (index == 0)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            else if (index == 1)
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));
        }
    }
}
