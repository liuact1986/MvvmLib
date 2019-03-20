using System;

namespace MvvmLib.IoC
{
    public class AutoDiscover
    {
        private Injector injector;

        public AutoDiscover(Injector injector)
        {
            this.injector = injector;
        }

        public void CheckRegistered(Type type, string name)
        {
            if (!injector.IsRegistered(type, name) 
                && !ValueContainer.IsTypeSupported(type))
            {
                if (injector.AutoDiscovery)
                {
                    if (type.IsInterface) { throw new ResolutionFailedException("Cannot resolve the unregistered type for \"" + type.Name + "\""); }

                    injector.RegisterType(type, name);
                }
                else
                {
                    throw new ResolutionFailedException("No type \"" + type.Name + "\" with the name \"" + name + "\" registered");
                }
            }
        }
    }

}