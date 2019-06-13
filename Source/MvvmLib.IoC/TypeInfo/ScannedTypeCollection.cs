using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MvvmLib.IoC.TypeInfo
{
    /// <summary>
    /// Collection for <see cref="ScannedType"/>.
    /// </summary>
    public class ScannedTypeCollection : Collection<ScannedType>
    {
        /// <summary>
        /// Gets the list of <see cref="ScannedType"/> with Type that implements the interface.
        /// </summary>
        /// <param name="interfaceType">The interface type</param>
        /// <returns>A list of <see cref="ScannedType"/></returns>
        public List<ScannedType> ThatImplement(Type interfaceType)
        {
            var implementationTypes = new List<ScannedType>();
            foreach (var item in Items)
            {
                if (item.GetInterfaces().Contains(interfaceType))
                {
                    implementationTypes.Add(item);
                }
            }

            return implementationTypes;
        }
    }
}
