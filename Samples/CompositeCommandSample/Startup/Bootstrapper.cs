using System.Windows;
using MvvmLib.IoC;
using CompositeCommandSample.Views;
using CompositeCommandSample.Common;

namespace CompositeCommandSample
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
            container.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }
    }
}