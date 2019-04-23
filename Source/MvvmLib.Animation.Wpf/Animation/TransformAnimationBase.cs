using System.Windows;
using System.Windows.Media;

namespace MvvmLib.Animation
{
    public abstract class TransformAnimationBase : ContentAnimationBase
    {
        private readonly Point defaultRenderTransformOrigin = new Point(0.5, 0.5);
        protected virtual Point DefaultRenderTransformOrigin => defaultRenderTransformOrigin;

        protected Point? renderTransformOrigin;
        public Point RenderTransformOrigin
        {
            get { return renderTransformOrigin ?? DefaultRenderTransformOrigin; }
            set { renderTransformOrigin = value; }
        }

        protected ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
        }

        protected SkewTransform skewTransform;
        public SkewTransform SkewTransform
        {
            get { return skewTransform; }
        }

        protected RotateTransform rotateTransform;
        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
        }

        protected TranslateTransform translateTransform;
        public TranslateTransform TranslateTransform
        {
            get { return translateTransform; }
        }

        protected override void Init()
        {
            scaleTransform = new ScaleTransform();
            skewTransform = new SkewTransform();
            rotateTransform = new RotateTransform();
            translateTransform = new TranslateTransform();
            InitTransformProperties();
            var transformGroup = new TransformGroup
            {
                Children =
                {
                    scaleTransform,
                    skewTransform,
                    rotateTransform,
                    translateTransform
                }
            };
            Element.RenderTransform = transformGroup;
            Element.RenderTransformOrigin = RenderTransformOrigin;
        }

        protected virtual void InitTransformProperties()
        {

        }
    }


}
