using MvvmLib.Animation;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationViewModel : BindableBase, ILoadedEventListener, ISelectable
    {
        private IContentAnimation entranceNavAnimation;
        public IContentAnimation EntranceNavAnimation
        {
            get { return entranceNavAnimation; }
            set { SetProperty(ref entranceNavAnimation, value); }
        }

        private IContentAnimation exitNavAnimation;
        public IContentAnimation ExitNavAnimation
        {
            get { return exitNavAnimation; }
            set { SetProperty(ref exitNavAnimation, value); }
        }

        private bool navSimultaneous;
        public bool NavSimultaneous
        {
            get { return navSimultaneous; }
            set { SetProperty(ref navSimultaneous, value); }
        }

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

        private ContentRegion contentRegion;

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

            this.contentRegion = regionNavigationService.GetContentRegion("AnimationSample");

            GoViewAWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(opacityEntranceAnimation, opacityExitAnimation);
                await contentRegion.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFadeAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(opacityEntranceAnimation, opacityExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fxCornerEntranceAnimation, fxCornerExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });

            GoViewBWithFxCornerAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fxCornerEntranceAnimation, fxCornerExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(rotateEntranceAnimation, rotateExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithRotateAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(rotateEntranceAnimation, rotateExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(scaleEntranceAnimation, scaleExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithScaleAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(scaleEntranceAnimation, scaleExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(translateEntranceAnimation, translateExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithTranslateAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(translateEntranceAnimation, translateExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(skewEntranceAnimation, skewExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithSkewAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(skewEntranceAnimation, skewExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFallAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fallEntranceAnimation, fallExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithFallAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fallEntranceAnimation, fallExitAnimation, true);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });

            GoViewAWithFxVscaleAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fxVscaleEntranceAnimation, fxVscaleExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewA));
            });
            GoViewBWithFxVscaleAnimationCommand = new RelayCommand(async () =>
            {
                ConfigureAnimation(fxVscaleEntranceAnimation, fxVscaleExitAnimation);

                await contentRegion.NavigateAsync(typeof(ViewB));
            });
        }

        public void ConfigureAnimation(IContentAnimation entranceAnimation, IContentAnimation exitAnimation, bool simultaneous = false)
        {
            EntranceNavAnimation = entranceAnimation;
            ExitNavAnimation = exitAnimation;
            NavSimultaneous = simultaneous;
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


}
