﻿using MvvmLib.Mvvm;

namespace CompositeCommandSample.Common
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveAllCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand SaveAllCommand { get; } = new CompositeCommand();
    }
}
