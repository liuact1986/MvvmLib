using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class OpacityAnimation : ContentAnimationBase
    {
        protected override double DefaultFrom => 0;
        protected override double DefaultTo => 1;

        protected override void CheckValues()
        {
            if (From < 0 || From > 1)
                throw new ArgumentException("Value between 0 and 1 for an opacity animation");
            if (To < 0 || To > 1)
                throw new ArgumentException("Value between 0 and 1 for an opacity animation");
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Control.Opacity)"));
        }
    }


}
