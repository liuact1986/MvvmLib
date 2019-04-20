using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationViewModel : ILoadedEventListener , ISelectable
    {
        private TranslateAnimation translateEntranceAnimation;
        private TranslateAnimation translateExitAnimation;

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

        public ICommand GoViewAWithFallAnimationCommand { get; }
        public ICommand GoViewBWithFallAnimationCommand { get; }

        public ICommand GoViewAWithFxVscaleAnimationCommand { get; }
        public ICommand GoViewBWithFxVscaleAnimationCommand { get; }

        public AnimationViewModel(IRegionNavigationService regionNavigationService)
        {
            var opacityEntranceAnimation = new OpacityAnimation { From = 0, To = 1 };
            var opacityExitAnimation = new OpacityAnimation { From = 1, To = 0 };

            var fxCornerEntranceAnimation = new FXCornerEntranceAnimation();
            var fxCornerExitAnimation = new FXCornerExitAnimation();

            var rotateEntranceAnimation = new RotateAnimation { From = 0, To = 360, RenderTransformOrigin = new Point(0.5, 0.5) };
            var rotateExitAnimation = new RotateAnimation { From = 360, To = 0, RenderTransformOrigin = new Point(0.5, 0.5) };

            var scaleEntranceAnimation = new ScaleAnimation
            {
                From = 0,
                To = 1,
                RenderTransformOrigin = new Point(0.5, 0.5),
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };
            var scaleExitAnimation = new ScaleAnimation
            {
                From = 1,
                To = 0,
                RenderTransformOrigin = new Point(0.5, 0.5),
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };

            translateEntranceAnimation = new TranslateAnimation { From = 800, To = 0 };
            translateExitAnimation = new TranslateAnimation { From = 0, To = 800 };

            var skewEntranceAnimation = new SkewAnimation { From = 30, To = 0, TransformDirection = TransformDirection.Y };
            var skewExitAnimation = new SkewAnimation { From = 0, To = 30, TransformDirection = TransformDirection.Y };

            var fallEntranceAnimation = new FallEntranceAnimation();
            var fallExitAnimation = new FallExitAnimation();

            var fxVscaleEntranceAnimation = new FxVScaleEntranceAnimation();
            var fxVscaleExitAnimation = new FxVScaleExitAnimation();

            var region = regionNavigationService.GetContentRegion("AnimationSample");

            GoViewAWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(opacityEntranceAnimation, opacityExitAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(opacityEntranceAnimation, opacityExitAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fxCornerEntranceAnimation, fxCornerExitAnimation, true);
                await region.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fxCornerEntranceAnimation, fxCornerExitAnimation, true);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(rotateEntranceAnimation, rotateExitAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(rotateEntranceAnimation, rotateExitAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(scaleEntranceAnimation, scaleExitAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(scaleEntranceAnimation, scaleExitAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation, true);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation, true);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(skewEntranceAnimation, skewExitAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(skewEntranceAnimation, skewExitAnimation);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFallAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fallEntranceAnimation, fallExitAnimation, true);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithFallAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fallEntranceAnimation, fallExitAnimation, true);
                await region.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFxVscaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fxVscaleEntranceAnimation, fxVscaleExitAnimation);
                await region.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithFxVscaleAnimationCommand = new RelayCommand(async () =>
            {
                region.ConfigureAnimation(fxVscaleEntranceAnimation, fxVscaleExitAnimation);
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
            translateEntranceAnimation.From = width;
            translateExitAnimation.To = width;
        }

        public bool IsTarget(Type viewType, object parameter)
        {
            return true;
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
