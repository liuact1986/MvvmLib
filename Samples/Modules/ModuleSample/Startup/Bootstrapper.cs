using ModuleSample.ViewModels;
using ModuleSample.Views;
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
            //ModuleManager.RegisterModule("ModuleA", @"C:\Projects\vx1\MvvmLib\Samples\Modules\ModuleA\bin\Debug\ModuleA.dll", "ModuleA.ModuleAConfig");
        }
    }

}
