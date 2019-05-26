using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to resolve and cache assembly types.
    /// </summary>
    public class ScannedTypeManager
    {
        private readonly Dictionary<string, ScannedAssembly> assemblies;
        /// <summary>
        /// The assemblies resolved.
        /// </summary>
        public Dictionary<string, ScannedAssembly> Assemblies
        {
            get { return assemblies; }
        }

        /// <summary>
        /// Initializes the ScannedTypeManager.
        /// </summary>
        public ScannedTypeManager()
        {
            assemblies = new Dictionary<string, ScannedAssembly>();
        }

        private ScannedAssembly GetAssembly(Assembly assembly)
        {
            var assemblyQualifiedName = assembly.FullName;
            if (assemblies.TryGetValue(assemblyQualifiedName, out ScannedAssembly scannedAssembly))
            {
                return scannedAssembly;
            }
            else
            {
                scannedAssembly = new ScannedAssembly(assembly);
                assemblies[assemblyQualifiedName] = scannedAssembly;
                return scannedAssembly;
            }
        }

        /// <summary>
        /// Tries to resolve the implementation type for the interface.
        /// </summary>
        /// <param name="interfaceType">The interface type</param>
        /// <returns>The type resolved</returns>
        public Type FindImplementationType(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"An interface is expected. Current \"{interfaceType}\"");

            var implementationTypes = GetAssembly(interfaceType.GetTypeInfo().Assembly).GetTypes().ThatImplement(interfaceType);
            int count = implementationTypes.Count;
            if (count == 1)
            {
                return implementationTypes[0].Type;
            }
            else
            {
                foreach (var implementationType in implementationTypes)
                {
                    var attribute = implementationType.Type.GetCustomAttribute(typeof(PreferredImplementationAttribute)) as PreferredImplementationAttribute;
                    if (attribute != null)
                    {
                        return implementationType.Type;
                    }
                }
            }
            return null;
        }
    }
}
