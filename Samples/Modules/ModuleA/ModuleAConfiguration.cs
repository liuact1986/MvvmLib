﻿using ModuleA.ViewModels;
using ModuleA.Views;
using MvvmLib.Modules;
using MvvmLib.Navigation;

namespace ModuleA
{
    public class ModuleAConfiguration : IModuleConfiguration
    {
        public void Initialize()
        {
            SourceResolver.RegisterTypeForNavigation<ViewA>(); // With View
            SourceResolver.RegisterTypeForNavigation<ViewBViewModel>("ViewB"); // With ViewModel (+ DataTemplate)
        }
    }
}
