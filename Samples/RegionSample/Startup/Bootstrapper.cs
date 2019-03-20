using RegionSample.Adapters;
using RegionSample.ViewModels;
using RegionSample.Views;
using System.Windows;
using MvvmLib.Navigation;
using MvvmLib.IoC;

namespace RegionSample
{

    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container) : base(container)
        {
        }

        protected override Window CreateShell()
        {
            return container.GetInstance<Shell>();
        }

        protected override void RegisterTypes()
        {
            container.RegisterSingleton<ViewBViewModel>();
        }

        protected override void RegisterCustomRegionAdapters()
        {
            RegionAdapterContainer.RegisterAdapter(new StackPanelRegionAdapter());
        }
    }
}