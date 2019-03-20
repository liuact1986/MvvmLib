using HelloWorld.Views;
using MvvmLib.IoC;
using System.Windows;

namespace HelloWorld
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
           
        }
    }
}
