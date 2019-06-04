using AdaptiveSample.Views;
using System.Windows;
using MvvmLib.Mvvm;
using MvvmLib.IoC;
using MvvmLib.Message;
using MvvmLib.Navigation;

namespace AdaptiveSample
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

    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container)
            : base(container)
        {
        }

        protected override Window CreateShell()
        {
            return container.GetInstance<MainWindow>();
        }

        protected override void RegisterTypes()
        {

        }
    }
}
