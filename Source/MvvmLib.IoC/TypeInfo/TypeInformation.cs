using System.Reflection;

namespace MvvmLib.IoC
{
    public class TypeInformation
    {
        public ConstructorInfo Constructor { get; }

        public ParameterInfo[] Parameters { get; }

        public TypeInformation(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            Constructor = constructor;
            Parameters = parameters;
        }
    }
}
