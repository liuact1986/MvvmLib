using MvvmLib.Navigation;
using MvvmLib.IoC;
using MvvmLib.Message;

namespace CompositeCommandSample
{
    public abstract class MvvmLibBootstrapper : BootstrapperBase
    {
        protected IInjector container;

        public MvvmLibBootstrapper(IInjector container)
        {
            this.container = container;
        }

        protected override void RegisterRequiredTypes()
        {
            container.RegisterInstance<IInjector>(container);
            container.RegisterSingleton<IEventAggregator, EventAggregator>();
            container.RegisterSingleton<IRegionNavigationService, RegionNavigationService>();
        }

        protected override void SetViewFactory()
        {
            ViewResolver.SetViewFactory((viewType) => container.GetNewInstance(viewType));
        }

        protected override void SetViewModelFactory()
        {
            ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
        }
    }
}