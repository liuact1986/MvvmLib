using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to select the preferred implementation for an interface. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PreferredImplementationAttribute : Attribute
    {
    }
}
