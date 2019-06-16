using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Navigation;

namespace ModuleSample.Startup
{
    public abstract class MvvmLibBootstrapper : BootstrapperBase
    {
        protected IInjector container;

        public MvvmLibBootstrapper(IInjector container)
        {
            if (container == null)
                throw new System.ArgumentNullException(nameof(container));

            this.container = container;
        }

        protected override void RegisterRequiredTypes()
        {
            container.RegisterInstance<IInjector>(container);
            container.RegisterSingleton<IEventAggregator, EventAggregator>();
        }

        protected override void SetViewFactory()
        {
            SourceResolver.SetFactory((sourceType) => container.GetNewInstance(sourceType));
        }

        protected override void SetViewModelFactory()
        {
            ViewModelLocationProvider.SetViewModelFactory((viewModelType) => container.GetInstance(viewModelType));
        }
    }
}
