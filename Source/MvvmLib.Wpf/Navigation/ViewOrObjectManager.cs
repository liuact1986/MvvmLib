using System;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public enum ResolutionType
    {
        Singleton,
        Existing,
        New
    }

    public struct ViewOrObjectInstanceResult
    {
        private readonly Type sourceType;
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly ResolutionType resolutionType;
        public ResolutionType ResolutionType
        {
            get { return resolutionType; }
        }

        private readonly object instance;
        public object Instance
        {
            get { return instance; }
        }

        public ViewOrObjectInstanceResult(Type sourceType, object instance, ResolutionType resolutionType)
        {
            this.sourceType = sourceType;
            this.instance = instance;
            this.resolutionType = resolutionType;
        }
    }


    /// <summary>
    /// Allows to manage view or object instances for regions.
    /// </summary>
    public class ViewOrObjectManager
    {
        private readonly Dictionary<Type, object> singletons;

        /// <summary>
        /// Registered View or objects as Singletons that implement <see cref="IViewLifetimeStrategy"/>.
        /// </summary>
        public IReadOnlyDictionary<Type, object> Singletons
        {
            get { return singletons; }
        }

        private readonly Dictionary<Type, List<KeyValuePair<object, object>>> selectables;
       
        /// <summary>
        /// Registered View or objects that implement <see cref="ISelectable"/>.
        /// </summary>
        public IReadOnlyDictionary<Type, List<KeyValuePair<object, object>>> Selectables
        {
            get { return selectables; }
        }

        /// <summary>
        /// Creates the view or object maanager.
        /// </summary>
        public ViewOrObjectManager()
        {
            singletons = new Dictionary<Type, object>();
            selectables = new Dictionary<Type, List<KeyValuePair<object, object>>>();
        }

        /// <summary>
        /// Add view or object that implements <see cref="IViewLifetimeStrategy"/> or <see cref="ISelectable"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="viewOrObject">The instance</param>
        /// <param name="context">The context</param>
        public void TryAddViewOrObject(Type sourceType, object viewOrObject, object context)
        {
            bool isSingleton = false;
            // singleton 
            if (viewOrObject is IViewLifetimeStrategy p)
            {
                if (p.Strategy == StrategyType.Singleton)
                {
                    isSingleton = true;
                    singletons[sourceType] = viewOrObject;
                }
            }
            else if (context != null && context is IViewLifetimeStrategy p2)
            {
                if (p2.Strategy == StrategyType.Singleton)
                {
                    isSingleton = true;
                    singletons[sourceType] = viewOrObject;
                }
            }

            if (!isSingleton)
            {
                if (context != null && context is ISelectable)
                {
                    if (!selectables.ContainsKey(sourceType))
                        selectables[sourceType] = new List<KeyValuePair<object, object>>();

                    selectables[sourceType].Add(new KeyValuePair<object, object>(viewOrObject, context));
                }
            }
        }

        /// <summary>
        /// Tries to get a singleton for the type.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The singleton found or null</returns>
        public object TryGetSingleton(Type sourceType)
        {
            if (singletons.TryGetValue(sourceType, out object viewSingleton))
                return viewSingleton;

            return null;
        }

        /// <summary>
        /// Tries to get a registered selectable for the type.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The selectable found or null</returns>
        public object TryGetSelectable(Type sourceType, object parameter)
        {
            if (selectables.TryGetValue(sourceType, out List<KeyValuePair<object, object>> instances))
            {
                foreach (var instance in instances)
                {
                    var context = instance.Value;
                    if (context != null && context is ISelectable p)
                        if (p.IsTarget(sourceType, parameter))
                            return instance.Key; // view
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an new instance of the type.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The new instance</returns>
        public object CreateInstance(Type sourceType)
        {
            var viewOrObject = ViewResolver.CreateInstance(sourceType);
            return viewOrObject;
        }

        /// <summary>
        /// Gets a singleton, a selectable or an new instance.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The instance with <see cref="ResolutionType"/></returns>
        public ViewOrObjectInstanceResult GetOrCreateViewOrObjectInstance(Type sourceType, object parameter)
        {
            var singleton = TryGetSingleton(sourceType);
            if (singleton != null)
                return new ViewOrObjectInstanceResult(sourceType, singleton, ResolutionType.Singleton);


            var selectable = TryGetSelectable(sourceType, parameter);
            if (selectable != null)
                return new ViewOrObjectInstanceResult(sourceType, selectable, ResolutionType.Existing);

            var newViewOrObject = CreateInstance(sourceType);
            return new ViewOrObjectInstanceResult(sourceType, newViewOrObject, ResolutionType.New);
        }

        /// <summary>
        /// Removes the selectable.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if removed</returns>
        public bool RemoveSelectable(Type sourceType)
        {
            if (selectables.ContainsKey(sourceType))
                return selectables.Remove(sourceType);

            return false;
        }
    }
}
