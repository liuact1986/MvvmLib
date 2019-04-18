using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationViewModel : ILoadedEventListener
    {
        private TranslateAnimation entranceTranslateAnimation;
        private TranslateAnimation exitTranslateAnimation;

        public ICommand GoViewAWithFadeAnimationCommand { get; }
        public ICommand GoViewBWithFadeAnimationCommand { get; }

        public ICommand GoViewAWithFxCornerAnimationCommand { get; }
        public ICommand GoViewBWithFxCornerAnimationCommand { get; }

        public ICommand GoViewAWithRotateAnimationCommand { get; }
        public ICommand GoViewBWithRotateAnimationCommand { get; }

        public ICommand GoViewAWithScaleAnimationCommand { get; }
        public ICommand GoViewBWithScaleAnimationCommand { get; }

        public ICommand GoViewAWithTranslateAnimationCommand { get; }
        public ICommand GoViewBWithTranslateAnimationCommand { get; }

        public ICommand GoViewAWithSkewAnimationCommand { get; }
        public ICommand GoViewBWithSkewAnimationCommand { get; }

        public AnimationViewModel(IRegionManager regionManager)
        {
            var entranceOpacityAnimation = new OpacityAnimation { From = 0, To = 1 };
            var exitOpacityAnimation = new OpacityAnimation { From = 1, To = 0 };

            var entranceFxCornerAnimation = new FXCornerEntranceAnimation();
            var exitFxCornerAnimation = new FXCornerExitAnimation();

            var entranceRotateAnimation = new RotateAnimation { From = 0, To = 360, RenderTransformOrigin = new Point(0.5, 0.5) };
            var exitRotateAnimation = new RotateAnimation { From = 360, To = 0, RenderTransformOrigin = new Point(0.5, 0.5) };

            var entranceScaleAnimation = new ScaleAnimation
            {
                From = 0,
                To = 1,
                RenderTransformOrigin = new Point(0.5, 0.5),
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };
            var exitScaleAnimation = new ScaleAnimation
            {
                From = 1,
                To = 0,
                RenderTransformOrigin = new Point(0.5, 0.5),
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };

            entranceTranslateAnimation = new TranslateAnimation { From = 800, To = 0 };
            exitTranslateAnimation = new TranslateAnimation { From = 0, To = 800 };

            var entranceSkewAnimation = new SkewAnimation { From = 30, To = 0, TransformDirection = TransformDirection.Y };
            var exitSkewAnimation = new SkewAnimation { From = 0, To = 30, TransformDirection = TransformDirection.Y };

            var region = regionManager.GetContentRegion("AnimationSample");

            GoViewAWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceOpacityAnimation, exitOpacityAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceOpacityAnimation, exitOpacityAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceFxCornerAnimation, exitFxCornerAnimation, true);
                await region.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceFxCornerAnimation, exitFxCornerAnimation, true);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceRotateAnimation, exitRotateAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceRotateAnimation, exitRotateAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceScaleAnimation, exitScaleAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceScaleAnimation, exitScaleAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceTranslateAnimation, exitTranslateAnimation, true);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceTranslateAnimation, exitTranslateAnimation, true);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceSkewAnimation, exitSkewAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(entranceSkewAnimation, exitSkewAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });
        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            view.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = e.NewSize.Width;
            entranceTranslateAnimation.From = width;
            exitTranslateAnimation.To = width;
        }
    }

    // 1. inherit from ContentAnimationBase or TransformAnimationBase
    public class MyAnimation : TransformAnimationBase
    {
        // 2. implements the base class
        public override void CancelAnimation()
        {
            throw new System.NotImplementedException();
        }

        protected override void BeginAnimation()
        {
            throw new System.NotImplementedException();
        }

        protected override AnimationTimeline CreateAnimation()
        {
            throw new System.NotImplementedException();
        }
    }
}
