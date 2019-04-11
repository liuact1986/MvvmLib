using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.IoC
{
    public class Injector : IInjector
    {
        private IObjectCreationManager objectCreationManager;
        private AutoDiscover autoDiscover;
        private TypeInformationManager typeInformationManager;
        private InstancesCache instancesCache;

        private Dictionary<Type, Dictionary<string, ContainerRegistration>> registrations
            = new Dictionary<Type, Dictionary<string, ContainerRegistration>>();

        public bool AutoDiscovery { get; set; }

        public bool NonPublicConstructors { get; set; }

        public bool NonPublicProperties { get; set; }

        public DelegateFactoryType DelegateFactoryType
        {
            get => this.objectCreationManager.DelegateFactoryType;
            set => this.objectCreationManager.DelegateFactoryType = value;
        }

        private readonly List<EventHandler<InjectorRegistrationEventArgs>> registered;
        public event EventHandler<InjectorRegistrationEventArgs> Registered
        {
            add { if (!registered.Contains(value)) registered.Add(value); }
            remove { if (registered.Contains(value)) registered.Remove(value); }
        }

        private readonly List<EventHandler<InjectorResolveEventArgs>> resolved;
        public event EventHandler<InjectorResolveEventArgs> Resolved
        {
            add { if (!resolved.Contains(value)) resolved.Add(value); }
            remove { if (resolved.Contains(value)) resolved.Remove(value); }
        }

        private static readonly object _instanceLock = new object();

        private static Injector _default;
        public static Injector Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_instanceLock)
                    {
                        if (_default == null)
                        {
                            _default = new Injector();
                        }
                    }
                }

                return _default;
            }
        }

        public Injector()
          : this(new ObjectCreationManager())
        { }

        public Injector(IObjectCreationManager objectCreationManager)
        {
            this.registered = new List<EventHandler<InjectorRegistrationEventArgs>>();
            this.resolved = new List<EventHandler<InjectorResolveEventArgs>>();

            this.AutoDiscovery = true;
            this.NonPublicConstructors = true;
            this.NonPublicProperties = true;

            this.objectCreationManager = objectCreationManager;
            this.autoDiscover = new AutoDiscover(this);
            this.typeInformationManager = new TypeInformationManager();
            this.instancesCache = new InstancesCache();
        }

        #region Register

        private void AddOrUpdateRegistration(Type type, ContainerRegistration registration, string name)
        {
            if (!this.registrations.ContainsKey(type))
            {
                this.registrations[type] = new Dictionary<string, ContainerRegistration>();
            }

            this.registrations[type][name] = registration;
        }

        public bool IsRegistered(Type type, string name)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            return this.registrations.ContainsKey(type)
               && this.registrations[type].ContainsKey(name);
        }

        public bool IsRegistered(Type type)
        {
            return this.IsRegistered(type, MvvmLibConstants.DefaultName);
        }

        public TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo, string name)
        {
            if (typeFrom == null) { throw new ArgumentNullException(nameof(typeFrom)); }
            if (typeTo == null) { throw new ArgumentNullException(nameof(typeTo)); }
            if (!typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo())) { throw new RegistrationFailedException(typeFrom.Name + " is not assignable from " + typeTo.Name); }
            if (this.IsRegistered(typeFrom, name)) { throw new RegistrationFailedException("The type \"" + typeFrom.Name + "\" with the name \"" + name + "\" is already registered"); }

            var registration = new TypeRegistration(typeFrom, typeTo, name);
            var options = new TypeRegistrationOptions(this, registration);

            this.AddOrUpdateRegistration(typeFrom, registration, name);
            this.RaiseRegistered(registration);

            return options;
        }

        public TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo)
        {
            return this.RegisterType(typeFrom, typeTo, MvvmLibConstants.DefaultName);
        }

        public TypeRegistrationOptions RegisterType(Type type, string name)
        {
            return this.RegisterType(type, type, name);
        }

        public TypeRegistrationOptions RegisterType(Type type)
        {
            return this.RegisterType(type, type, MvvmLibConstants.DefaultName);
        }

        public InstanceRegistrationOptions RegisterInstance(Type type, object instance, string name)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (this.IsRegistered(type, name)) { throw new RegistrationFailedException("An instance of type \"" + type.Name + "\" with the name \"" + name + "\" is already registered"); }

            var registration = new InstanceRegistration(type, instance, name);
            var options = new InstanceRegistrationOptions(this, registration);

            this.AddOrUpdateRegistration(type, registration, name);
            this.RaiseRegistered(registration);

            return options;
        }

        public InstanceRegistrationOptions RegisterInstance(Type type, object instance)
        {
            return this.RegisterInstance(type, instance, MvvmLibConstants.DefaultName);
        }

        public FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory, string name)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (factory == null) { throw new ArgumentNullException(nameof(factory)); }
            if (this.IsRegistered(type, name)) { throw new RegistrationFailedException("The type \"" + type.Name + "\" with the name \"" + name + "\" is already registered"); }

            var registration = new FactoryRegistration(type, factory, name);
            var options = new FactoryRegistrationOptions(this, registration);

            this.AddOrUpdateRegistration(type, registration, name);
            this.RaiseRegistered(registration);

            return options;
        }

        public FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory)
        {
            return this.RegisterFactory(type, factory, MvvmLibConstants.DefaultName);
        }

        internal void RemoveFromCache(Type type, string name)
        {
            this.instancesCache.Remove(type, name);
        }

        public bool UnregisterAll(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (this.IsRegistered(type))
            {
                this.registrations.Remove(type);

                if (this.instancesCache.IsTypeCached(type))
                {
                    this.instancesCache.Remove(type);
                }
                return true;
            }
            return false;
        }

        public bool Unregister(Type type, string name)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (this.IsRegistered(type, name))
            {
                this.registrations[type].Remove(name);

                if (this.instancesCache.IsCached(type, name))
                {
                    this.instancesCache.Remove(type, name);
                }
                if (this.typeInformationManager.ContainsKey(type))
                {
                    this.typeInformationManager.Remove(type);
                }
                return true;
            }
            return false;
        }

        public bool Unregister(Type type)
        {
            return this.Unregister(type, MvvmLibConstants.DefaultName);
        }

        private void RaiseRegistered(ContainerRegistration registration)
        {
            var context = new InjectorRegistrationEventArgs(registration);
            foreach (var handler in this.registered)
            {
                handler(this, context);
            }
        }

        #endregion // Register


        #region Resolve

        public bool IsCached(Type type, string name)
        {
            return this.instancesCache.IsCached(type, name);
        }

        public bool IsCached(Type type)
        {
            return this.IsCached(type, MvvmLibConstants.DefaultName);
        }

        public object GetFromCache(Type type, string name)
        {
            return this.instancesCache.GetFromCache(type, name);
        }

        public object GetFromCache(Type type)
        {
            return this.GetFromCache(type, MvvmLibConstants.DefaultName);
        }

        private object GetFunc(Type type)
        {
            var returnType = type.GetGenericArguments()[0];

            var method = ReflectionUtils.GetStaticMethod(typeof(IInjectorResolverExtensions), "GetInstance", new Type[] { typeof(IInjectorResolver) });
            method = method.MakeGenericMethod(returnType);

            var expressionCall = Expression.Call(null, method, Expression.Constant(this));
            var compiled = Expression.Lambda(expressionCall).Compile();
            return compiled;
        }

        private object ResolveParameterValue(TypeRegistration registration, ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            if (ReflectionUtils.IsFunc(parameterType))
            {
                return GetFunc(parameterType);
            }

            // value ?
            if (registration.HasValue(parameter.Name)) // myString
            {
                return registration.GetValue(parameter.Name); // "My Value"
            }
            // registered type / instance ?
            if (this.registrations.ContainsKey(parameterType))
            {
                // return last
                var registrationEntry = this.registrations[parameterType].Last();
                return this.DoGetInstance(registrationEntry.Value, parameterType, registrationEntry.Key);
            }
            else
            {
                // not registered

                if (this.AutoDiscovery)
                {
                    if (!ValueContainer.IsTypeSupported(parameterType))
                    {
                        this.RegisterType(parameterType);
                        var registrationEntry = this.registrations[parameterType].Last();
                        return this.DoGetInstance(registrationEntry.Value, parameterType, registrationEntry.Key);
                    }
                    else
                    {
                        object value = null;
                        if (parameterType.IsValueType)
                        {
                            // get default value with factory ??
                            value = Activator.CreateInstance(parameterType);
                        }
                        registration.ValueContainer.RegisterValue(parameter.Name, value);
                        return value;
                    }
                }
                else
                {
                    throw new ResolutionFailedException("Cannot resolve unregistered parameter \"" + parameterType.Name + "\"");
                }
            }

        }

        private object[] ResolveParameterValues(TypeRegistration registration, ParameterInfo[] parameters)
        {
            var parameterValues = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = this.ResolveParameterValue(registration, parameters[i]);
            }
            return parameterValues;
        }

        private object DoGetNewInstance(TypeRegistration registration)
        {
            var type = registration.TypeTo;

            // get constructor and parameters
            var typeInfo = this.typeInformationManager.GetTypeInformation(type, NonPublicConstructors);
            if (typeInfo.Parameters.Length > 0)
            {
                // values to injected
                var parameterValues = this.ResolveParameterValues(registration, typeInfo.Parameters);
                var instance = this.objectCreationManager.CreateInstance(type, typeInfo.Constructor, parameterValues);
                this.instancesCache.TryAddToCache(registration, instance);
                this.RaiseResolved(registration, instance);
                return instance;
            }
            else
            {
                var instance = this.objectCreationManager.CreateInstance(type, typeInfo.Constructor);
                this.instancesCache.TryAddToCache(registration, instance);
                this.RaiseResolved(registration, instance);
                return instance;
            }
        }

        private object DoGetInstance(ContainerRegistration registration, Type type, string name)
        {
            switch (registration.ContainerRegistrationType)
            {
                case ContainerRegistrationType.Type:
                    if (this.IsCached(type, name))
                    {
                        var instanceCached = this.GetFromCache(type, name);
                        this.RaiseResolved(registration, instanceCached);
                        return instanceCached;
                    }
                    else
                    {
                        var typeRegistration = registration as TypeRegistration;
                        return DoGetNewInstance(typeRegistration);
                    }
                case ContainerRegistrationType.Instance:
                    var instanceRegistration = registration as InstanceRegistration;
                    this.RaiseResolved(registration, instanceRegistration.Instance);
                    return instanceRegistration.Instance;
                case ContainerRegistrationType.Factory:
                    var factoryRegistration = registration as FactoryRegistration;
                    var result = factoryRegistration.Factory.Invoke();
                    this.RaiseResolved(registration, result);
                    return result;
                default:
                    throw new ResolutionFailedException("Unexpected ContainerRegistrationType");
            }
        }

        public object GetInstance(Type type, string name)
        {
            this.autoDiscover.CheckRegistered(type, name);

            var registration = registrations[type][name];
            return DoGetInstance(registration, type, name);
        }

        public object GetInstance(Type type)
        {
            return this.GetInstance(type, MvvmLibConstants.DefaultName);
        }

        public object GetNewInstance(Type type, string name)
        {
            this.autoDiscover.CheckRegistered(type, name);

            var registration = registrations[type][name];
            if (registration.ContainerRegistrationType == ContainerRegistrationType.Type)
            {
                var typeRegistration = registration as TypeRegistration;
                return DoGetNewInstance(typeRegistration);
            }
            else
            {
                throw new ResolutionFailedException("Cannot get a new instance for the registration type \"" + registration.ContainerRegistrationType.ToString() + "\"");
            }
        }

        public object GetNewInstance(Type type)
        {
            return this.GetNewInstance(type, MvvmLibConstants.DefaultName);
        }

        private object DoBuildUp(Type type, string name, object instance)
        {
            var registration = registrations[type][name];

            var properties = ReflectionUtils.GetProperties(type, NonPublicProperties);
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var attribute = property.GetCustomAttribute(typeof(DependencyAttribute)) as DependencyAttribute;
                    if (attribute != null)
                    {
                        if (ValueContainer.IsTypeSupported(property.PropertyType))
                        {
                            // value
                            var nameOrPropertyName = attribute.Name != null ? attribute.Name : property.Name;
                            var typeRegistration = registration as TypeRegistration;
                            if (typeRegistration != null && typeRegistration.HasValue(nameOrPropertyName)) // myString
                            {
                                var value = typeRegistration.GetValue(nameOrPropertyName); // "My Value"
                                property.SetValue(instance, value);
                            }
                        }
                        else
                        {
                            // Type, Factory, Instance
                            var value = attribute.Name != null ?
                                this.GetInstance(property.PropertyType, attribute.Name)
                                : this.GetInstance(property.PropertyType);
                            property.SetValue(instance, value);
                        }
                    }
                }
            }

            return instance;
        }

        public object BuildUp(object instance)
        {
            if (instance == null) { throw new ArgumentNullException(nameof(instance)); }

            var type = instance.GetType();
            var name = MvvmLibConstants.DefaultName;
            this.autoDiscover.CheckRegistered(type, name);

            return this.DoBuildUp(instance.GetType(), name, instance);
        }

        public object BuildUp(Type type, string name)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            var instance = this.GetInstance(type, name);
            return this.DoBuildUp(type, name, instance);
        }

        public object BuildUp(Type type)
        {
            return this.BuildUp(type, MvvmLibConstants.DefaultName);
        }

        public List<object> GetAllInstances(Type type)
        {
            if (this.registrations.ContainsKey(type))
            {
                var result = new List<object>();

                var registrationsOfType = this.registrations[type].Values;
                foreach (var registration in registrationsOfType)
                {
                    var instance = DoGetInstance(registration, type, registration.Name);
                    result.Add(instance);
                }

                return result;
            }
            return null;
        }

        public void ClearCache()
        {
            this.instancesCache.Clear();
            this.typeInformationManager.Clear();
        }

        public void Clear()
        {
            this.registrations.Clear();
            this.ClearCache();
        }

        private void RaiseResolved(ContainerRegistration registration, object instance)
        {
            registration.OnResolved?.Invoke(registration, instance);

            var context = new InjectorResolveEventArgs(registration, instance);
            foreach (var handler in this.resolved)
            {
                handler(this, context);
            }
        }

        #endregion // Resolve
    }

}