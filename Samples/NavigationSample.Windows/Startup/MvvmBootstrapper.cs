using MvvmLib.IoC;
using MvvmLib.Navigation;

namespace NavigationSample.Windows
{
    public abstract class MvvmBootstrapper : BootstrapperBase
    {
        protected IInjector container;

        public MvvmBootstrapper(IInjector container)
        {
            this.container = container;
        }

        protected override void SetViewModelFactory()
        {
            ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
        }

        protected override void RegisterRequiredTypes()
        {
            container.RegisterType<INavigationManager, NavigationManager>();
            container.RegisterType<IBackRequestManager, BackRequestManager>();
        }
    }
}
