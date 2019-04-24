namespace MvvmLib.IoC
{
    /// <summary>
    /// The registration event args class.
    /// </summary>
    public class RegistrationEventArgs
    {
        private readonly ContainerRegistration registration;
        /// <summary>
        /// The registration.
        /// </summary>
        public ContainerRegistration Registration
        {
            get { return registration; }
        }

        /// <summary>
        /// Creates the registration event args class.
        /// </summary>
        /// <param name="registration">The registration</param>
        public RegistrationEventArgs(ContainerRegistration registration)
        {
            this.registration = registration;
        }
    }
}