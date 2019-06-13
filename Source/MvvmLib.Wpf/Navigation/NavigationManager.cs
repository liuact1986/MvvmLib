using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation manager allows to create <see cref="NavigationSource"/> (for <see cref="ContentControl"/>) and <see cref="SharedSource{T}"/> (for <see cref="ItemsControl"/>, <see cref="ListView"/>, <see cref="TabControl"/>, etc.).
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
        /// Sets the navigation source.
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
                AddNavigationSource(sourceName, new ContentControlNavigationSource(sourceName, control));
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

        #region NavigationSourceContainer

        /// <summary>
        /// Gets the <see cref="NavigationSourceContainer"/> for the source name.
        /// </summary>
        /// <param name="sourceName">The Navigation source name</param>
        /// <returns>The navigation source container or null</returns>
        public static NavigationSourceContainer GetNavigationSources(string sourceName)
        {
            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                return navigationSourceContainer;
            }
            return null;
        }

        /// <summary>
        /// Unregisters all navigation sources for the source name.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <returns>True if removed</returns>
        public static bool RemoveNavigationSources(string sourceName)
        {
            if (allNavigationSources.ContainsKey(sourceName))
            {
                var removed = allNavigationSources.Remove(sourceName);
                return removed;
            }
            return false;
        }

        #endregion // NavigationSourceContainer

        /// <summary>
        /// Tries to get the default <see cref="KeyedNavigationSource"/> with the default key. A <see cref="NavigationSourceContainer"/> is created if the <see cref="AllNavigationSources"/> collection does not contain the source name.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <returns>The navigation source or null</returns>
        public static NavigationSource GetDefaultNavigationSource(string sourceName)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));

            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                var navigationSource = navigationSourceContainer.NavigationSources.FirstOrDefault(n => n is KeyedNavigationSource && ((KeyedNavigationSource)n).Key == DefaultKeyedNavigationSourceKey);
                if (navigationSource != null)
                    return navigationSource;
            }
            return null;
        }

        /// <summary>
        /// Gets or creates the <see cref="KeyedNavigationSource"/> with the default key. A <see cref="NavigationSourceContainer"/> is created if the <see cref="AllNavigationSources"/> collection does not contain the source name.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <returns>The navigation source</returns>
        public static NavigationSource CreateDefaultNavigationSource(string sourceName)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));

            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                var navigationSource = navigationSourceContainer.NavigationSources.FirstOrDefault(n => n is KeyedNavigationSource && ((KeyedNavigationSource)n).Key == DefaultKeyedNavigationSourceKey);
                if (navigationSource != null)
                {
                    throw new InvalidOperationException("The navigation source is already created");
                }
                else
                {
                    var defaultNavigationSource = new KeyedNavigationSource(DefaultKeyedNavigationSourceKey);
                    navigationSourceContainer.Add(defaultNavigationSource);
                    allNavigationSources[sourceName] = navigationSourceContainer;
                    return defaultNavigationSource;
                }
            }
            else
            {
                navigationSourceContainer = new NavigationSourceContainer();
                var defaultNavigationSource = new KeyedNavigationSource(DefaultKeyedNavigationSourceKey);
                navigationSourceContainer.Add(defaultNavigationSource);
                allNavigationSources[sourceName] = navigationSourceContainer;
                return defaultNavigationSource;
            }
        }

        /// <summary>
        /// Creates the <see cref="KeyedNavigationSource"/> with the default key. A <see cref="NavigationSourceContainer"/> is created if the <see cref="AllNavigationSources"/> collection does not contain the source name.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <returns>The navigation source</returns>
        public static NavigationSource GetOrCreateDefaultNavigationSource(string sourceName)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));

            var defaultNavigationSource = GetDefaultNavigationSource(sourceName);
            if (defaultNavigationSource != null)
                return defaultNavigationSource;
            else
                return CreateDefaultNavigationSource(sourceName);
        }

        /// <summary>
        /// Adds the <see cref="NavigationSource"/>. The <see cref="NavigationSourceContainer"/> is created if the <see cref="AllNavigationSources"/> does not contain the source name.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <param name="navigationSource">The navigation source</param>
        public static void AddNavigationSource(string sourceName, NavigationSource navigationSource)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                navigationSourceContainer.Add(navigationSource);
            }
            else
            {
                navigationSourceContainer = new NavigationSourceContainer();
                navigationSourceContainer.Add(navigationSource);
                allNavigationSources[sourceName] = navigationSourceContainer;
            }
        }

        /// <summary>
        /// Removes the <see cref="NavigationSource"/> from the <see cref="NavigationSourceContainer"/>.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <param name="navigationSource">The navigation source</param>
        /// <returns>True if removed</returns>
        public static bool RemoveNavigationSource(string sourceName, NavigationSource navigationSource)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (allNavigationSources.TryGetValue(sourceName, out NavigationSourceContainer navigationSourceContainer))
            {
                if (navigationSourceContainer.Contains(navigationSource))
                {
                    return navigationSourceContainer.Remove(navigationSource);
                }
            }
            return false;
        }


        #region SharedSource

        /// <summary>
        /// Checks if a shared source is registered for the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>True if found</returns>
        public static bool ContainsSharedSource<T>()
        {
            return sharedSources.ContainsKey(typeof(T));
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
        /// Gets or create a <see cref="SharedSource{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetOrCreateSharedSource<T>()
        {
            var sharedSource = GetSharedSource<T>();
            if (sharedSource != null)
                return sharedSource;
            else
                return CreateSharedSource<T>();
        }

        /// <summary>
        /// Gets a new <see cref="SharedSource{T}"/>. The previous shared source is removed.
        /// </summary>
        /// <typeparam name="T">The type used as key</typeparam>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetNewSharedSource<T>()
        {
            RemoveSharedSource<T>();
            return CreateSharedSource<T>();
        }

        /// <summary>
        /// Creates the <see cref="SharedSource{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SharedSource<T> CreateSharedSource<T>()
        {
            if (ContainsSharedSource<T>())
                throw new InvalidOperationException($"A shared source source is already registered for '{typeof(T).Name}'");

            var sharedSource = new SharedSource<T>();
            sharedSources[typeof(T)] = sharedSource;
            return sharedSource;
        }


        /// <summary>
        /// Removes the shared source.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>True if removed</returns>
        public static bool RemoveSharedSource<T>()
        {
            var type = typeof(T);
            if (sharedSources.ContainsKey(type))
            {
                ((SharedSource<T>)sharedSources[type]).Clear();
                return sharedSources.Remove(type);
            }
            return false;
        }

        #endregion // SharedSource

        #region Keyed SharedSource

        /// <summary>
        /// Checks if a shared source is registered for the type and the key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>True if found</returns>
        public static bool ContainsSharedSource<T>(string key)
        {
            return KeyedSharedSources.ContainsKey(typeof(T))
                && keyedSharedSources[typeof(T)].ContainsKey(key);
        }

        /// <summary>
        /// Creates a <see cref="SharedSource{T}"/> with a key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns></returns>
        public static SharedSource<T> CreateSharedSource<T>(string key)
        {
            var sharedSource = new SharedSource<T>();
            var type = typeof(T);
            if (!keyedSharedSources.ContainsKey(type))
                keyedSharedSources[type] = new Dictionary<string, ISharedSource>();

            keyedSharedSources[type][key] = sharedSource;
            return sharedSource;
        }

        /// <summary>
        /// Gets the shared source.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The keyed shared source or null</returns>
        public static SharedSource<T> GetSharedSource<T>(string key)
        {
            if (keyedSharedSources.TryGetValue(typeof(T), out Dictionary<string, ISharedSource> sharedSourcesOfType))
            {
                if (sharedSourcesOfType.TryGetValue(key, out ISharedSource sharedSource))
                {
                    return (SharedSource<T>)sharedSource;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or create a <see cref="SharedSource{T}"/> with a key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The Shared Source</returns>
        public static SharedSource<T> GetOrCreateSharedSource<T>(string key)
        {
            var sharedSource = GetSharedSource<T>(key);
            if (sharedSource != null)
            {
                return sharedSource;
            }

            return CreateSharedSource<T>(key);
        }

        /// <summary>
        /// Gets a new <see cref="SharedSource{T}"/> with a key. The previous shared source is removed.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The shared source</returns>
        public static SharedSource<T> GetNewSharedSource<T>(string key)
        {
            RemoveSharedSource<T>(key);

            return CreateSharedSource<T>(key);
        }

        /// <summary>
        /// Removes the <see cref="SharedSource{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The key</param>
        /// <returns>True if removed</returns>
        public static bool RemoveSharedSource<T>(string key)
        {
            if (keyedSharedSources.TryGetValue(typeof(T), out Dictionary<string, ISharedSource> sharedSourcesOfType))
            {
                if (sharedSourcesOfType.TryGetValue(key, out ISharedSource sharedSource))
                {
                    ((SharedSource<T>)sharedSource).Clear();
                    var removed = sharedSourcesOfType.Remove(key);
                    if(sharedSourcesOfType.Count == 0)
                    {
                        keyedSharedSources.Remove(typeof(T));
                    }
                    return removed;
                }
            }
            return false;
        }

        #endregion // Keyed SharedSource
    }
}
