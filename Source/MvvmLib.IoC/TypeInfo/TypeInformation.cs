using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Type information class.
    /// </summary>
    public class TypeInformation
    {
        private readonly ConstructorInfo constructor;
        /// <summary>
        /// The constructor.
        /// </summary>
        public ConstructorInfo Constructor
        {
            get { return constructor; }
        }

        private readonly ParameterInfo[] parameters;
        /// <summary>
        /// The parameters.
        /// </summary>
        public ParameterInfo[] Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// Creates the type information class.
        /// </summary>
        /// <param name="constructor">The constructor</param>
        /// <param name="parameters">The parameters</param>
        public TypeInformation(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            this.constructor = constructor;
            this.parameters = parameters;
        }
    }
}