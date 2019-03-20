﻿using MvvmLib.IoC;
using System.Windows;

namespace HelloWorld
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper(new Injector());
            bootstrapper.Run();
        }
    }
}
