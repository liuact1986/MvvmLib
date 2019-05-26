using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Navigation Manager class. Allows to create <see cref="NavigationSource"/> for <see cref="ContentControl"/> and <see cref="SharedSource{T}"/> for <see cref="ItemsControl"/>, ListBox, <see cref="TabControl"/>, etc.
    /// </summary>
    public class NavigationManager
    {
        private static readonly Dictionary<string, NavigationSource> navigationSources;
        /// <summary>
        /// Gets the navigation sources.
        /// </summary>
        public static IReadOnlyDictionary<string, NavigationSource> NavigationSources
        {
            get { return navigationSources; }
        }

        private static readonly Dictionary<Type, ISharedSource> sharedSources;
        /// <summary>
        /// The shared sources.
        /// </summary>
        public static IReadOnlyDictionary<Type, ISharedSource> SharedSources
        {
            get { return sharedSources; }
        }

        private static Dictionary<Type, Dictionary<string, ISharedSource>> keyedSharedSources;
        /// <summary>
        /// The keyed shared sources.
        /// </summary>
        public static IReadOnlyDictionary<Type, Dictionary<string, ISharedSource>> KeyedSharedSources
        {
            get { return keyedSharedSources; }
        }

        /// <summary>
        /// Gets the source name.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>The source name</returns>
        public static string GetSourceName(DependencyObject obj)
        {
            return (string)obj.GetValue(SourceNameProperty);
        }

        /// <summary>
        /// Sets the source name for the dependency object.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <param name="value">The source name</param>
        public static void SetSourceName(DependencyObject obj, string value)
        {
            obj.SetValue(SourceNameProperty, value);
        }

        /// <summary>
        /// Allows to register a <see cref="ContentControlNavigationSource"/> with the attached property.
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

                var name = (string)e.NewValue;
                if (name == null)
                    throw new ArgumentException("A SourceName is required");

                RegisterNavigationSource(name, new ContentControlNavigationSource(name, control));
            }
        }

        static NavigationManager()
        {
            navigationSources = new Dictionary<string, NavigationSource>();
            sharedSources = new Dictionary<Type, ISharedSource>();
            keyedSharedSources = new Dictionary<Type, Dictionary<string, ISharedSource>>();
        }

        private static bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }

        /// <summary>
        /// Allows to register a custom Navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <param name="navigationSource">The navigation source</param>
        public static void RegisterNavigationSource(string name, NavigationSource navigationSource)
        {
            if (navigationSources.ContainsKey(name))
                throw new ArgumentException($"A navigation source with the name \"{name}\" is already registered");

            navigationSources[name] = navigationSource;
        }

        /// <summary>
        /// Creates the navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <returns>The navigation source created</returns>
        public static NavigationSource CreateNavigationSource(string name)
        {
            if (navigationSources.ContainsKey(name))
                throw new ArgumentException($"A navigation source with the name \"{name}\" is already registered");

            var navigationSource = new NavigationSource(name);
            navigationSources[name] = navigationSource;
            return navigationSource;
        }

        /// <summary>
        /// Gets an existing navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <returns>The navigation source found or null</returns>
        public static NavigationSource GetNavigationSource(string name)
        {
            if (navigationSources.TryGetValue(name, out NavigationSource navigationSource))
            {
                return navigationSource;
            }
            return null;
        }

        /// <summary>
        /// Gets an existing navigation source for the name or creates a new navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <returns>The navigation source found or created</returns>
        public static NavigationSource GetOrCreateNavigationSource(string name)
        {
            if (navigationSources.TryGetValue(name, out NavigationSource navigationSource))
                return navigationSource;
            else
                return CreateNavigationSource(name);
        }

        /// <summary>
        /// Removes the navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <returns>true if removed</returns>
        public static bool RemoveNavigationSource(string name)
        {
            if (navigationSources.ContainsKey(name))
            {
                var removed = navigationSources.Remove(name);
                return removed;
            }
            return false;
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

    }
}
