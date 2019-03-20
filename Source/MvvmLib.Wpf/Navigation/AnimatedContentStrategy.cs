using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{
    public class AnimatedContentStrategy : IAnimatedContentStrategy
    {
        IAnimationService animationService;

        public int DefaultAnimationDuration
        {
            get { return this.animationService.DefaultAnimationDuration; }
            set { this.animationService.DefaultAnimationDuration = value; }
        }

        public IEasingFunction DefaultEaseFunction
        {
            get { return this.animationService.DefaultEaseFunction; }
            set { this.animationService.DefaultEaseFunction = value; }
        }

        public double DefaultFromLeftValue
        {
            get { return this.animationService.DefaultFromLeftValue; }
            set { this.animationService.DefaultFromLeftValue = value; }
        }

        public double DefaultFromRightValue
        {
            get { return this.animationService.DefaultFromRightValue; }
            set { this.animationService.DefaultFromRightValue = value; }
        }

        public double DefaultFromTopValue
        {
            get { return this.animationService.DefaultFromTopValue; }
            set { this.animationService.DefaultFromTopValue = value; }
        }

        public double DefaultFromBottomValue
        {
            get { return this.animationService.DefaultFromBottomValue; }
            set { this.animationService.DefaultFromBottomValue = value; }
        }

        public double DefaultToLeftValue
        {
            get { return this.animationService.DefaultToLeftValue; }
            set { this.animationService.DefaultToLeftValue = value; }
        }

        public double DefaultToRightValue
        {
            get { return this.animationService.DefaultToRightValue; }
            set { this.animationService.DefaultToRightValue = value; }
        }

        public double DefaultToTopValue
        {
            get { return this.animationService.DefaultToTopValue; }
            set { this.animationService.DefaultToTopValue = value; }
        }

        public double DefaultToBottomValue
        {
            get { return this.animationService.DefaultToBottomValue; }
            set { this.animationService.DefaultToBottomValue = value; }
        }

        public AnimatedContentStrategy()
            : this(new AnimationService())
        { }

        public AnimatedContentStrategy(IAnimationService animationService)
        {
            this.animationService = animationService;
        }

        public void PreventAfterAnimation(FrameworkElement view)
        {
            view.Opacity = 1;
            view.RenderTransform = new TranslateTransform(0, 0);
        }

        public void OnEnter(FrameworkElement view, Action setContentCallback, EntranceTransitionType entranceTransitionType, Action cb = null)
        {
            if (view != null)
            {
                this.PreventAfterAnimation(view);
            }

            if (view == null || entranceTransitionType == EntranceTransitionType.None)
            {
                setContentCallback();
                cb?.Invoke();
            }
            else if (entranceTransitionType == EntranceTransitionType.FadeIn)
            {
                this.animationService.FadeIn(view, setContentCallback, (s, e) =>
                {
                    cb?.Invoke();
                });
            }
            else if (entranceTransitionType == EntranceTransitionType.SlideInFromLeft)
            {
                this.animationService.SlideInFromLeft(view, setContentCallback, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (entranceTransitionType == EntranceTransitionType.SlideInFromRight)
            {
                this.animationService.SlideInFromRight(view, setContentCallback, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (entranceTransitionType == EntranceTransitionType.SlideInFromBottom)
            {
                this.animationService.SlideInFromBottom(view, setContentCallback, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (entranceTransitionType == EntranceTransitionType.SlideInFromTop)
            {
                this.animationService.SlideInFromTop(view, setContentCallback, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
        }

        public void OnLeave(FrameworkElement view, ExitTransitionType exitTransitionType, Action cb = null)
        {
            if (view == null || exitTransitionType == ExitTransitionType.None)
            {
                cb?.Invoke();
            }
            else if (exitTransitionType == ExitTransitionType.FadeOut)
            {
                this.animationService.FadeOut(view, (s, e) =>
                 {
                    cb?.Invoke();
                 });
            }
            else if (exitTransitionType == ExitTransitionType.SlideOutToLeft)
            {
                this.animationService.SlideOutToLeft(view, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (exitTransitionType == ExitTransitionType.SlideOutToRight)
            {
                this.animationService.SlideOutToRight(view, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (exitTransitionType == ExitTransitionType.SlideOutToBottom)
            {
                this.animationService.SlideOutToBottom(view, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
            else if (exitTransitionType == ExitTransitionType.SlideOutToTop)
            {
                this.animationService.SlideOutToTop(view, (s, e) =>
                {
                   cb?.Invoke();
                });
            }
        }

    }

}