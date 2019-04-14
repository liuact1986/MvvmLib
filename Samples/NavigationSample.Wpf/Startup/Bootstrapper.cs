
using MvvmLib.IoC;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System.Windows;

namespace NavigationSample
{

    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container) 
            : base(container)
        {  }

        protected override Window CreateShell()
        {
            return container.GetInstance<Shell>();
        }

        protected override void RegisterTypes()
        {
            container.RegisterSingleton<IFakePeopleService, FakePeopleService>();
        }
    }
}
