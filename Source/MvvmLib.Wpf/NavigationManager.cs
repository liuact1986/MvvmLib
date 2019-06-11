using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Navigation Manager class. Allows to create <see cref="NavigationSourceContainer"/> for <see cref="ContentControl"/> and <see cref="SharedSource{T}"/> for <see cref="ItemsControl"/>, ListBox, <see cref="TabControl"/>, etc.
    /// </summary>
    public class NavigationManager
    {
        /// <summary>
        /// The default key used for <see cref="KeyedNavigationSource"/>.
        /// </summary>
        public const string DefaultKeyedNavigationSourceKey = "__Default__";

        private static readonly Dictionary<string, NavigationSourceContainer> allNavigationSources;
        /// <summary>
        /// The navigation sources.
        /// </summary>
        public static IReadOnlyDictionary<string, NavigationSourceContainer> AllNavigationSources
        {
            get { return allNavigationSources; }
        }

        private static readonly Dictionary<Type, ISharedSource> sharedSources;
        /// <summary>
        /// The shared sources.
        /// </summary>
        public static IReadOnlyDictionary<Type, ISharedSource> SharedSources
        {
            get { return sharedSources; }
        }

        private static readonly Dictionary<Type, Dictionary<string, ISharedSource>> keyedSharedSources;
        /// <summary>
        /// The keyed shared sources.
        /// </summary>
        public static IReadOnlyDictionary<Type, Dictionary<string, ISharedSource>> KeyedSharedSources
        {
            get { return keyedSharedSources; }
        }

        /// <summary>
        /// Gets the navigation source name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSourceName(DependencyObject obj)
        {
            return (string)obj.GetValue(SourceNameProperty);
        }

        /// <summary>
        /// Sets the naigation source.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSourceName(DependencyObject obj, string value)
        {
            obj.SetValue(SourceNameProperty, value);
        }

        /// <summary>
        /// Allows to create a <see cref="ContentControlNavigationSource"/> with the attached property.
        /// </summary>
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.RegisterAttached("SourceName", typeof(string), typeof(NavigationManager), new PropertyMetadata(null, OnSourceNameChanged));

        private static void OnSourceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsInDesignMode(d))
            {
                if (!(d is ContentControl))
                    throw new InvalidOperationException($"Expected target for SourceName is a ContentControl. Current type {d.GetType()}\"");

                var control = (ContentControl)d;
                if (control.Content != null || (BindingOperations.GetBinding(control, ContentControl.ContentProperty) != null))
                    throw new InvalidOperationException("ContentControl is not empty or binded");

                var sourceName = (string)e.NewValue;
                if (sourceName == null)
                    throw new ArgumentException("A SourceName is required");

                RegisterNavigationSource(sourceName, new ContentControlNavigationSource(sourceName, control));
            }
        }

        static NavigationManager()
        {
            allNavigationSources = new Dictionary<string, NavigationSourceContainer>();
            sharedSources = new Dictionary<Type, ISharedSource>();
            keyedSharedSources = new Dictionary<Type, Dictionary<string, ISharedSource>>();
        }

        private static bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }

        /// <summary>
        /// Allows to register a <see cref="NavigationSource"/>. The navigation source is created if not registered.
        /// </summary>
        /// <param name="sourceName">The Navigation source name</param>
        /// <param name="navigationSource">The navigation source</param>
        public static void RegisterNavigationSource(string sourceName, NavigationSource navigationSource)
        {
            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                var existingNavigationSource = navigationSourceContainer[0];
                navigationSource.Sync(existingNavigationSource.History);
                navigationSourceContainer.Register(navigationSource);
            }
            else
            {
                navigationSourceContainer = new NavigationSourceContainer();
                navigationSourceContainer.Register(navigationSource);
                allNavigationSources[sourceName] = navigationSourceContainer;
            }
        }

        /// <summary>
        /// Gets the navigation source collection for the source name.
        /// </summary>
        /// <param name="sourceName">The Navigation source name</param>
        /// <returns>The navigation source collection or null</returns>
        public static NavigationSourceContainer GetNavigationSources(string sourceName)
        {
            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                return navigationSourceContainer;
            }
            return null;
        }

        /// <summary>
        /// Unregisters all navigation sources for the navigation source name.
        /// </summary>
        /// <param name="sourceName">The Navigation source name</param>
        /// <returns>True if removed</returns>
        public static bool UnregisterNavigationSources(string sourceName)
        {
            if (allNavigationSources.ContainsKey(sourceName))
            {
                var removed = allNavigationSources.Remove(sourceName);
                return removed;
            }
            return false;
        }

        /// <summary>
        /// Unregisters the navigation source from the <see cref="NavigationSourceContainer"/>.
        /// </summary>
        /// <param name="sourceName">The navigation source name</param>
        /// <param name="navigationSource">The navigation source</param>
        /// <returns>True if removed</returns>
        public static bool UnregisterNavigationSource(string sourceName, NavigationSource navigationSource)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                if (navigationSourceContainer.IsRegistered(navigationSource))
                {
                    return navigationSourceContainer.Unregister(navigationSource);
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the <see cref="NavigationSourceContainer"/> with a default <see cref="KeyedNavigationSource"/> with the default key <see cref="DefaultKeyedNavigationSourceKey"/>.
        /// </summary>
        /// <param name="sourceName">The navigation source name</param>
        /// <returns>The navigation source created</returns>
        public static NavigationSource CreateNavigationSource(string sourceName)
        {
            if (allNavigationSources.ContainsKey(sourceName))
                throw new ArgumentException($"A navigation source with the name \"{sourceName}\" is already registered");

            var navigationSources = new NavigationSourceContainer();

            var navigationSource = new KeyedNavigationSource(DefaultKeyedNavigationSourceKey);
            navigationSources.Register(navigationSource);
            allNavigationSources[sourceName] = navigationSources;

            return navigationSource;
        }

        /// <summary>
        /// Gets the first navigation source for the name.
        /// </summary>
        /// <param name="sourceName">The navigation source name</param>
        /// <returns>The navigation source found or null</returns>
        public static NavigationSource GetNavigationSource(string sourceName)
        {
            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                var navigationSource = navigationSourceContainer[0];
                return navigationSource;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the first navigation source for the name or creates a new navigation source.
        /// </summary>
        /// <param name="sourceName">The navigation source name</param>
        /// <returns>The navigation source found or created</returns>
        public static NavigationSource GetOrCreateNavigationSource(string sourceName)
        {
            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                var navigationSource = navigationSourceContainer[0];
                return navigationSource;
            }
            else
                return CreateNavigationSource(sourceName);
        }

        /// <summary>
        /// Gets or create a <see cref="SharedSource{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetOrCreateSharedSource<T>()
        {
            if (sharedSources.TryGetValue(typeof(T), out ISharedSource sharedSource))
            {
                return (SharedSource<T>)sharedSource;
            }
            else
            {
                sharedSource = new SharedSource<T>();
                sharedSources[typeof(T)] = sharedSource;
                return (SharedSource<T>)sharedSource;
            }
        }

        /// <summary>
        /// Gets the <see cref="SharedSource{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetSharedSource<T>()
        {
            if (sharedSources.TryGetValue(typeof(T), out ISharedSource sharedSource))
            {
                return (SharedSource<T>)sharedSource;
            }
            return null;
        }

        /// <summary>
        /// Gets or create a <see cref="SharedSource{T}"/> with a name / key.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <param name="name">The name</param>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetOrCreateSharedSource<T>(string name)
        {
            if (keyedSharedSources.TryGetValue(typeof(T), out Dictionary<string, ISharedSource> sharedSourcesOfType))
            {
                if (sharedSourcesOfType.TryGetValue(name, out ISharedSource sharedSource))
                {
                    return (SharedSource<T>)sharedSource;
                }
            }

            var newSharedSource = new SharedSource<T>();
            var type = typeof(T);
            if (!keyedSharedSources.ContainsKey(type))
                keyedSharedSources[type] = new Dictionary<string, ISharedSource>();

            keyedSharedSources[type][name] = newSharedSource;
            return (SharedSource<T>)newSharedSource;
        }

        /// <summary>
        /// Gets the <see cref="SharedSource{T}"/> with a name / key.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <param name="name">The name</param>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetSharedSource<T>(string name)
        {
            if (keyedSharedSources.TryGetValue(typeof(T), out Dictionary<string, ISharedSource> sharedSourcesOfType))
            {
                if (sharedSourcesOfType.TryGetValue(name, out ISharedSource sharedSource))
                {
                    return (SharedSource<T>)sharedSource;
                }
            }
            return null;
        }
    }
}
