using ModuleB.Services;
using ModuleB.Views;
using ModuleSharedServices;
using MvvmLib.IoC;
using MvvmLib.Modules;
using MvvmLib.Navigation;
using System.Threading.Tasks;

namespace ModuleB
{
    public class ModuleBConfiguration : IModuleConfiguration
    {
        private readonly IInjector injector;

        public ModuleBConfiguration(IInjector injector)
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
