using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class RotateAnimation : TransformAnimationBase
    {
        private RotateTransform rotate;
        public RotateTransform Rotate
        {
            get { return rotate; }
            protected set { rotate = value; }
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
            Rotate = new RotateTransform();
            if (Angle.HasValue)
                Rotate.Angle = Angle.Value;
            if (CenterX.HasValue)
                Rotate.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                Rotate.CenterY = CenterY.Value;

            Element.RenderTransform = Rotate;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            return animation;
        }

        protected override void BeginAnimation()
        {
            Rotate.BeginAnimation(RotateTransform.AngleProperty, Animation);
        }

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                Rotate.BeginAnimation(RotateTransform.AngleProperty, null);
                AnimationWasCancelled = true;
                IsAnimating = false;
            }
        }
    }


}
