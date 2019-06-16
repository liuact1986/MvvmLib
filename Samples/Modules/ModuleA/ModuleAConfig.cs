using ModuleA.ViewModels;
using ModuleA.Views;
using MvvmLib.Modules;
using MvvmLib.Navigation;

namespace ModuleA
{
    public class ModuleAConfig : IModuleConfig
    {
        public void Initialize()
        {
            SourceResolver.RegisterTypeForNavigation<ViewA>();
            SourceResolver.RegisterTypeForNavigation<ViewBViewModel>("ViewB");
        }
    }
}
