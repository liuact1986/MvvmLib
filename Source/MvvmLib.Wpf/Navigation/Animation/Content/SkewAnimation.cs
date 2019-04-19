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

        private SkewTransform skewTransform;
        public SkewTransform SkewTransform
        {
            get { return skewTransform; }
            protected set { skewTransform = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            // AngleX, AngleY, CenterX, CenterY
            SkewTransform = new SkewTransform();
            if (AngleX.HasValue)
                SkewTransform.AngleX = AngleX.Value;
            if (AngleY.HasValue)
                SkewTransform.AngleY = AngleY.Value;
            if (CenterX.HasValue)
                SkewTransform.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                SkewTransform.CenterY = CenterY.Value;

            Element.RenderTransform = SkewTransform;
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

            SkewTransform.BeginAnimation(dp, Animation);
        }

        public override void CancelAnimation()
        {
            if (SkewTransform != null)
            {
                var dp = TransformDirection == TransformDirection.X ?
                  SkewTransform.AngleXProperty
                  : SkewTransform.AngleYProperty;

                SkewTransform.BeginAnimation(dp, null);
            }
        }
    }


}
