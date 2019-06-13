using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to select the preferred constructor for a type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class PreferredConstructorAttribute : Attribute
    { }
}
