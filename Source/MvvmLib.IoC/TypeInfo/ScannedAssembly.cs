using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MvvmLib.IoC.TypeInfo
{
    /// <summary>
    /// A wrapper for assembly.
    /// </summary>
    public class ScannedAssembly
    {
        private Assembly assembly;
        /// <summary>
        /// The wrapped assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }

        private ScannedTypeCollection types;
        /// <summary>
        /// The cleaned assembly types (without interfaces and auto-generated types)
        /// </summary>
        public ScannedTypeCollection Types
        {
            get { return types; }
        }

        /// <summary>
        /// Creates the SannedAssembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public ScannedAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the types for the assembly.
        /// </summary>
        /// <returns>A collection of <see cref="ScannedType"/></returns>
        public ScannedTypeCollection GetTypes()
        {
            if (types != null)
            {
                return types;
            }
            else
            {
                var scannedTypes = new ScannedTypeCollection();
                try
                {
                    var types = assembly.GetTypes().Where(t => !t.IsInterface && t.Name[0] != '<');
                    foreach (var type in types)
                    {
                        scannedTypes.Add(new ScannedType(type));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR: Failed to parse assembly types. Assembly {assembly.FullName}. Error \"{ex}\" \"{ex.Message}\". Timestamp:{0:DateTime.Now}.");
                }
                this.types = scannedTypes;
                return scannedTypes;
            }
        }
    }
}
