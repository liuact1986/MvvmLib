using System.Windows;

namespace MvvmLib.Navigation
{
    public abstract class TransformAnimationBase : ContentAnimationBase
    {
        private readonly Point defaultRenderTransformOrigin = new Point(0, 0);
        protected virtual Point DefaultRenderTransformOrigin => defaultRenderTransformOrigin;

        protected Point? renderTransformOrigin;
        public Point RenderTransformOrigin
        {
            get { return renderTransformOrigin ?? DefaultRenderTransformOrigin; }
            set { renderTransformOrigin = value; }
        }

    }



}
