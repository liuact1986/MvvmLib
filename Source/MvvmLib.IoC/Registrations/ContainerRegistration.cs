using System;

namespace MvvmLib.IoC.Registrations
{
    /// <summary>
    /// Container registration base class.
    /// </summary>
    public class ContainerRegistration
    {
        /// <summary>
        /// The name / key.
        /// </summary>
        protected string name;
        /// <summary>
        /// The name / key.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Invoked on resolution.
        /// </summary>
        protected internal Action<ContainerRegistration, object> onResolved;
        /// <summary>
        /// Invoked on resolution.
        /// </summary>
        public Action<ContainerRegistration, object> OnResolved
        {
            get { return onResolved; }
        }
    }
}
