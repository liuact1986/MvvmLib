using MvvmLib.Navigation;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavigationSample.Windows.Views
{
    public sealed partial class Shell : Page
    {

        INavigationManager navigationManager;
        IBackRequestManager backRequestManager;
        IFrameNavigationService navigationService;

        public Shell(INavigationManager navigationManager, IBackRequestManager backRequestManager)
        {
            this.navigationManager = navigationManager;
            this.backRequestManager = backRequestManager;

            this.InitializeComponent();

            this.Loaded += OnShellLoaded;
        }

        private async void OnShellLoaded(object sender, RoutedEventArgs e)
        {
            navigationService = navigationManager.GetDefault();
            navigationService.Navigated += OnNavigated;
            navigationService.NavigationCanceled += OnNavigationCanceled;

            // back request (Title bar + NavigationView)
            backRequestManager.Handle(MainFrame, () => HandleBackRequested());
            NavigationView.BackRequested += (se, ex) => HandleBackRequested();

            // navigate to home page on load
            await navigationService.NavigateAsync(typeof(HomePage));
        }

        private async void HandleBackRequested()
        {
            if (navigationService.CanGoBack)
            {
                await navigationService.GoBackAsync();
            }
        }

        private void SyncMenutItem()
        {
            var view = MainFrame.Content as FrameworkElement;
            if (view != null)
            {
                var viewTypeName = view.GetType().Name;
                var selectedMenuItem = NavigationView.MenuItems.FirstOrDefault((m) =>
                {
                    return ((NavigationViewItem)m).Tag.ToString() == viewTypeName;
                });

                NavigationView.SelectedItem = selectedMenuItem;
            }
        }

        private void OnNavigated(object sender, FrameNavigatedEventArgs e)
        {
            SyncMenutItem();
        }

        private void OnNavigationCanceled(object sender, FrameNavigationCanceledEventArgs e)
        {
            SyncMenutItem();
        }


        private async void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // settings page
                await navigationService.NavigateAsync(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer != null)
            {
                var tag = args.InvokedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "PageA":
                        await navigationService.NavigateAsync(typeof(PageA), "PageA navigation parameter");
                        break;
                    case "PageB":
                        await navigationService.NavigateAsync(typeof(PageB), "PageB navigation parameter");
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
