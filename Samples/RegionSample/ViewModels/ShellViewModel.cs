using RegionSample.Views;
using System;
using System.Diagnostics;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Windows;
using System.Windows.Media.Animation;

namespace RegionSample.ViewModels
{

    public class ShellViewModel : BindableBase
    {
        public IRelayCommand NavigateToRootCommand { get; private set; }
        public IRelayCommand NavigateCommand { get; private set; }
        public IRelayCommand GoBackCommand { get; private set; }
        public IRelayCommand GoForwardCommand { get; private set; }
        public IRelayCommand NavigateComposedCommand { get; private set; }
        public IRelayCommand FakeLoginCommand { get; private set; }
        public IRelayCommand OpenWindowCommand { get; private set; }

        public IRelayCommand AddCommand { get; private set; }
        public IRelayCommand InsertCommand { get; private set; }
        public IRelayCommand RemoveAtCommand { get; private set; }
        public IRelayCommand ClearCommand { get; private set; }

        IRegionNavigationService navigationService;

        public ShellViewModel(IRegionNavigationService navigationService)
        {
            this.navigationService = navigationService;

            var entranceFadeAnimation = new OpacityAnimation { From = 0, To = 1 };
            var exitFadeAnimation = new OpacityAnimation { From = 1, To = 0 };

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

            var entranceTranslateAnimation = new TranslateAnimation { From = 800, To = 0 };
            var exitTranslateAnimation = new TranslateAnimation { From = 0, To = 800 };

            var contentRegion = navigationService.GetContentRegion("ContentRegion", "ContentRegion1");
            // animation
            contentRegion.ConfigureAnimation(entranceTranslateAnimation, exitTranslateAnimation, true);
            // handlers
            contentRegion.CanGoBackChanged += OnContentRegionCanGoBackChanged;
            contentRegion.CanGoForwardChanged += OnContentRegionCanGoForwardChanged;
            contentRegion.Navigated += ShellViewModel_Navigated;
            contentRegion.NavigationFailed += ContentRegion_NavigationFailed;

            var contentRegion2 = navigationService.GetContentRegion("ContentRegion", "ContentRegion2");
            // animation
            contentRegion2.ConfigureAnimation(entranceScaleAnimation, exitScaleAnimation);
            // handlers
            contentRegion2.CanGoBackChanged += OnContentRegionCanGoBackChanged;
            contentRegion2.CanGoForwardChanged += OnContentRegionCanGoForwardChanged;
            contentRegion2.Navigated += ShellViewModel_Navigated;

            var itemsRegion = navigationService.GetItemsRegion("ItemsRegion");
            itemsRegion.ConfigureAnimation(entranceTranslateAnimation, exitTranslateAnimation);

            var stackPanelRegion = navigationService.GetItemsRegion("StackPanelRegion", "StackPanelRegion1");
            var tabControlRegion = navigationService.GetItemsRegion("TabControlRegion", "TabControlRegion1");

            NavigateToRootCommand = new RelayCommand(async () =>
            {
                await contentRegion.NavigateToRootAsync();
                await contentRegion2.NavigateToRootAsync();
            },
            () => contentRegion.CanGoBack || contentRegion2.CanGoBack);

            NavigateCommand = new RelayCommand<Type>(async (viewOrViewModelType) =>
            {
                await contentRegion.NavigateAsync(viewOrViewModelType, viewOrViewModelType.Name + " [default]");
                await contentRegion2.NavigateAsync(viewOrViewModelType, viewOrViewModelType.Name + " [default]");
            });

            GoBackCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoBackAsync();
                await contentRegion2.GoBackAsync();
            },
            () => contentRegion.CanGoBack || contentRegion2.CanGoBack);

            GoForwardCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoForwardAsync();
                await contentRegion2.GoForwardAsync();
            },
            () => contentRegion.CanGoForward || contentRegion2.CanGoForward);


            FakeLoginCommand = new RelayCommand(async () =>
            {
                await contentRegion.NavigateAsync(typeof(HomeView), "Remove login page from history + can go back changed notification");
            });

            NavigateComposedCommand = new RelayCommand(async () =>
            {
                try
                {
                    await contentRegion.NavigateAsync(typeof(ComposedView));

                    await navigationService.GetItemsRegion("RegionLeft").AddAsync(typeof(ViewC), "ViewCParameter");

                    await navigationService.GetContentRegion("RegionRight").NavigateAsync(typeof(ViewD));
                    await navigationService.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
                }
                catch
                {
                    // child is not found if navigation is canceled
                }
            });

            OpenWindowCommand = new RelayCommand(() =>
            {
                var w = new CustomWindow();
                w.Show();
            });


            // Items region

            AddCommand = new RelayCommand<Type>(async (viewType) =>
            {
                try
                {
                    if (viewType == typeof(ComposedView))
                    {
                        await itemsRegion.AddAsync(viewType, viewType.Name + " [default] [Items]");
                        await navigationService.GetItemsRegion("RegionLeft").AddAsync(typeof(ViewC), "ViewCParameter [items]");
                        await navigationService.GetContentRegion("RegionRight").NavigateAsync(typeof(ViewD));
                        await navigationService.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
                    }
                    else
                    {
                        await itemsRegion.AddAsync(viewType, viewType.Name + " [default] [Items]");
                        await stackPanelRegion.AddAsync(viewType, viewType.Name + " [default] [StackPanel]");
                        await tabControlRegion.AddAsync(viewType, viewType.Name + " [default] [TabControl]");
                    }
                }
                catch
                {
                    // child is not found if navigation is canceled
                }

            });

            InsertCommand = new RelayCommand<string>(async (indexString) =>
            {
                int index = 0;
                if (int.TryParse(indexString, out index))
                {
                    await itemsRegion.InsertAsync(index, typeof(ViewD), "Insert parameter");
                    await stackPanelRegion.InsertAsync(index, typeof(ViewD), "Insert parameter");
                    await tabControlRegion.InsertAsync(index, typeof(ViewD), "Insert parameter");
                }
            });

            RemoveAtCommand = new RelayCommand<string>(async (indexString) =>
            {
                int index = 0;
                if (int.TryParse(indexString, out index))
                {
                    try
                    {
                        await itemsRegion.RemoveAtAsync(index);
                        await stackPanelRegion.RemoveAtAsync(index);
                        await tabControlRegion.RemoveAtAsync(index);
                    }
                    catch
                    {

                    }
                }
            });

            ClearCommand = new RelayCommand(() =>
            {
                itemsRegion.Clear();
                stackPanelRegion.Clear();
                tabControlRegion.Clear();
            });
        }

        private void OnContentRegionCanGoBackChanged(object sender, EventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void OnContentRegionCanGoForwardChanged(object sender, EventArgs e)
        {
            GoForwardCommand.RaiseCanExecuteChanged();
        }

        private void ContentRegion_NavigationFailed(object sender, RegionNavigationFailedEventArgs e)
        {
            //Debug.WriteLine("Navigation failed , source: " + e.Exception);
        }

        private void ShellViewModel_Navigated(object sender, RegionNavigationEventArgs e)
        {

        }

    }


}
