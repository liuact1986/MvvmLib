using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{

    [ContentProperty("Animations")]
    public class ParallelAnimation : DependencyObject
    {
        private Storyboard storyboard;

        public ObservableCollection<ControlledAnimation> Animations
        {
            get { return (ObservableCollection<ControlledAnimation>)GetValue(AnimationsProperty); }
            set { SetValue(AnimationsProperty, value); }
        }

        public static readonly DependencyProperty AnimationsProperty =
            DependencyProperty.Register("Animations", typeof(ObservableCollection<ControlledAnimation>), typeof(ParallelAnimation), new PropertyMetadata(null));

        public Point? RenderTransformOrigin
        {
            get { return (Point?)GetValue(RenderTransformOriginProperty); }
            set { SetValue(RenderTransformOriginProperty, value); }
        }

        public static readonly DependencyProperty RenderTransformOriginProperty =
            DependencyProperty.Register("RenderTransformOrigin", typeof(Point?), typeof(ParallelAnimation), new PropertyMetadata(null));

        private ScaleTransform scaleTransform;
        public ScaleTransform ScaleTransform
        {
            get { return scaleTransform; }
        }

        private SkewTransform skewTransform;
        public SkewTransform SkewTransform
        {
            get { return skewTransform; }
        }

        private RotateTransform rotateTransform;
        public RotateTransform RotateTransform
        {
            get { return rotateTransform; }
        }

        private TranslateTransform translateTransform;
        public TranslateTransform TranslateTransform
        {
            get { return translateTransform; }
        }

        private Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        private bool isAnimating = false;
        public bool IsAnimating
        {
            get { return isAnimating; }
        }

        public ParallelAnimation()
        {
            this.Animations = new ObservableCollection<ControlledAnimation>();
        }

        public void BeginAnimation(FrameworkElement targetElement, Action onCompleted)
        {
            isAnimating = true;
            this.onCompleted = onCompleted;

            this.Prepare(targetElement);

            storyboard = new Storyboard();

            foreach (var controlledAnimation in this.Animations)
            {
                var animation = AddAnimationToStoryboard(storyboard, controlledAnimation);

                // Set Storyboard target
                SetStoryboardTarget(animation, targetElement);

                // Set Storyboard target property
                SetStoryboardTargetProperty(controlledAnimation);
            }

            HandleStoryboardCompleted();

            BeginAnimationInternal();
        }

        private void BeginAnimationInternal()
        {
            storyboard.Begin();
        }

        public void StopAnimation()
        {
            if (storyboard != null)
            {
                UnhandleStoryboardCompleted();
                storyboard.Stop();
            }
            isAnimating = false;
        }

        public void HandleStoryboardCompleted()
        {
            storyboard.Completed += OnStoryboardCompleted;
        }

        public void UnhandleStoryboardCompleted()
        {
            storyboard.Completed += OnStoryboardCompleted;
        }

        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            isAnimating = false;
            OnCompleted?.Invoke();
        }

        private AnimationTimeline AddAnimationToStoryboard(Storyboard storyboard, ControlledAnimation controlledAnimation)
        {
            var animation = controlledAnimation.Animation;
            if (animation == null)
                throw new ArgumentException("The property Animation of the controlled Animation is empty");

            storyboard.Children.Add(animation);
            return animation;
        }

        private void SetStoryboardTarget(AnimationTimeline animation, DependencyObject targetElement)
        {
            Storyboard.SetTarget(animation, targetElement);
        }

        private void SetStoryboardTargetProperty(ControlledAnimation controlledAnimation)
        {
            var animation = controlledAnimation.Animation;
            if (controlledAnimation.TargetProperty != null)
                Storyboard.SetTargetProperty(animation, new PropertyPath(controlledAnimation.TargetProperty));
            else
            {
                if (!controlledAnimation.TargetPropertyType.HasValue)
                    throw new ArgumentException("No TargetProperty or TargetPropertyType provided");

                switch (controlledAnimation.TargetPropertyType.Value)
                {
                    case AnimationTargetPropertyType.Opacity:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));
                        break;
                    case AnimationTargetPropertyType.ScaleX:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                        break;
                    case AnimationTargetPropertyType.ScaleY:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        break;
                    case AnimationTargetPropertyType.SkewX:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(SkewTransform.AngleX)"));
                        break;
                    case AnimationTargetPropertyType.SkewY:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(SkewTransform.AngleY)"));
                        break;
                    case AnimationTargetPropertyType.Rotate:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
                        break;
                    case AnimationTargetPropertyType.TranslateX:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
                        break;
                    case AnimationTargetPropertyType.TranslateY:
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
                        break;
                }
            }
        }

        private void Prepare(FrameworkElement element)
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-set-a-property-after-animating-it-with-a-storyboard
            element.BeginAnimation(UIElement.OpacityProperty, null);
            element.BeginAnimation(UIElement.RenderTransformProperty, null);
            element.Opacity = 1.0;

            scaleTransform = new ScaleTransform();
            skewTransform = new SkewTransform();
            rotateTransform = new RotateTransform();
            translateTransform = new TranslateTransform();
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
            element.RenderTransform = transformGroup;
            if (RenderTransformOrigin.HasValue)
                element.RenderTransformOrigin = RenderTransformOrigin.Value;
        }
    }

}
