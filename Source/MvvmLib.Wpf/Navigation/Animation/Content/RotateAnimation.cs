using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class RotateAnimation : TransformAnimationBase
    {
        private RotateTransform rotateTransform;
        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
            protected set { rotateTransform = value; }
        }

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

        protected override AnimationTimeline CreateAnimation()
        {
            // Angle , CenterX, CenterY
            RotateTransform = new RotateTransform();
            if (Angle.HasValue)
                RotateTransform.Angle = Angle.Value;
            if (CenterX.HasValue)
                RotateTransform.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                RotateTransform.CenterY = CenterY.Value;

            Element.RenderTransform = RotateTransform;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            return animation;
        }

        protected override void BeginAnimation()
        {
            RotateTransform.BeginAnimation(RotateTransform.AngleProperty, Animation);
        }

        public override void CancelAnimation()
        {
            if (RotateTransform != null)
                RotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);
        }
    }


}
