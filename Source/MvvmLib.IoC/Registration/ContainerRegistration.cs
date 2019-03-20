using System;

namespace MvvmLib.IoC
{

    public class ContainerRegistration
    {
        public string Name { get; internal set; }

        public ContainerRegistrationType ContainerRegistrationType { get; }

        public Action<ContainerRegistration, object> OnResolved { get; internal set; }

        public ContainerRegistration(ContainerRegistrationType containerRegistrationType, string name)
        {
            this.ContainerRegistrationType = containerRegistrationType;
            this.Name = name;
        }
    }
}