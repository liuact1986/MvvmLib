using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class OpacityAnimation : ContentAnimationBase
    {
        protected override double DefaultFrom => 0;
        protected override double DefaultTo => 1;

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                Element.BeginAnimation(Control.OpacityProperty, null);
                AnimationWasCancelled = true;
                IsAnimating = false;
            }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            if (From < 0 || From > 1) { throw new ArgumentException("Value between 0 and 1 for an opacity animation"); }
            if (To < 0 || To > 1) { throw new ArgumentException("Value between 0 and 1 for an opacity animation"); }

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;
            return animation;
        }

        protected override void BeginAnimation()
        {
            Element.BeginAnimation(Control.OpacityProperty, Animation);
        }
    }
}
