using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class TranslateAnimation : TransformAnimationBase
    {
        protected override double DefaultFrom => 0;
        protected override double DefaultTo => 30;

        private readonly TransformDirection defaultTransformDirection = TransformDirection.X;
        protected virtual TransformDirection DefaultTransformDirection => defaultTransformDirection;

        protected TransformDirection? transformDirection;
        public TransformDirection TransformDirection
        {
            get { return transformDirection ?? DefaultTransformDirection; }
            set { transformDirection = value; }
        }

        private double? x;
        public double? X
        {
            get { return x; }
            set { x = value; }
        }

        private double? y;
        public double? Y
        {
            get { return y; }
            set { y = value; }
        }

        private TranslateTransform translate;
        public TranslateTransform Translate
        {
            get { return translate; }
            protected set { translate = value; }
        }

        protected override AnimationTimeline CreateAnimation()
        {
            Translate = new TranslateTransform();
            if (X.HasValue)
                Translate.X = X.Value;
            if (Y.HasValue)
                Translate.Y = Y.Value;
            Element.RenderTransform = Translate;
            Element.RenderTransformOrigin = RenderTransformOrigin;

            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;

            return animation;
        }

        protected override void BeginAnimation()
        {
            var dp = TransformDirection == TransformDirection.X ?
                TranslateTransform.XProperty
                : TranslateTransform.YProperty;

            Translate.BeginAnimation(dp, Animation);
        }

        public override void CancelAnimation()
        {
            if (Element != null)
            {
                var dp = TransformDirection == TransformDirection.X ?
                    TranslateTransform.XProperty
                    : TranslateTransform.YProperty;

                Translate.BeginAnimation(dp, null);
                AnimationWasCancelled = true;
                IsAnimating = false;
            }
        }
    }

}
