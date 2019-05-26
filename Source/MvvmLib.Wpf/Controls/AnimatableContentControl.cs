using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    /// <summary>
    /// A <see cref="ContentControl"/> that allows to animate on content change.
    /// </summary>
    public class AnimatableContentControl : ContentControl
    {
        private const string MainGridPartName = "PART_MainGrid";
        private const string PreviousContentPresenterPartName = "PreviousContentPresenter";
        private const string CurrentContentPresenterPartName = "CurrentContentPresenter";
        private const string EntranceAnimationStoryboardName = "EntranceAnimationStoryboard";
        private const string ExitAnimationStoryboardName = "ExitAnimationStoryboard";
        private Grid mainGrid;
        private ContentPresenter currentContentPresenter;
        private ContentPresenter previousContentPresenter;
        private Queue<AnimationQueueItem> queue;
        private bool handleContentChanged;
        private StoryboardAccessor entranceStoryboardAccessor;
        private StoryboardAccessor exitStoryboardAccessor;
        private bool hasApplyTemplate;

        /// <summary>
        /// Checks if the control is animating.
        /// </summary>
        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            private set { SetValue(IsAnimatingProperty, value); }
        }

        private static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.Register("IsAnimating", typeof(bool), typeof(AnimatableContentControl), new PropertyMetadata(false));

        /// <summary>
        /// The entrance animation Storyboard.
        /// </summary>
        public Storyboard EntranceAnimation
        {
            get { return (Storyboard)GetValue(EntranceAnimationProperty); }
            set { SetValue(EntranceAnimationProperty, value); }
        }

        /// <summary>
        /// The entrance animation Storyboard.
        /// </summary>
        public static readonly DependencyProperty EntranceAnimationProperty =
            DependencyProperty.Register("EntranceAnimation", typeof(Storyboard), typeof(AnimatableContentControl), new PropertyMetadata(null, OnEntranceAnimationChanged));

        private static void OnEntranceAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animatableContentControl = d as AnimatableContentControl;
            if (animatableContentControl.IsLoaded)
                animatableContentControl.SetEntranceAnimationResource();
        }

        /// <summary>
        /// The exit animation Storyboard.
        /// </summary>
        public Storyboard ExitAnimation
        {
            get { return (Storyboard)GetValue(ExitAnimationProperty); }
            set { SetValue(ExitAnimationProperty, value); }
        }

        /// <summary>
        /// The exit animation Storyboard.
        /// </summary>
        public static readonly DependencyProperty ExitAnimationProperty =
            DependencyProperty.Register("ExitAnimation", typeof(Storyboard), typeof(AnimatableContentControl), new PropertyMetadata(null, OnExitAnimationChanged));

        private static void OnExitAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animatableContentControl = d as AnimatableContentControl;
            if (animatableContentControl.IsLoaded)
                animatableContentControl.SetExitAnimationResource();
        }

        /// <summary>
        /// Allows to play simultaneously the entrance and exit animations. 
        /// The oldcontent is added to the previous content presenter and the new content to the current content presenter.
        /// </summary>
        public bool Simultaneous
        {
            get { return (bool)GetValue(SimultaneousProperty); }
            set { SetValue(SimultaneousProperty, value); }
        }

        /// <summary>
        /// Allows to play simultaneously the entrance and exit animations. 
        /// The oldcontent is added to the previous content presenter and the new content to the current content presenter.
        /// </summary>
        public static readonly DependencyProperty SimultaneousProperty =
            DependencyProperty.Register("Simultaneous", typeof(bool), typeof(AnimatableContentControl), new PropertyMetadata(false));

        /// <summary>
        /// Allows to cancel the animation queue.
        /// </summary>
        public bool IsCancelled
        {
            get { return (bool)GetValue(IsCancelledProperty); }
            set { SetValue(IsCancelledProperty, value); }
        }

        /// <summary>
        /// Allows to cancel the animation queue.
        /// </summary>
        public static readonly DependencyProperty IsCancelledProperty =
            DependencyProperty.Register("IsCancelled", typeof(bool), typeof(AnimatableContentControl), new PropertyMetadata(false, OnIsCancelledChanged));

        private static void OnIsCancelledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animatableContentControl = d as AnimatableContentControl;
            var isCancelled = (bool)e.NewValue;
            if (isCancelled)
                animatableContentControl.CancelAnimations();
        }

        /// <summary>
        /// Invoked on animation completed.
        /// </summary>
        public event EventHandler AnimationCompleted;

        /// <summary>
        /// Invoked on animation cancelled with <see cref="IsCancelled"/> or <see cref="CancelAnimations"/>.
        /// </summary>
        public event EventHandler AnimationCancelled;

        static AnimatableContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatableContentControl), new FrameworkPropertyMetadata(typeof(AnimatableContentControl)));
        }

        /// <summary>
        /// Creates the animatable content control.
        /// </summary>
        public AnimatableContentControl()
        {
            this.queue = new Queue<AnimationQueueItem>();
            this.handleContentChanged = true;
        }

        /// <summary>
        /// Apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (hasApplyTemplate)
                return;

            base.OnApplyTemplate();

            this.mainGrid = this.GetTemplateChild(MainGridPartName) as Grid;
            this.previousContentPresenter = this.GetTemplateChild(PreviousContentPresenterPartName) as ContentPresenter;
            this.currentContentPresenter = this.GetTemplateChild(CurrentContentPresenterPartName) as ContentPresenter;

            SetEntranceAnimationResource();
            SetExitAnimationResource();

            hasApplyTemplate = true;
        }

        /// <summary>
        /// Invoked on content changed.
        /// </summary>
        /// <param name="oldContent">The old content</param>
        /// <param name="newContent">The new content</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (!hasApplyTemplate)
                ApplyTemplate();

            if (handleContentChanged)
                this.RunOrEnqueue(oldContent, newContent);
        }

        private void SetEntranceAnimationResource()
        {
            if (EntranceAnimation != null)
                this.mainGrid.Resources[EntranceAnimationStoryboardName] = EntranceAnimation;
        }

        private void SetExitAnimationResource()
        {
            if (ExitAnimation != null)
                this.mainGrid.Resources[ExitAnimationStoryboardName] = ExitAnimation;
        }
        private Storyboard GetEntranceAnimationStoryboardInResources()
        {
            var storyboard = this.mainGrid.Resources[EntranceAnimationStoryboardName] as Storyboard;
            return storyboard;
        }

        private Storyboard GetExitAnimationStoryboardInResources()
        {
            var storyboard = this.mainGrid.Resources[ExitAnimationStoryboardName] as Storyboard;
            return storyboard;
        }

        private void DoLeave(Action onLeaveCompleted)
        {
            if (ExitAnimation != null)
            {
                var storyboard = GetExitAnimationStoryboardInResources();
                exitStoryboardAccessor = new StoryboardAccessor(storyboard);
                exitStoryboardAccessor.HandleCompleted(() =>
                {
                    exitStoryboardAccessor.UnhandleCompleted();
                    onLeaveCompleted();
                });

                storyboard.Begin(mainGrid, true);
            }
            else
                onLeaveCompleted();
        }

        private void DoEnter(Action onEnterCompleted)
        {
            if (EntranceAnimation != null)
            {
                var storyboard = GetEntranceAnimationStoryboardInResources();
                entranceStoryboardAccessor = new StoryboardAccessor(storyboard);
                entranceStoryboardAccessor.HandleCompleted(() =>
                {
                    entranceStoryboardAccessor.UnhandleCompleted();
                    onEnterCompleted();
                });
                storyboard.Begin(mainGrid, true);
            }
            else
                onEnterCompleted();
        }

        private void OnCompleted()
        {
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void OnCancelled()
        {
            AnimationCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeContentAndAnimate(object oldContent, object newContent)
        {
            if (Simultaneous)
            {
                int i = 0;
                previousContentPresenter.Content = oldContent;
                previousContentPresenter.Visibility = Visibility.Visible;
                // animate previous content presenter
                DoLeave(() =>
                {
                    previousContentPresenter.Visibility = Visibility.Collapsed;
                    i++;
                    if (i == 2)
                        DequeueInternal();
                });

                currentContentPresenter.Content = newContent;
                // animate current content presenter
                DoEnter(() =>
                {
                    i++;
                    if (i == 2)
                        DequeueInternal();
                });
            }
            else
            {
                if (oldContent != null)
                {

                    // animate current content presenter
                    DoLeave(() =>
                    {
                        previousContentPresenter.Content = oldContent;
                        previousContentPresenter.Visibility = Visibility.Collapsed;

                        currentContentPresenter.Content = newContent;

                        // animate current content presenter
                        DoEnter(() => DequeueInternal());
                    });
                }
                else
                {
                    previousContentPresenter.Content = oldContent;
                    previousContentPresenter.Visibility = Visibility.Collapsed;

                    currentContentPresenter.Content = newContent;
                    // animate current content presenter
                    DoEnter(() => DequeueInternal());
                }
            }
        }

        private void DequeueInternal()
        {
            if (queue.Count > 0)
            {
                var queueItem = queue.Dequeue();
                ChangeContentAndAnimate(queueItem.OldContent, queueItem.NewContent);
            }
            else
            {
                IsAnimating = false;
                OnCompleted();
            }
        }

        private void RunOrEnqueue(object oldContent, object newContent)
        {

            if (IsAnimating)
                queue.Enqueue(new AnimationQueueItem(oldContent, newContent));
            else
            {
                IsAnimating = true;
                this.ChangeContentAndAnimate(oldContent, newContent);
            }
        }

        /// <summary>
        /// Run the animation and change the content.
        /// </summary>
        /// <param name="newContent">The new content</param>
        public void Run(object newContent)
        {
            this.RunOrEnqueue(currentContentPresenter.Content, newContent);
        }

        private void ResetInternal(UIElement element)
        {
            this.CancelAnimations();

            // How to: Set a Property After Animating It with a Storyboard
            // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-set-a-property-after-animating-it-with-a-storyboard
            element.BeginAnimation(UIElement.OpacityProperty, null);
            element.BeginAnimation(UIElement.RenderTransformProperty, null);
            element.Opacity = 1.0;
            var group = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform(),
                    new SkewTransform(),
                    new RotateTransform(),
                    new TranslateTransform()
                }
            };
            element.RenderTransform = group;
        }

        /// <summary>
        /// Resets the render transform, opacity and cancel animations. 
        /// </summary>
        public void Reset()
        {
            this.ResetInternal(previousContentPresenter);
            this.ResetInternal(currentContentPresenter);
        }

        /// <summary>
        /// Cancel animation, clear the queue and go to last content without animation.
        /// </summary>
        public void CancelAnimations()
        {
            if (IsAnimating)
            {
                if (exitStoryboardAccessor != null)
                    exitStoryboardAccessor.Storyboard.Stop(mainGrid);
                if (entranceStoryboardAccessor != null)
                    entranceStoryboardAccessor.Storyboard.Stop(mainGrid);

                // set content
                if (queue.Count > 0)
                {
                    var lastQueueItem = queue.LastOrDefault();

                    previousContentPresenter.Content = lastQueueItem.OldContent;
                    previousContentPresenter.Visibility = Visibility.Collapsed;
                    currentContentPresenter.Content = lastQueueItem.NewContent;

                    queue.Clear();
                }
                IsAnimating = false;
            }
            OnCancelled();
        }
    }

    internal class AnimationQueueItem
    {
        private object oldContent;
        public object OldContent
        {
            get { return oldContent; }
        }

        private object newContent;
        public object NewContent
        {
            get { return newContent; }
        }

        public AnimationQueueItem(object oldContent, object newContent)
        {
            this.oldContent = oldContent;
            this.newContent = newContent;
        }
    }

}
