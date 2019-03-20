namespace MvvmLib.IoC
{
    public class InjectorResolveEventArgs
    {
        public ContainerRegistration Registration { get; }
        public object Instance { get; }

        public InjectorResolveEventArgs(ContainerRegistration registration, object instance)
        {
            this.Registration = registration;
            this.Instance = instance;
        }
    }
}