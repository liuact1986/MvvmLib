namespace MvvmLib.IoC
{
    /// <summary>
    /// The resolution event args class.
    /// </summary>
    public class ResolutionEventArgs
    {
        private ContainerRegistration registration;
        /// <summary>
        /// The registration.
        /// </summary>
        public ContainerRegistration Registration
        {
            get { return registration; }
        }

        private object instance;
        /// <summary>
        /// The instance.
        /// </summary>
        public object Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Creates the resolution event args class
        /// </summary>
        /// <param name="registration">The registration</param>
        /// <param name="instance">The instance</param>
        public ResolutionEventArgs(ContainerRegistration registration, object instance)
        {
            this.registration = registration;
            this.instance = instance;
        }

    }
}