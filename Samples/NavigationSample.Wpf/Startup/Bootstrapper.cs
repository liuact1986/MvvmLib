
using MvvmLib.IoC;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Models;
using NavigationSample.Wpf.Views;
using System.Windows;
using System.Windows.Controls;

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
