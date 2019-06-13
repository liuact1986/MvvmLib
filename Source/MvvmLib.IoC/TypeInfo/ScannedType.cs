using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MvvmLib.IoC.TypeInfo
{
    /// <summary>
    /// A wrapper for Type (class).
    /// </summary>
    public class ScannedType
    {
        private Type type;
        /// <summary>
        /// The wrapped Type (class).
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        private Type[] implementedInterfaces;

        /// <summary>
        /// Creates the ScannedType.
        /// </summary>
        /// <param name="type">The wrapped Type</param>
        public ScannedType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.type = type;
        }

        /// <summary>
        /// Gets the interfaces of the wrapped type.
        /// </summary>
        /// <returns>An array of interfaces</returns>
        public Type[] GetInterfaces()
        {
            if (implementedInterfaces != null)
            {
                return implementedInterfaces;
            }
            else
            {
                var interfaces = type.GetInterfaces();
                this.implementedInterfaces = interfaces;
                return interfaces;
            }
        }
    }
}
