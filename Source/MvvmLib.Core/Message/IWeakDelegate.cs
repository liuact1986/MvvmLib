using System;

namespace MvvmLib.Message
{
    /// <summary>
    /// Use Weak references to store and create delegates.
    /// </summary>
    public interface IWeakDelegate
    {
        /// <summary>
        /// Tries to create a delegate if weak reference target is alive.
        /// </summary>
        Delegate Target { get; }
    }
}
