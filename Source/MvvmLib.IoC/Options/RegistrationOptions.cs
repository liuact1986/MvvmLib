using System;

namespace MvvmLib.IoC
{
    public class RegistrationOptions
    {
        protected Injector injector;

        public RegistrationOptions(Injector injector)
        {
            this.injector = injector;
        }
    }
}