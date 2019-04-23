using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class RotateAnimation : TransformAnimationBase
    {
        private double? centerX;
        public double? CenterX
        {
            get { return centerX; }
            set { centerX = value; }
        }

        private double? centerY;
        public double? CenterY
        {
            get { return centerY; }
            set { centerY = value; }
        }

        private double? angle;
        public double? Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        protected override void InitTransformProperties()
        {
            if (Angle.HasValue)
                RotateTransform.Angle = Angle.Value;
            if (CenterX.HasValue)
                RotateTransform.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                RotateTransform.CenterY = CenterY.Value;
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
        }
    }


}
