using ModuleB.Services;
using ModuleB.Views;
using ModuleSharedServices;
using MvvmLib.IoC;
using MvvmLib.Modules;
using MvvmLib.Navigation;

namespace ModuleB
{
    public class ModuleBConfig : IModuleConfig
    {
        private readonly IInjector injector;

        public ModuleBConfig(IInjector injector)
        {
            this.injector = injector;
        }

        public void Initialize()
        {
            injector.RegisterType<IMyService, MyService>();

            // injector.RegisterType<IMySharedService, MySharedService>();

            SourceResolver.RegisterTypeForNavigation<ViewC>();
        }
    }
}
