using MvvmLib.IoC;
using MvvmLib.Navigation;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace NavigationSample.Windows
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private const string NavigationStateName = "__navigationstate__";

        public new static App Current => (App)Application.Current;

        public IInjector Container { get; private set; } = new Injector();

        public INavigationManager NavigationManager { get; private set; }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var bootstrapper = new Bootstrapper(Container);
            bootstrapper.Run();

            NavigationManager = Container.GetInstance<INavigationManager>();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(NavigationStateName))
                {
                    var navigationState = ApplicationData.Current.LocalSettings.Values[NavigationStateName].ToString();
                    NavigationManager.GetDefault().SetNavigationState(navigationState);
                }
            }

        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            var navigationState = NavigationManager.GetDefault().GetNavigationState();
            ApplicationData.Current.LocalSettings.Values[NavigationStateName] = navigationState;

            deferral.Complete();
        }
    }
}
