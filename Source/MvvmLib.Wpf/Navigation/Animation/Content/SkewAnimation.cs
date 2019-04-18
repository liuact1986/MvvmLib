using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class SkewAnimation : TransformAnimationBase
    {
        private readonly TransformDirection defaultTransformDirection = TransformDirection.X;
        protected virtual TransformDirection DefaultTransformDirection => defaultTransformDirection;

        protected TransformDirection? transformDirection;
        public TransformDirection TransformDirection
        {
            get { return transformDirection ?? DefaultTransformDirection; }
            set { transformDirection = value; }
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

        private double? angleX;
        public double? AngleX
        {
            get { return angleX; }
            set { angleX = value; }
        }

        private double? angleY;
        public double? AngleY
        {
            get { return angleY; }
            set { angleY = value; }
        }

        private SkewTransform skew;
        public SkewTransform Skew
        {
            get { return skew; }
            protected set { skew = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            // AngleX, AngleY, CenterX, CenterY
            Skew = new SkewTransform();
            if (AngleX.HasValue)
                Skew.AngleX = AngleX.Value;
            if (AngleY.HasValue)
                Skew.AngleY = AngleY.Value;
            if (CenterX.HasValue)
                Skew.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                Skew.CenterY = CenterY.Value;

            Element.RenderTransform = Skew;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            return animation;
        }

        protected override void BeginAnimation()
        {
            var dp = TransformDirection == TransformDirection.X ?
                SkewTransform.AngleXProperty
                : SkewTransform.AngleYProperty;

            Skew.BeginAnimation(dp, Animation);
        }

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                var dp = TransformDirection == TransformDirection.X ?
                  SkewTransform.AngleXProperty
                  : SkewTransform.AngleYProperty;

                Skew.BeginAnimation(dp, null);
                AnimationWasCancelled = true;
                IsAnimating = false;
            }
        }
    }


}
