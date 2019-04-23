using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
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

        protected override void InitTransformProperties()
        {
            if (AngleX.HasValue)
                SkewTransform.AngleX = AngleX.Value;
            if (AngleY.HasValue)
                SkewTransform.AngleY = AngleY.Value;
            if (CenterX.HasValue)
                SkewTransform.CenterX = CenterX.Value;
            if (CenterY.HasValue)
                SkewTransform.CenterY = CenterY.Value;
        }

        protected override void SetTargetProperty(AnimationTimeline animation, int index)
        {
            var path = TransformDirection == TransformDirection.X ?
               "(UIElement.RenderTransform).(TransformGroup.Children)[1].(SkewTransform.AngleX)"
               : "(UIElement.RenderTransform).(TransformGroup.Children)[1].(SkewTransform.AngleY)";

            Storyboard.SetTargetProperty(animation, new PropertyPath(path));
        }
    }


}
