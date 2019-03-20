using RegionSample.Views;
using System;
using System.Diagnostics;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{

    public class ShellViewModel : BindableBase
    {
        public IRelayCommand NavigateToRootCommand { get; private set; }
        public IRelayCommand NavigateCommand { get; private set; }
        public IRelayCommand GoBackCommand { get; private set; }
        public IRelayCommand GoForwardCommand { get; private set; }
        public IRelayCommand NavigateComposedCommand { get; private set; }
        public IRelayCommand OpenWindowCommand { get; private set; }

        public IRelayCommand AddCommand { get; private set; }
        public IRelayCommand InsertCommand { get; private set; }
        public IRelayCommand RemoveAtCommand { get; private set; }
        public IRelayCommand ClearCommand { get; private set; }

        IRegionManager regionManager;

        public ShellViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            var contentRegion = regionManager.GetContentRegion("ContentRegion", "ContentRegion1");
            var contentRegion2 = regionManager.GetContentRegion("ContentRegion", "ContentRegion2");

            var itemsRegion = regionManager.GetItemsRegion("ItemsRegion", "ItemsRegion1");
            var stackPanelRegion = regionManager.GetItemsRegion("StackPanelRegion", "StackPanelRegion1");
            var tabControlRegion = regionManager.GetItemsRegion("TabControlRegion", "TabControlRegion1");

            contentRegion.Navigated += ShellViewModel_Navigated;
            contentRegion.NavigationFailed += ContentRegion_NavigationFailed;
            contentRegion2.Navigated += ShellViewModel_Navigated;

            NavigateToRootCommand = new RelayCommand(async () =>
            {
                await contentRegion.NavigateToRootAsync(EntranceTransitionType.FadeIn, ExitTransitionType.FadeOut);
                await contentRegion2.NavigateToRootAsync(EntranceTransitionType.FadeIn, ExitTransitionType.FadeOut);
            },
            () => contentRegion.CanGoBack || contentRegion2.CanGoBack);

            NavigateCommand = new RelayCommand<Type>(async (viewOrViewModelType) =>
            {
                await contentRegion.NavigateAsync(viewOrViewModelType, viewOrViewModelType.Name + " [default]",
                    EntranceTransitionType.SlideInFromRight, ExitTransitionType.SlideOutToRight);

                await contentRegion2.NavigateAsync(viewOrViewModelType, viewOrViewModelType.Name + " [default]",
                   EntranceTransitionType.SlideInFromRight, ExitTransitionType.SlideOutToRight);
            });

            GoBackCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoBackAsync(EntranceTransitionType.SlideInFromLeft, ExitTransitionType.SlideOutToLeft);
                await contentRegion2.GoBackAsync(EntranceTransitionType.SlideInFromLeft, ExitTransitionType.SlideOutToLeft);
            },
            () => contentRegion.CanGoBack || contentRegion2.CanGoBack);

            GoForwardCommand = new RelayCommand(async () =>
            {
                await contentRegion.GoForwardAsync(EntranceTransitionType.SlideInFromBottom, ExitTransitionType.SlideOutToBottom);
                await contentRegion2.GoForwardAsync(EntranceTransitionType.SlideInFromBottom, ExitTransitionType.SlideOutToBottom);
            },
            () => contentRegion.CanGoForward || contentRegion2.CanGoForward);

            NavigateComposedCommand = new RelayCommand(async () =>
            {
                try
                {
                    await contentRegion.NavigateAsync(typeof(ComposedView), EntranceTransitionType.FadeIn);

                    await regionManager.GetItemsRegion("RegionLeft").AddAsync(typeof(ViewC), "ViewCParameter");

                    await regionManager.GetContentRegion("RegionRight").NavigateAsync(typeof(ViewD));
                    await regionManager.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
                    //await regionManager.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
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
                        await itemsRegion.AddAsync(viewType, viewType.Name + " [default] [Items]", EntranceTransitionType.SlideInFromRight);
                        await regionManager.GetItemsRegion("RegionLeft").AddAsync(typeof(ViewC), "ViewCParameter [items]");
                        await regionManager.GetContentRegion("RegionRight").NavigateAsync(typeof(ViewD));
                        await regionManager.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
                        //await regionManager.GetContentRegion("SubChildRegion").NavigateAsync(typeof(ViewE));
                    }
                    else
                    {
                        await itemsRegion.AddAsync(viewType, viewType.Name + " [default] [Items]", EntranceTransitionType.SlideInFromRight);
                        await stackPanelRegion.AddAsync(viewType, viewType.Name + " [default] [StackPanel]", EntranceTransitionType.SlideInFromTop);
                        await tabControlRegion.AddAsync(viewType, viewType.Name + " [default] [TabControl]", EntranceTransitionType.SlideInFromTop);
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
                    await itemsRegion.InsertAsync(index, typeof(ViewD), "Insert parameter", EntranceTransitionType.FadeIn);
                    await stackPanelRegion.InsertAsync(index, typeof(ViewD), "Insert parameter", EntranceTransitionType.FadeIn);
                    await tabControlRegion.InsertAsync(index, typeof(ViewD), "Insert parameter", EntranceTransitionType.FadeIn);
                }
            });

            RemoveAtCommand = new RelayCommand<string>(async (indexString) =>
            {
                int index = 0;
                if (int.TryParse(indexString, out index))
                {
                    try
                    {
                        await itemsRegion.RemoveAtAsync(index, ExitTransitionType.SlideOutToBottom);
                        await stackPanelRegion.RemoveAtAsync(index, ExitTransitionType.SlideOutToBottom);
                        await tabControlRegion.RemoveAtAsync(index, ExitTransitionType.SlideOutToBottom);
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

        private void ContentRegion_NavigationFailed(object sender, RegionNavigationFailedEventArgs e)
        {
            Debug.WriteLine("Navigation failed , source: " + e.Source.ToString() + ", parameter: " + e.Parameter?.ToString());
        }

        private void ShellViewModel_Navigated(object sender, RegionNavigationEventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
        }

    }


}
