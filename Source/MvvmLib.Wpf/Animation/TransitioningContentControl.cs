using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    /// <summary>
    /// Allows to play animation on load (Entrance animation) and explicitly on leave (Exit animation), for example on click on a button or item to remove the item from a list.
    /// </summary>
    public class TransitioningContentControl : ContentControl
    {
        private const string MainGridPartName = "PART_MainGrid";
        private const string ContentPresenterPartName = "ContentPresenter";
        private const string EntranceTransitionStoryboardName = "EntranceTransitionStoryboard";
        private const string ExitTransitionStoryboardName = "ExitTransitionStoryboard";
        private Grid mainGrid;
        private ContentPresenter contentPresenter;
        private StoryboardAccessor entranceStoryboardAccessor;
        private StoryboardAccessor exitStoryboardAccessor;

        /// <summary>
        /// The entrance transition Storyboard.
        /// </summary>
        public Storyboard EntranceTransition
        {
            get { return (Storyboard)GetValue(EntranceTransitionProperty); }
            set { SetValue(EntranceTransitionProperty, value); }
        }

        /// <summary>
        /// The entrance transition Storyboard.
        /// </summary>
        public static readonly DependencyProperty EntranceTransitionProperty =
            DependencyProperty.Register("EntranceTransition", typeof(Storyboard), typeof(TransitioningContentControl), new PropertyMetadata(null, OnEntranceTransitionChanged));

        private static void OnEntranceTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = d as TransitioningContentControl;
            if (transitioningContentControl.IsLoaded)
                transitioningContentControl.SetEntranceTransitionResource();
        }

        /// <summary>
        /// The exit transition Storyboard.
        /// </summary>
        public Storyboard ExitTransition
        {
            get { return (Storyboard)GetValue(ExitTransitionProperty); }
            set { SetValue(ExitTransitionProperty, value); }
        }

        /// <summary>
        /// The exit animation Storyboard.
        /// </summary>
        public static readonly DependencyProperty ExitTransitionProperty =
            DependencyProperty.Register("ExitTransition", typeof(Storyboard), typeof(TransitioningContentControl), new PropertyMetadata(null, OnExitTransitionChanged));

        private static void OnExitTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = d as TransitioningContentControl;
            if (transitioningContentControl.IsLoaded)
                transitioningContentControl.SetExitTransitionResource();
        }

        /// <summary>
        /// Allows to cancel the transition.
        /// </summary>
        public bool IsCancelled
        {
            get { return (bool)GetValue(IsCancelledProperty); }
            set { SetValue(IsCancelledProperty, value); }
        }

        /// <summary>
        /// Allows to cancel the transition.
        /// </summary>
        public static readonly DependencyProperty IsCancelledProperty =
            DependencyProperty.Register("IsCancelled", typeof(bool), typeof(TransitioningContentControl), new PropertyMetadata(false, OnIsCancelledChanged));

        private static void OnIsCancelledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = d as TransitioningContentControl;
            var isCancelled = (bool)e.NewValue;
            if (isCancelled)
                transitioningContentControl.CancelTransition();
        }

        /// <summary>
        /// Allows to play the exit transition.
        /// </summary>
        public bool IsLeaving
        {
            get { return (bool)GetValue(IsLeavingProperty); }
            set { SetValue(IsLeavingProperty, value); }
        }

        /// <summary>
        /// Allows to play the exit transition.
        /// </summary>
        public static readonly DependencyProperty IsLeavingProperty =
            DependencyProperty.Register("IsLeaving", typeof(bool), typeof(TransitioningContentControl), new PropertyMetadata(false, OnIsLeavingChanged));

        private static void OnIsLeavingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = d as TransitioningContentControl;
            var isLeaving = (bool)e.NewValue;
            if (isLeaving)
                transitioningContentControl.DoLeave();
        }

        /// <summary>
        /// Invoked on transition completed.
        /// </summary>
        public event EventHandler TransitionCompleted;

        /// <summary>
        /// Invoked on transition cancelled.
        /// </summary>
        public event EventHandler TransitionCancelled;

        static TransitioningContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningContentControl), new FrameworkPropertyMetadata(typeof(TransitioningContentControl)));
        }

        /// <summary>
        /// Creates the transtioning content control.
        /// </summary>
        public TransitioningContentControl()
        {
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mainGrid = this.GetTemplateChild(MainGridPartName) as Grid;
            this.contentPresenter = this.GetTemplateChild(ContentPresenterPartName) as ContentPresenter;

            SetEntranceTransitionResource();
            SetExitTransitionResource();
        }

        private void SetEntranceTransitionResource()
        {
            if (EntranceTransition != null)
                this.mainGrid.Resources[EntranceTransitionStoryboardName] = EntranceTransition;
        }

        private void SetExitTransitionResource()
        {
            if (ExitTransition != null)
                this.mainGrid.Resources[ExitTransitionStoryboardName] = ExitTransition;
        }

        private Storyboard GetEntranceTransitionStoryboardInResources()
        {
            var storyboard = this.mainGrid.Resources[EntranceTransitionStoryboardName] as Storyboard;
            return storyboard;
        }

        private Storyboard GetExitTransitionStoryboardInResources()
        {
            var storyboard = this.mainGrid.Resources[ExitTransitionStoryboardName] as Storyboard;
            return storyboard;
        }

        private void OnTransitionCompleted()
        {
            TransitionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void OnTransitionCancelled()
        {
            TransitionCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DoEnter();
        }

        private void ResetInternal(UIElement element)
        {
            this.CancelTransition();

            // How to: Set a Property After Animating It with a Storyboard
            // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-set-a-property-after-animating-it-with-a-storyboard
            contentPresenter.BeginAnimation(UIElement.OpacityProperty, null);
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
        /// Resets the render transform, opacity and cancel transition. 
        /// </summary>
        public void Reset()
        {
            this.ResetInternal(contentPresenter);
        }

        /// <summary>
        /// Plays the exit transition.
        /// </summary>
        public void DoLeave()
        {
            if (ExitTransition != null)
            {
                var storyboard = GetExitTransitionStoryboardInResources();
                exitStoryboardAccessor = new StoryboardAccessor(storyboard);
                exitStoryboardAccessor.HandleCompleted(() =>
                {
                    exitStoryboardAccessor.UnhandleCompleted();
                    OnTransitionCompleted();
                });

                storyboard.Begin(mainGrid, true);
            }
            else
                OnTransitionCompleted();
        }

        /// <summary>
        /// Plays the entrance transition.
        /// </summary>
        public void DoEnter()
        {
            if (EntranceTransition != null)
            {
                var storyboard = GetEntranceTransitionStoryboardInResources();
                entranceStoryboardAccessor = new StoryboardAccessor(storyboard);
                entranceStoryboardAccessor.HandleCompleted(() =>
                {
                    entranceStoryboardAccessor.UnhandleCompleted();
                    OnTransitionCompleted();
                });
                storyboard.Begin(mainGrid, true);
            }
            else
                OnTransitionCompleted();
        }

        /// <summary>
        /// Cancels the transition.
        /// </summary>
        public void CancelTransition()
        {
            if (exitStoryboardAccessor != null)
                exitStoryboardAccessor.Storyboard.Stop(mainGrid);
            if (entranceStoryboardAccessor != null)
                entranceStoryboardAccessor.Storyboard.Stop(mainGrid);

            OnTransitionCancelled();
        }

    }
}
