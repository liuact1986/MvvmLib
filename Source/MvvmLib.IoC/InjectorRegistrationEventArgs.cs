namespace MvvmLib.IoC
{
    public class InjectorRegistrationEventArgs
    {
        private ContainerRegistration Registration { get; }

        public InjectorRegistrationEventArgs(ContainerRegistration registration)
        {
            this.Registration = registration;
        }
    }
}