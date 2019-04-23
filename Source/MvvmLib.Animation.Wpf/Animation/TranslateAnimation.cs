using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class TranslateAnimation : TransformAnimationBase
    {
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

        protected override void InitTransformProperties()
        {
            if (X.HasValue)
                translateTransform.X = X.Value;
            if (Y.HasValue)
                translateTransform.Y = Y.Value;
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            var path = TransformDirection == TransformDirection.X ? "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"
              : "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)";

            Storyboard.SetTargetProperty(animation, new PropertyPath(path));
        }
    }


}
