using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to discover types, create instances and inject dependencies.
    /// </summary>
    public interface IInjectorResolver
    {
        /// <summary>
        /// Allows to discover non registered types.
        /// </summary>
        bool AutoDiscovery { get; set; }

        /// <summary>
        /// Allows to include non public constructors.
        /// </summary>
        bool NonPublicConstructors { get; set; }

        /// <summary>
        /// Allows to include non public properties.
        /// </summary>
        bool NonPublicProperties { get; set; }

        /// <summary>
        /// The delegate factory type, Linq Expressions used by default.
        /// </summary>
        DelegateFactoryType DelegateFactoryType { get; set; }

        /// <summary>
        /// Invoked on instance resolution.
        /// </summary>
        event EventHandler<ResolutionEventArgs> Resolved;

        /// <summary>
        /// Gets a new instance for the type and name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        object GetNewInstance(Type type, string name);

        /// <summary>
        /// Gets a new instance for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance</returns>
        object GetNewInstance(Type type);
        /// <summary>
        /// Gets the instance for the type and name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        object GetInstance(Type type, string name);

        /// <summary>
        /// Gets the instance for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance</returns>
        object GetInstance(Type type);

        /// <summary>
        /// Gets all instances of the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The list of instances</returns>
        List<object> GetAllInstances(Type type);

        /// <summary>
        /// Fills the properties of the instance with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        object BuildUp(Type type, string name, object instance);

        /// <summary>
        /// Fills the properties of the instance.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        object BuildUp(object instance);

        /// <summary>
        /// Fills the properties of an instance with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>The instance filled</returns>
        object BuildUp(Type type, string name);

        /// <summary>
        /// Fills the properties of an instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance filled</returns>
        object BuildUp(Type type);
    }

}