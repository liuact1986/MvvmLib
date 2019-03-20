using MvvmLib.IoC;
using MvvmLib.Navigation;
using NavigationSample.Windows.Services;
using NavigationSample.Windows.ViewModels;
using NavigationSample.Windows.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavigationSample.Windows
{

    public class Bootstrapper : MvvmBootstrapper
    {
        public Bootstrapper(IInjector container)
            : base(container)
        { }

        protected override void RegisterTypes()
        {
            container.RegisterType<IMyService, MyService>();
            container.RegisterSingleton<PageBViewModel>();
        }

        protected override Page CreateShell()
        {
            return container.GetInstance<Shell>();
        }

        protected override void ConfigureNavigation(Page shell)
        {
            var frame = ((Shell)shell).MainFrame; //  x:FieldModifier="Public"
            NavigationManager.Register(frame);
        }

        protected override void InitializeShell(Page shell)
        {
            Window.Current.Content = shell;
        }

    }
}
