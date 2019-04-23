using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{

    public abstract class ContentAnimationBase : IContentAnimation
    {
        private readonly double defaultFrom = 0;
        protected virtual double DefaultFrom { get { return defaultFrom; } }
        private readonly double defaultTo = 0;
        protected virtual double DefaultTo { get { return defaultTo; } }
        private readonly Duration defaultDuration = new Duration(TimeSpan.FromMilliseconds(300));
        protected virtual Duration DefaultDuration {get {return defaultDuration;}}

        protected UIElement element;
        public UIElement Element
        {
            get { return element; }
            set { element = value; }
        }

        protected double? from;
        public double From
        {
            get { return from ?? DefaultFrom; }
            set { from = value; }
        }

        protected double? to;
        public double To
        {
            get { return to ?? DefaultTo; }
            set { to = value; }
        }

        protected IEasingFunction easingFunction;
        public IEasingFunction EasingFunction
        {
            get { return easingFunction; }
            set { easingFunction = value; }
        }

        protected Duration? duration;
        public Duration Duration
        {
            get { return duration ?? DefaultDuration; }
            set { duration = value; }
        }

        protected AnimationTimeline[] animations;
        public AnimationTimeline[] Animations
        {
            get { return animations; }
        }

        protected Storyboard storyboard;
        public Storyboard Storyboard
        {
            get { return storyboard; }
        }

        protected Action<ContentAnimationBase> onPrepare;
        public Action<ContentAnimationBase> OnPrepare
        {
            get { return onPrepare; }
            set { onPrepare = value; }
        }

        protected Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        protected Action onCancelled;
        public Action OnCancelled
        {
            get { return onCancelled; }
            set { onCancelled = value; }
        }


        protected bool isAnimating;
        public bool IsAnimating
        {
            get { return isAnimating; }
        }

        public virtual void Run(UIElement element, Action onCompleted, Action onCancelled)
        {
            this.element = element;
            this.onCompleted = onCompleted;
            this.onCancelled = onCancelled;

            isAnimating = true;

            CheckValues();
            Init();

            var animations = this.CreateAnimations();
            this.animations = animations;

            storyboard = new Storyboard();

            foreach (var animation in this.animations)
                storyboard.Children.Add(animation);

            // prepare set content
            Prepare();

            HandleCompleted();

            PlayAnimations();
        }

        public virtual void Run(UIElement element, Action onCompleted)
        {
            this.Run(element, onCompleted, this.onCancelled);
        }

        public virtual void Run(UIElement element)
        {
            this.Run(element, this.onCompleted, this.onCancelled);
        }

        protected virtual void Init()
        {

        }

        protected virtual void CheckValues()
        {

        }

        protected virtual AnimationTimeline[] CreateAnimations()
        {
            var animation = new DoubleAnimation(From, To, Duration);
            if (EasingFunction != null)
                animation.EasingFunction = EasingFunction;
            return new AnimationTimeline[] { animation };
        }

        public void HandleCompleted()
        {
            storyboard.Completed += OnStoryboardCompleted;
        }

        public void UnhandleCompleted()
        {
            storyboard.Completed -= OnStoryboardCompleted;
        }

        protected virtual void OnStoryboardCompleted(object sender, EventArgs e)
        {
            OnCompleted?.Invoke();
            isAnimating = false;
        }

        public virtual void CancelAnimations()
        {
            isAnimating = false;
            if (storyboard != null)
                Storyboard.Stop();
            OnCancelled?.Invoke();
        }

        protected void Prepare()
        {
            OnPrepare?.Invoke(this);
        }

        protected virtual void SetTarget(AnimationTimeline animation, int index)
        {
            Storyboard.SetTarget(animation, element);
        }

        protected abstract void SetTargetProperty(AnimationTimeline animation, int index);

        protected virtual void PlayAnimations()
        {
            for (int index = 0; index < animations.Length; index++)
            {
                var animation = animations[index];
                SetTarget(animation, index);
                SetTargetProperty(animation, index);
            }

            storyboard.Begin();
        }
    }

}
