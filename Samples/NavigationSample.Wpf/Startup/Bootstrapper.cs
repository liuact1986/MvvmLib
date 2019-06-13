using MvvmLib.IoC;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.ViewModels;
using NavigationSample.Wpf.Views;
using System.Windows;

namespace NavigationSample.Wpf.Startup
{
    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container)
            : base(container)
        { }

        protected override void RegisterTypes()
        {
            //container.RegisterSingleton<IFakePeopleService, FakePeopleService>();
        }

        protected override void PreloadApplicationData()
        {
            // Main content
            NavigationManager.CreateDefaultNavigationSource("Main");
            NavigationManager.CreateSharedSource<MenuItem>();
            // 1. MasterDetails Sample
            NavigationManager.CreateDefaultNavigationSource("MasterDetails");
            NavigationManager.CreateSharedSource<Person>("MasterDetails");
            // 2. AnimatableContentControl Sample
            NavigationManager.CreateDefaultNavigationSource("AnimationSample");
            // 5. History Sample
            NavigationManager.CreateDefaultNavigationSource("HistorySample");
            // 6. TabControl and ListView
            NavigationManager.CreateSharedSource<IDetailViewModel>();
            NavigationManager.CreateSharedSource<Person>();
            NavigationManager.CreateSharedSource<MyItemDetailsViewModel>();
            
        }

        //protected override object CreateShellViewModel()
        //{
        //    return container.GetInstance<ShellViewModel>();
        //}

        protected override Window CreateShell()
        {
            return container.GetInstance<Shell>();
        }
    }

}
