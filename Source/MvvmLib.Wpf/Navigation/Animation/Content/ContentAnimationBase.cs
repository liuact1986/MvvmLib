using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public abstract class ContentAnimationBase : IContentAnimation
    {
        private readonly double defaultFrom = 0;
        protected virtual double DefaultFrom => defaultFrom;

        private readonly double defaultTo = 0;
        protected virtual double DefaultTo => defaultTo;

        private readonly Duration defaultDuration = new Duration(TimeSpan.FromMilliseconds(300));
        protected virtual Duration DefaultDuration => defaultDuration;

        private UIElement element;
        public UIElement Element
        {
            get { return element; }
            protected set { element = value; }
        }

        private AnimationTimeline animation;
        public AnimationTimeline Animation
        {
            get { return animation; }
            protected set { animation = value; }
        }

        private Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        private Action onCancelled;
        public Action OnCancelled
        {
            get { return onCancelled; }
            set { onCancelled = value; }
        }

        private Action<IContentAnimation> onPrepare;
        public Action<IContentAnimation> OnPrepare
        {
            get { return onPrepare; }
            set { onPrepare = value; }
        }

        private bool isAnimating;
        public bool IsAnimating
        {
            get { return isAnimating; }
            protected set { isAnimating = value; }
        }

        private bool animationWasCancelled;
        public bool AnimationWasCancelled
        {
            get { return animationWasCancelled; }
            protected set { animationWasCancelled = value; }
        }

        private IEasingFunction easingFunction;
        public IEasingFunction EasingFunction
        {
            get { return easingFunction; }
            set { easingFunction = value; }
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

        protected Duration? duration;
        public Duration Duration
        {
            get { return duration ?? DefaultDuration; }
            set { duration = value; }
        }

        public abstract void CancelAnimation();

        public void Start(UIElement element, Action onCompleted)
        {
            if (IsAnimating)
            {
                animationWasCancelled = true;
                UnhandleAnimationCompleted();
                CancelAnimation();
            }
            else
            {
                //valueBeforeCancelled = null;
                animationWasCancelled = false;
            }

            this.isAnimating = true;

            this.element = element;
            OnCompleted = onCompleted;

            // create animation
            this.animation = CreateAnimation();
            // subscribe to completed
            HandleAnimationCompleted();
            // prepare
            Prepare();
            // begin animation
            BeginAnimation();

        }

        protected abstract AnimationTimeline CreateAnimation();

        protected abstract void BeginAnimation();

        public void Prepare()
        {
            OnPrepare?.Invoke(this);
        }

        public void HandleAnimationCompleted()
        {
            Animation.Completed += OnAnimationCompleted;
        }

        public void UnhandleAnimationCompleted()
        {
            Animation.Completed -= OnAnimationCompleted;
        }

        protected virtual void OnAnimationCompleted(object sender, EventArgs e)
        {
            UnhandleAnimationCompleted();
            IsAnimating = false;
            OnCompleted?.Invoke();
        }
    }

}
