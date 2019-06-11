
using MvvmLib.IoC;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.ViewModels;
using NavigationSample.Wpf.Views;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample
{

    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container)
            : base(container)
        { }

        protected override Window CreateShell()
        {
            return container.GetInstance<Shell>();
        }

        protected override void RegisterTypes()
        {
            container.RegisterSingleton<IFakePeopleService, FakePeopleService>();
        }

        protected override void PreloadApplicationData()
        {
            NavigationManager.CreateNavigationSource("MainContent");
            // 1. Master Details
            NavigationManager.CreateNavigationSource("Details");
            // 2. AnimatableContentControl
            NavigationManager.CreateNavigationSource("AnimationSample");
            // 5. History Sample
            NavigationManager.CreateNavigationSource("HistorySample");

            // 6. TabControl and ListView
            NavigationManager.GetOrCreateSharedSource<IDetailViewModel>();
            NavigationManager.GetOrCreateSharedSource<Person>();
            NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();
        }
    }

}
