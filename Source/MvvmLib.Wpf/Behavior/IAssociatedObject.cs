﻿using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to provide a dependency object to the bahavior.
    /// </summary>
    public interface IAssociatedObject
    {
        /// <summary>
        /// The dependency object / control.
        /// </summary>
        DependencyObject AssociatedObject { get; set; }
    }
}