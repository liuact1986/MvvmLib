using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to register or discover types, factories, instances then create instances and inject dependencies.
    /// </summary>
    public class Injector : IInjector
    {
        private const string DefaultName = "___Default___";
        private readonly ConcurrentDictionary<Type, Dictionary<string, ContainerRegistration>> registrations;
        private readonly TypeInformationManager typeInformationManager;
        private readonly ObjectCreationManager objectCreationManager;
        private readonly ScannedTypeManager scannedTypeManager;
        private readonly SingletonCache singletonCache;

        private bool autoDiscovery;
        /// <summary>
        /// Allows to discover non registered types.
        /// </summary>
        public bool AutoDiscovery
        {
            get { return autoDiscovery; }
            set { autoDiscovery = value; }
        }

        private bool nonPublicConstructors;
        /// <summary>
        /// Allows to include non public constructors.
        /// </summary>
        public bool NonPublicConstructors
        {
            get { return nonPublicConstructors; }
            set { nonPublicConstructors = value; }
        }

        private bool nonPublicProperties;
        /// <summary>
        /// Allows to include non public properties.
        /// </summary>
        public bool NonPublicProperties
        {
            get { return nonPublicProperties; }
            set { nonPublicProperties = value; }
        }

        /// <summary>
        /// The delegate factory type, Linq Expressions used by default.
        /// </summary>
        public DelegateFactoryType DelegateFactoryType
        {
            get { return this.objectCreationManager.DelegateFactoryType; }
            set { this.objectCreationManager.DelegateFactoryType = value; }
        }

        private readonly List<EventHandler<RegistrationEventArgs>> registered;
        /// <summary>
        /// Invoked on registration.
        /// </summary>
        public event EventHandler<RegistrationEventArgs> Registered
        {
            add { if (!registered.Contains(value)) registered.Add(value); }
            remove { if (registered.Contains(value)) registered.Remove(value); }
        }

        private readonly List<EventHandler<ResolutionEventArgs>> resolved;
        /// <summary>
        /// Invoked on instance resolution.
        /// </summary>
        public event EventHandler<ResolutionEventArgs> Resolved
        {
            add { if (!resolved.Contains(value)) resolved.Add(value); }
            remove { if (resolved.Contains(value)) resolved.Remove(value); }
        }

        /// <summary>
        /// Creates the injector class.
        /// </summary>
        /// <param name="typeInformationManager">The type information manager</param>
        /// <param name="objectCreationManager">The object creation manager</param>
        /// <param name="singletonCache">The cache for singletons</param>
        /// <param name="scannedTypeManager">The scanned type manager</param>
        protected internal Injector(TypeInformationManager typeInformationManager, ObjectCreationManager objectCreationManager, SingletonCache singletonCache, ScannedTypeManager scannedTypeManager)
        {
            if (typeInformationManager == null)
                throw new ArgumentNullException(nameof(typeInformationManager));
            if (objectCreationManager == null)
                throw new ArgumentNullException(nameof(objectCreationManager));
            if (singletonCache == null)
                throw new ArgumentNullException(nameof(singletonCache));
            if (scannedTypeManager == null)
                throw new ArgumentNullException(nameof(scannedTypeManager));

            registrations = new ConcurrentDictionary<Type, Dictionary<string, ContainerRegistration>>();
            registered = new List<EventHandler<RegistrationEventArgs>>();
            resolved = new List<EventHandler<ResolutionEventArgs>>();

            this.typeInformationManager = typeInformationManager;
            this.objectCreationManager = objectCreationManager;
            this.singletonCache = singletonCache;
            this.scannedTypeManager = scannedTypeManager;
            this.autoDiscovery = true;
        }

        /// <summary>
        /// Creates the injector class.
        /// </summary>
        public Injector()
            : this(new TypeInformationManager(), new ObjectCreationManager(), new SingletonCache(), new ScannedTypeManager())
        { }

        #region Registration

        /// <summary>
        /// Checks if there is a registration for the type with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>true if found</returns>
        public bool IsRegistered(Type type, string name)
        {
            return this.registrations.ContainsKey(type) && this.registrations[type].ContainsKey(name);
        }

        /// <summary>
        /// Checks if there is a registration for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>true if found</returns>
        public bool IsRegistered(Type type)
        {
            return this.IsRegistered(type, DefaultName);
        }

        private void AddRegistration(Type type, string name, ContainerRegistration registration)
        {
            if (!this.registrations.ContainsKey(type))
                this.registrations[type] = new Dictionary<string, ContainerRegistration>();

            this.registrations[type][name] = registration;
        }

        private void CheckRegistered(Type type, string name)
        {
            if (IsRegistered(type, name))
            {
                if (name == DefaultName)
                    throw new InvalidOperationException($"A type \"{type.Name}\" is already registered");
                else
                    throw new InvalidOperationException($"A type \"{type.Name}\" with the name \"{name}\" is already registered");
            }
        }

        private void CheckNotRegistered(Type type, string name)
        {
            if (!IsRegistered(type, name))
            {
                if (name == DefaultName)
                    throw new InvalidOperationException($"Type \"{type.Name}\" not registered");
                else
                    throw new InvalidOperationException($"Type \"{type.Name}\" with the name \"{name}\" not registered");
            }
        }

        private TypeRegistrationOptions ProcessRegisterType(Type typeFrom, string name, Type typeTo)
        {
            this.CheckRegistered(typeFrom, name);

            var registration = new TypeRegistration(typeFrom, name, typeTo);
            var clearCacheForType = new Action<Type, string>((t, n) => this.singletonCache.Remove(t, n));
            var registrationOptions = new TypeRegistrationOptions(registration, clearCacheForType);

            this.AddRegistration(typeFrom, name, registration);

            this.RaiseRegistered(registration);

            return registrationOptions;
        }

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <param name="name">The name / key</param>
        /// <param name="typeTo">The implementation type</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions RegisterType(Type typeFrom, string name, Type typeTo)
        {
            if (typeFrom == null)
                throw new ArgumentNullException(nameof(typeFrom));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (typeTo == null)
                throw new ArgumentNullException(nameof(typeTo));

            return this.ProcessRegisterType(typeFrom, name, typeTo);
        }

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <param name="typeTo">The implementation type</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo)
        {
            if (typeFrom == null)
                throw new ArgumentNullException(nameof(typeFrom));
            if (typeTo == null)
                throw new ArgumentNullException(nameof(typeTo));

            return this.ProcessRegisterType(typeFrom, DefaultName, typeTo);
        }

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions RegisterType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return this.ProcessRegisterType(type, DefaultName, type);
        }

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions RegisterType(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return this.ProcessRegisterType(type, name, type);
        }

        private InstanceRegistrationOptions ProcessRegisterInstance(Type type, string name, object instance)
        {
            this.CheckRegistered(type, name);

            var registration = new InstanceRegistration(type, name, instance);
            var registrationOptions = new InstanceRegistrationOptions(registration);

            this.AddRegistration(type, name, registration);

            this.RaiseRegistered(registration);

            return registrationOptions;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
        public InstanceRegistrationOptions RegisterInstance(Type type, string name, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return this.ProcessRegisterInstance(type, name, instance);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
        public InstanceRegistrationOptions RegisterInstance(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return this.ProcessRegisterInstance(type, DefaultName, instance);
        }

        private FactoryRegistrationOptions ProcessRegisterFactory(Type type, string name, Func<object> factory)
        {
            this.CheckRegistered(type, name);

            var registration = new FactoryRegistration(type, name, factory);
            var registrationOptions = new FactoryRegistrationOptions(registration);

            this.AddRegistration(type, name, registration);

            this.RaiseRegistered(registration);

            return registrationOptions;
        }

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        public FactoryRegistrationOptions RegisterFactory(Type type, string name, Func<object> factory)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return this.ProcessRegisterFactory(type, name, factory);
        }

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        public FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return this.ProcessRegisterFactory(type, DefaultName, factory);
        }

        /// <summary>
        /// Unregisters all registrations for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if unregistered</returns>
        public bool UnregisterAll(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (this.registrations.TryGetValue(type, out Dictionary<string, ContainerRegistration> containerRegistrations))
            {
                if (this.registrations.TryRemove(type, out containerRegistrations))
                {
                    if (this.singletonCache.Cache.ContainsKey(type))
                        this.singletonCache.Cache.Remove(type);

                    if (this.typeInformationManager.TypeCache.ContainsKey(type))
                        this.typeInformationManager.TypeCache.Remove(type);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unregisters the registration for the type with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>True if unregistered</returns>
        public bool Unregister(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (this.IsRegistered(type, name))
            {
                this.registrations[type].Remove(name);

                if (this.singletonCache.IsCached(type, name))
                    this.singletonCache.Remove(type, name);

                if (this.typeInformationManager.TypeCache.ContainsKey(type))
                    this.typeInformationManager.TypeCache.Remove(type);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Unregisters the registration for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if unregistered</returns>
        public bool Unregister(Type type)
        {
            return this.Unregister(type, DefaultName);
        }

        private void RaiseRegistered(ContainerRegistration registration)
        {
            var context = new RegistrationEventArgs(registration);
            foreach (var handler in this.registered)
                handler(this, context);
        }

        #endregion // Registration

        #region Resolution

        private bool IsValueContainerType(Type type)
        {
            return ValueContainer.IsValueContainerType(type);
        }

        private void TryRegisterTypeIfNotRegistered(Type type, string name)
        {
            if (!IsRegistered(type, name) && autoDiscovery)
            {
                if (type.IsInterface)
                {
                    var implementationType = scannedTypeManager.FindImplementationType(type);
                    if (implementationType == null)
                        throw new ResolutionFailedException($"Unable to resolve the type for the interface not registered \"{type.Name}\"");

                    RegisterType(type, name, implementationType);
                }

                if (IsValueContainerType(type))
                    throw new ResolutionFailedException($"Invalid registration type \"{type.Name}\"");

                RegisterType(type, name);
            }
        }

        private bool IsSingletonCached(Type type, string name)
        {
            return this.singletonCache.IsCached(type, name);
        }

        private object GetSingletonFromCache(Type type, string name)
        {
            return this.singletonCache.Cache[type][name];
        }

        private object GetFunction(Type type)
        {
            var returnType = type.GetGenericArguments()[0];

            var method = typeof(IInjectorResolverExtensions).GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IInjectorResolver) }, null);
            method = method.MakeGenericMethod(returnType);

            var expressionCall = Expression.Call(null, method, Expression.Constant(this));
            var compiled = Expression.Lambda(expressionCall).Compile();
            return compiled;
        }

        private object ResolveParameterValue(TypeRegistration registration, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var parameterName = parameter.Name;

            // Func<object> ?
            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Func<>))
                return GetFunction(parameterType);

            // value ?
            if (registration.ValueContainer.ContainsKey(parameterName)) // myString
                return registration.ValueContainer[parameterName]; // "My Value"

            // registered type or instance or factory ?
            if (this.registrations.ContainsKey(parameterType))
            {
                // return last
                var registrationEntry = this.registrations[parameterType].Last();
                return this.DoGetInstance(registrationEntry.Value, parameterType, registrationEntry.Key);
            }
            else
            {
                // not registered
                if (autoDiscovery)
                {
                    if (IsValueContainerType(parameterType))
                    {
                        object value = null;
                        if (parameterType.IsValueType)
                            // get default value with factory ??
                            value = Activator.CreateInstance(parameterType);

                        registration.ValueContainer[parameterName] = value;
                        return value;
                    }
                    else
                    {
                        this.RegisterType(parameterType);
                        var registrationEntry = this.registrations[parameterType].Last();
                        return this.DoGetInstance(registrationEntry.Value, parameterType, registrationEntry.Key);
                    }
                }
                else
                {
                    if (IsValueContainerType(parameterType))
                        throw new ResolutionFailedException($"Unable to resolve unregistered parameter \"{parameterName}\"");
                    else
                        throw new ResolutionFailedException($"Unable to resolve unregistered parameter \"{parameterType.Name}\"");
                }
            }
        }

        private object[] ResolveParameterValues(TypeRegistration registration, ParameterInfo[] parameters)
        {
            var parameterValues = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                parameterValues[i] = this.ResolveParameterValue(registration, parameters[i]);

            return parameterValues;
        }

        private object DoGetNewInstance(TypeRegistration registration)
        {
            var type = registration.TypeTo;

            // get constructor and parameters
            var typeInformation = this.typeInformationManager.GetTypeInformation(type, NonPublicConstructors);
            if (typeInformation.Parameters.Length > 0)
            {
                // values to injected
                var parameterValues = this.ResolveParameterValues(registration, typeInformation.Parameters);
                var instance = this.objectCreationManager.CreateInstanceWithParameterizedConstructor(type, typeInformation.Constructor, parameterValues);
                this.singletonCache.TryAddToCache(registration, instance);
                this.OnResolvedForRegistration(registration, instance);
                this.OnResolved(registration, instance);
                return instance;
            }
            else
            {
                var instance = this.objectCreationManager.CreateInstanceWithEmptyConstructor(type, typeInformation.Constructor);
                this.singletonCache.TryAddToCache(registration, instance);
                this.OnResolvedForRegistration(registration, instance);
                this.OnResolved(registration, instance);
                return instance;
            }
        }

        private object DoGetInstance(ContainerRegistration registration, Type type, string name)
        {
            if (registration is TypeRegistration typeRegistration)
            {
                var implementedType = typeRegistration.TypeTo;
                if (typeRegistration.IsSingleton && this.IsSingletonCached(implementedType, name))
                {
                    var instance = this.GetSingletonFromCache(implementedType, name);
                    this.OnResolved(registration, instance);
                    return instance;
                }
                else
                {
                    var instance = DoGetNewInstance(typeRegistration);
                    return instance;
                }
            }
            else if (registration is InstanceRegistration instanceRegistration)
            {
                var instance = instanceRegistration.Instance;
                this.OnResolvedForRegistration(registration, instance);
                this.OnResolved(instanceRegistration, instance);
                return instance;
            }
            else if (registration is FactoryRegistration factoryRegistration)
            {
                var instance = factoryRegistration.Factory();
                this.OnResolvedForRegistration(registration, instance);
                this.OnResolved(factoryRegistration, instance);
                return instance;
            }

            throw new ResolutionFailedException("Unexpected ContainerRegistrationType");
        }

        /// <summary>
        /// Gets a new instance for the type and name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        public object GetNewInstance(Type type, string name)
        {
            this.TryRegisterTypeIfNotRegistered(type, name);

            this.CheckNotRegistered(type, name);

            var registration = registrations[type][name];
            if (registration is TypeRegistration typeRegistration)
            {
                var instance = DoGetNewInstance(typeRegistration);
                return instance;
            }
            else
                throw new ResolutionFailedException($"Cannot get a new instance for the registration type \"{registration.GetType().Name}\"");
        }

        /// <summary>
        /// Gets a new instance for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance</returns>
        public object GetNewInstance(Type type)
        {
            return this.GetNewInstance(type, DefaultName);
        }

        /// <summary>
        /// Gets the instance for the type and name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        public object GetInstance(Type type, string name)
        {
            this.TryRegisterTypeIfNotRegistered(type, name);

            this.CheckNotRegistered(type, name);

            var registration = registrations[type][name];
            var instance = this.DoGetInstance(registration, type, name);
            return instance;
        }

        /// <summary>
        /// Gets the instance for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance</returns>
        public object GetInstance(Type type)
        {
            return this.GetInstance(type, DefaultName);
        }

        /// <summary>
        /// Gets all instances of the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The list of instances</returns>
        public List<object> GetAllInstances(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (this.registrations.ContainsKey(type))
            {
                var instances = new List<object>();

                var registrationsOfType = this.registrations[type].Values;
                foreach (var registration in registrationsOfType)
                {
                    var instance = DoGetInstance(registration, type, registration.Name);
                    instances.Add(instance);
                }
                return instances;
            }
            return null;
        }

        private object DoBuildUp(Type type, string name, object instance)
        {
            var registration = registrations[type][name];

            var propertyWithDependencyAttributes = this.typeInformationManager.GetPropertiesWithDependencyAttribute(type, nonPublicProperties);
            foreach (var propertyWithDepencyAttribute in propertyWithDependencyAttributes)
            {
                var property = propertyWithDepencyAttribute.Property;
                if (IsValueContainerType(property.PropertyType))
                {
                    // value contianer type
                    var propertyWithDepencyAttributeName = propertyWithDepencyAttribute.Name ?? property.Name;
                    var typeRegistration = registration as TypeRegistration;

                    if (typeRegistration != null && typeRegistration.ValueContainer.ContainsKey(propertyWithDepencyAttributeName)) // myString
                    {
                        var value = typeRegistration.ValueContainer[propertyWithDepencyAttributeName]; // "My Value"
                        property.SetValue(instance, value);
                    }
                }
                else
                {
                    // Type, instance, factory
                    var propertyWithDepencyAttributeName = propertyWithDepencyAttribute.Name ?? DefaultName;
                    var value = this.GetInstance(property.PropertyType, propertyWithDepencyAttributeName);
                    property.SetValue(instance, value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Fills the properties of the instance with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        public object BuildUp(Type type, string name, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return this.DoBuildUp(type, name, instance);
        }

        /// <summary>
        /// Fills the properties of the instance.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        public object BuildUp(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var name = DefaultName;

            this.TryRegisterTypeIfNotRegistered(type, name);

            return this.DoBuildUp(type, name, instance);
        }

        /// <summary>
        /// Fills the properties of an instance with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>The instance filled</returns>
        public object BuildUp(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var instance = this.GetInstance(type, name);
            return this.DoBuildUp(type, name, instance);
        }

        /// <summary>
        /// Fills the properties of an instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The instance filled</returns>
        public object BuildUp(Type type)
        {
            return this.BuildUp(type, DefaultName);
        }

        private void OnResolvedForRegistration(ContainerRegistration registration, object instance)
        {
            registration.OnResolved?.Invoke(registration, instance);
        }

        private void OnResolved(ContainerRegistration registration, object instance)
        {
            var context = new ResolutionEventArgs(registration, instance);
            foreach (var handler in this.resolved)
                handler(this, context);
        }

        #endregion // Resolution

    }
}
