using MvvmLib.IoC;
using MvvmLib.Navigation;
using Xamarin.Forms;

namespace NavigationSample
{

    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IInjector Injector { get; } 

        public App()
        {
            InitializeComponent();

            Injector = new Injector();

            var bootstrapper = new Bootstrapper(Injector);
            bootstrapper.Run();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
