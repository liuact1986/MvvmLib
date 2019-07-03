using ModuleSample.ViewModels;
using ModuleSample.Views;
using ModuleSharedServices;
using MvvmLib.IoC;
using MvvmLib.Modules;
using MvvmLib.Navigation;
using System.Windows;

namespace ModuleSample.Startup
{
    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container)
            : base(container)
        { }

        protected override void RegisterTypes()
        {
            // shared service
            container.RegisterSingleton<IMySharedService, MySharedService>();
        }

        protected override void PreloadApplicationData()
        {
            
        }

        protected override object CreateShellViewModel()
        {
            return container.GetInstance<ShellViewModel>();
        }

        protected override Window CreateShell()
        {
            return container.GetInstance<Shell>();
        }

        protected override void RegisterModules()
        {
            // with App.Config 
            // or
            ModuleManager.RegisterModule("ModuleB", @"Modules\ModuleB.dll", "ModuleB.ModuleBConfig");
        }
    }

}
