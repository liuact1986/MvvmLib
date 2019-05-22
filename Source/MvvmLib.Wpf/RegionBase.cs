using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MvvmLib.Navigation
{

    public class SelectableRegistration
    {
        private bool isView;
        /// <summary>
        /// Checks if selectable is view.
        /// </summary>
        public bool IsView
        {
            get { return isView; }
        }

        private readonly Type sourceType;
        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object viewOrObject;
        /// <summary>
        /// The view or object.
        /// </summary>
        public object ViewOrObject
        {
            get { return viewOrObject; }
        }

        private object context;
        /// <summary>
        /// The context.
        /// </summary>
        public object Context
        {
            get { return context; }
        }

        public SelectableRegistration(bool isView, Type sourceType, object viewOrObject, object context)
        {
            //ViewA, instance viewA, context viewAViewModel or null => is view true
            //ViewAViewModel, instance viewAViewModel, null (viewAViewModel) => isView false
            this.isView = isView;
            this.sourceType = sourceType;
            this.viewOrObject = viewOrObject;
            this.context = context;
        }
    }

    /// <summary>
    /// The Region base class.
    /// </summary>
    public abstract class RegionBase : IRegion
    {
        private readonly ILogger DefaultLogger = new DebugLogger();

        private ILogger logger;
        /// <summary>
        /// The logger used by the library.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        /// <summary>
        /// The regions registry used by the region.
        /// </summary>
        protected readonly RegionsRegistry regionsRegistry;

        /// <summary>
        /// The selectables.
        /// </summary>
        protected readonly Dictionary<Type, List<SelectableRegistration>> selectables;

        /// <summary>
        /// Checks if region is loaded.
        /// </summary>
        protected internal bool isLoaded;
        /// <summary>
        /// Checks if region is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        /// <summary>
        /// The region name.
        /// </summary>
        protected readonly string regionName;
        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// The control for this region.
        /// </summary>
        protected readonly FrameworkElement control;
        /// <summary>
        /// The control for this region.
        /// </summary>
        public FrameworkElement Control
        {
            get { return control; }
        }

        /// <summary>
        /// The name of the control. Can be used to get a region by region name and control name.
        /// </summary>
        public string ControlName
        {
            get { return control.Name; }
        }

        /// <summary>
        /// Gets the current entry of history.
        /// </summary>
        public abstract NavigationEntry CurrentEntry { get; }

        /// <summary>
        /// Navigating event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigating = new List<EventHandler<RegionNavigationEventArgs>>();
        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating
        {
            add { if (!navigating.Contains(value)) navigating.Add(value); }
            remove { if (navigating.Contains(value)) navigating.Remove(value); }
        }

        /// <summary>
        /// Navigated event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigated = new List<EventHandler<RegionNavigationEventArgs>>();
        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated
        {
            add { if (!navigated.Contains(value)) navigated.Add(value); }
            remove { if (navigated.Contains(value)) navigated.Remove(value); }
        }

        /// <summary>
        /// NavigationFailed event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationFailedEventArgs>> navigationFailed = new List<EventHandler<RegionNavigationFailedEventArgs>>();

        /// <summary>
        /// Invoked on navigation cancelled or on exception.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed
        {
            add { if (!navigationFailed.Contains(value)) navigationFailed.Add(value); }
            remove { if (navigationFailed.Contains(value)) navigationFailed.Remove(value); }
        }

        /// <summary>
        /// Invoked on view loaded.
        /// </summary>
        public event EventHandler<ViewLoadedEventArgs> ViewLoaded;

        /// <summary>
        /// Creates the region.
        /// </summary>
        /// <param name="regionsRegistry">The region registry</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public RegionBase(string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
        {
            this.selectables = new Dictionary<Type, List<SelectableRegistration>>();
            this.regionName = regionName;
            this.control = control;
            this.regionsRegistry = regionsRegistry;

            this.control.Loaded += OnControlLoaded;
            this.control.Unloaded += OnControlUnloaded;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            this.control.Loaded -= OnControlLoaded;
            this.isLoaded = true;
        }

        /// <summary>
        /// Invoked on control unload.
        /// </summary>
        /// <param name="sender">The control</param>
        /// <param name="e">The routed event args</param>
        protected virtual void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.control.Unloaded -= OnControlUnloaded;

            if (!UnregisterRegion(this))
                Logger.Log($"Failed to unregister the region \"{RegionName}\", control name:\"{ControlName}\"", Category.Exception, Priority.High);
        }

        #region Selectables

        /// <summary>
        /// Tries to register view or context that implements <see cref="ISelectable"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="viewOrObject">The view or object</param>
        /// <param name="context">The context</param>
        protected void TryAddSelectable(Type sourceType, object viewOrObject, object context)
        {
            if (context is ISelectable)
            {
                if (!selectables.ContainsKey(sourceType))
                    selectables[sourceType] = new List<SelectableRegistration>();

                selectables[sourceType].Add(new SelectableRegistration(true, sourceType, viewOrObject, context));
            }
            else if (viewOrObject is ISelectable)
            {
                if (!selectables.ContainsKey(sourceType))
                    selectables[sourceType] = new List<SelectableRegistration>();

                selectables[sourceType].Add(new SelectableRegistration(false, sourceType, viewOrObject, context));
            }
        }

        /// <summary>
        /// Tries to get the selectable.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The selectable found or null</returns>
        protected SelectableRegistration TryGetSelectable(Type sourceType, object parameter)
        {
            if (selectables.TryGetValue(sourceType, out List<SelectableRegistration> registrations))
            {
                foreach (var registration in registrations)
                {
                    if (registration.IsView)
                    {
                        if (((ISelectable)registration.Context).IsTarget(sourceType, parameter))
                            return registration;
                    }
                    else
                    {
                        if (((ISelectable)registration.ViewOrObject).IsTarget(sourceType, parameter))
                            return registration;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Removes the selectables from view or object manager.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        /// <returns>True if selectable removed</returns>
        protected bool RemoveSelectable(NavigationEntry entry)
        {
            if (entry.Context is ISelectable || entry.Source is ISelectable)
            {
                var selectable = TryGetSelectable(entry.SourceType, entry.Parameter);
                if (selectable != null)
                {
                    var removed = selectables[entry.SourceType].Remove(selectable);
                    return removed;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the selectables from view or object manager.
        /// </summary>
        /// <param name="entries">The navigation entries</param>
        protected void RemoveSelectables(IEnumerable<NavigationEntry> entries)
        {
            foreach (var entry in entries)
                RemoveSelectable(entry);
        }

        #endregion // Selectables

        #region Deactivatable management


        /// <summary>
        /// Check Can Deactivate for child regions.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateChildRegionsOrThrowAsync(IList<RegionBase> childRegions)
        {
            // sub child => child => parent
            foreach (var childRegion in childRegions)
            {
                var currentEntry = childRegion.CurrentEntry;
                if (currentEntry != null)
                {
                    // child 
                    if (currentEntry.ChildRegions.Count > 0)
                        await childRegion.CheckCanDeactivateChildRegionsOrThrowAsync(currentEntry.ChildRegions);

                    // current
                    await childRegion.CheckCanDeactivateEntryOrThrowAsync(currentEntry);
                }
            }
        }


        /// <summary>
        /// Check Can Deactivate for current entry.
        /// </summary>
        /// <param name="entry">The navigation entry.</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateEntryOrThrowAsync(NavigationEntry entry)
        {
            var viewOrObject = entry.Source;
            if (viewOrObject != null && viewOrObject is ICanDeactivate viewOrObjectAsICanDeactivate)
                if (!await viewOrObjectAsICanDeactivate.CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);

            var context = entry.Context;
            if (context != null && context is ICanDeactivate contextAsICanDeactivate)
                if (!await contextAsICanDeactivate.CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }

        /// <summary>
        /// Check Can Deactivate for child regions and current entry.
        /// </summary>
        /// <param name="entry">The navigation entry.</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateOrThrowAsync(NavigationEntry entry)
        {
            // sub child => child => parent
            if (entry.ChildRegions.Count > 0)
                await CheckCanDeactivateChildRegionsOrThrowAsync(entry.ChildRegions);

            await CheckCanDeactivateEntryOrThrowAsync(entry);
        }

        #endregion // Deactivatable management

        #region ICanActivate management


        /// <summary>
        /// Checks can deactivate for view or object and context that implements <see cref="ICanDeactivate" /> or throws an exception.
        /// </summary>
        /// <param name="viewOrObject">The view or object</param>
        /// <param name="context">The data context</param>
        /// <param name="parameter">the parameter</param>
        /// <returns></returns>
        protected async Task CheckCanActivateOrThrowAsync(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is ICanActivate viewOrObjectAsICanActivate)
                if (!await viewOrObjectAsICanActivate.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);

            if (context != null && context is ICanActivate contextAsICanActivate)
                if (!await contextAsICanActivate.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }

        /// <summary>
        /// Checks can activate for view or object and context that implements <see cref="ICanActivate" /> or throws an exception.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        /// <returns></returns>
        protected async Task CheckCanActivateEntryOrThrowAsync(NavigationEntry entry)
        {
            // parent => child => sub child
            var viewOrObject = entry.Source;
            var parameter = entry.Parameter;
            if (viewOrObject != null && viewOrObject is ICanActivate viewOrObjectAsICanActivate)
                if (!await viewOrObjectAsICanActivate.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);

            var context = entry.Context;
            if (context != null && context is ICanActivate contextAsICanActivate)
                if (!await contextAsICanActivate.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }

        /// <summary>
        /// Checks can activate for view or object and context that implements <see cref="ICanActivate" /> or throws an exception.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        /// <returns></returns>
        protected async Task CheckCanActivateOrThrowAsync(NavigationEntry entry)
        {
            // parent => child => sub child
            await CheckCanActivateEntryOrThrowAsync(entry);

            if (entry.ChildRegions.Count > 0)
                await CheckCanActivateChildRegionsOrThrowAsync(entry.childRegions);

        }


        /// <summary>
        /// Checks can activate for view or object and context that implements <see cref="ICanActivate" /> or throws an exception.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        /// <returns></returns>
        protected async Task CheckCanActivateChildRegionsOrThrowAsync(IList<RegionBase> childRegions)
        {
            // parent => child => sub child
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    // current child
                    var viewOrObject = entry.Source;
                    await childRegion.CheckCanActivateOrThrowAsync(viewOrObject, entry.Context, entry.Parameter);


                    // sub child regions
                    if (entry.ChildRegions.Count > 0)
                        await childRegion.CheckCanActivateChildRegionsOrThrowAsync(entry.ChildRegions);
                }
            }
        }

        #endregion // ICanActivate management

        #region INavigatable management

        /// <summary>
        /// Invokes OnNavigatingFrom for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatingFrom(NavigationEntry entry)
        {
            // subs child => child => parent
            if (entry.ChildRegions.Count > 0)
                OnNavigatingFromChildRegions(entry.ChildRegions);

            var viewOrObject = entry.Source;
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingFrom();

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingFrom();
        }


        /// <summary>
        /// Invokes OnNavigatingFrom for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        protected void OnNavigatingFromChildRegions(IList<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    // sub child regions
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatingFromChildRegions(entry.ChildRegions);

                    // current child region
                    childRegion.OnNavigatingFrom(entry);
                }
            }
        }

        /// <summary>
        /// Invokes OnNavigatingTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="viewOrObject"></param>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        protected void OnNavigatingTo(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingTo(parameter);

            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingTo(parameter);
        }

        /// <summary>
        /// Invokes OnNavigatingTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatingTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var viewOrObject = entry.Source;
            var parameter = entry.Parameter;

            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingTo(parameter);

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingTo(parameter);

            if (entry.ChildRegions.Count > 0)
                OnNavigatingToChildRegions(entry.ChildRegions);
        }

        /// <summary>
        /// Invokes OnNavigatingTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        protected void OnNavigatingToChildRegions(IList<RegionBase> childRegions)
        {
            //  parent => child => sub child
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    childRegion.OnNavigatingTo(entry);

                    // sub child
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatingToChildRegions(entry.ChildRegions);
                }
            }
        }

        /// <summary>
        /// Invokes OnNavigatedTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="viewOrObject">The view or object</param>
        /// <param name="context">The data context</param>
        /// <param name="parameter">The parameter</param>
        protected void OnNavigatedTo(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatedTo(parameter);

            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatedTo(parameter);
        }

        /// <summary>
        /// Invokes OnNavigatedTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatedTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var viewOrObject = entry.Source;
            var parameter = entry.Parameter;
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatedTo(parameter);

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatedTo(parameter);

            if (entry.ChildRegions.Count > 0)
                OnNavigatedToChildRegions(entry.ChildRegions);
        }

        /// <summary>
        /// Invokes OnNavigatedTo for view or object and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        protected void OnNavigatedToChildRegions(List<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    childRegion.OnNavigatedTo(entry);

                    // sub child
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatedToChildRegions(entry.ChildRegions);
                }
            }
        }

        #endregion INavigatable management


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
        /// Gets or set the context.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="ViewOrObject">The view or object</param>
        /// <returns>The context or null</returns>
        protected object GetOrSetContext(Type sourceType, object ViewOrObject)
        {
            object context = null;
            var view = ViewOrObject as FrameworkElement;
            if (view != null)
            {
                if (view.DataContext != null)
                    context = view.DataContext;
                else
                {
                    context = ResolveContextWithViewModelLocator(sourceType);
                    if (context != null)
                        view.DataContext = context;
                }
            }
            //else
            //{
            //    // view or object ?
            //    context = viewOrObject;
            //}
            return context;
        }

        /// <summary>
        /// Resolves the view model model for the view type.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <returns>The view model</returns>
        protected object ResolveContextWithViewModelLocator(Type viewType)
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType); // singleton or new instance
            if (viewModelType != null)
            {
                var context = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                return context;
            }
            return null;
        }

        /// <summary>
        /// Unregisters the region.
        /// </summary>
        /// <param name="region">The items region</param>
        /// <returns>True if unregistered</returns>
        protected bool UnregisterRegion(IRegion region)
        {
            if (region is ContentRegion contentRegion)
                return regionsRegistry.UnregisterContentRegion(contentRegion);
            else if (region is ItemsRegion itemsRegion)
                return regionsRegistry.UnregisterItemsRegion(itemsRegion);
            else
                throw new NotSupportedException("Invalid region type");
        }

        /// <summary>
        /// Remove non loaded regions.
        /// </summary>
        protected void RemoveNonLoadedRegions()
        {
            regionsRegistry.RemoveNonLoadedRegions();
        }

        /// <summary>
        /// Find child regions for the region.
        /// </summary>
        /// <param name="parent">The control of the region</param>
        /// <returns>A list of regions</returns>
        protected List<RegionBase> FindChildRegions(DependencyObject parent)
        {
            var childRegions = new List<RegionBase>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child.GetValue(RegionManager.ContentRegionNameProperty) != null)
                {
                    var regionName = child.GetValue(RegionManager.ContentRegionNameProperty) as string;

                    var region = regionsRegistry.GetContentRegion(regionName, child);
                    if (region != null)
                        childRegions.Add(region);
                }
                else if (child.GetValue(RegionManager.ItemsRegionNameProperty) != null)
                {
                    var regionName = child.GetValue(RegionManager.ItemsRegionNameProperty) as string;

                    var region = regionsRegistry.GetItemsRegion(regionName, child);
                    if (region != null)
                        childRegions.Add(region);
                }
                childRegions.AddRange(FindChildRegions(child));
            }
            return childRegions;
        }

        /// <summary>
        /// Clear and unergister regions and child regions.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void ClearChildRegions(NavigationEntry entry)
        {
            // sub child => child => parent
            foreach (var child in entry.ChildRegions)
            {
                // sub child
                if (child.CurrentEntry != null && child.CurrentEntry.ChildRegions.Count > 0)
                    child.ClearChildRegions(child.CurrentEntry);

                if (child is ContentRegion)
                {
                    var contentRegion = child as ContentRegion;
                    contentRegion.ClearContent();
                    contentRegion.History.Clear();
                }
                else
                {
                    var itemsRegion = child as ItemsRegion;
                    itemsRegion.ClearItems();
                    itemsRegion.History.Clear();
                }
            }
        }

        /// <summary>
        /// Handle loaded for a view.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="onLoaded">The callback</param>
        protected void HandleViewLoaded(FrameworkElement view, Action<object, RoutedEventArgs> onLoaded)
        {
            // Called only after "control.Content = view"
            var listener = new LoadedEventListener(view);
            listener.Subscribe((s, e) =>
            {
                listener.Unsubscribe();
                listener = null;

                var eventArgs = new ViewLoadedEventArgs(regionName, view, this);
                ViewLoaded?.Invoke(this, eventArgs);

                Logger.Log($"View \"{view.GetType().FullName}\" loaded, region \"{regionName}\"", Category.Info, Priority.Low);

                onLoaded(s, e);
            });
        }


        /// <summary>
        /// Notify view models that implement <see cref="IIsLoaded" />.
        /// </summary>
        /// <param name="viewOrObject">The view or object</param>
        /// <param name="context">The data context</param>
        /// <param name="parameter">The parameter</param>
        protected void NotifyLoadedListeners(object viewOrObject, object context, object parameter)
        {
            try
            {
                if (context != null)
                {
                    if (context is IIsLoaded loadedEventListener)
                        loadedEventListener.OnLoaded(parameter);
                }
                else if (viewOrObject is IIsLoaded loadedEventListener)
                {
                    loadedEventListener.OnLoaded(parameter);
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets the generic type <see cref="IRegionKnowledge{T}"/> for the context type.
        /// </summary>
        /// <param name="contextType">The context type</param>
        /// <returns>The generic type or null</returns>
        protected Type GetGenericRegionKnowledge(Type contextType)
        {
            var interfaces = contextType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRegionKnowledge<>))
                {
                    var regionType = @interface.GetGenericArguments()[0];
                    return regionType;
                }
            }

            return null;
        }

        /// <summary>
        /// Notify view models that implement <see cref="ILoadedEventListener" />.
        /// </summary>
        /// <param name="region">The region</param>
        /// <param name="context">The data context</param>
        protected void NotifyRegionKnowledge(IRegion region, object context)
        {
            // IRegionKnowledge
            if (context is IRegionKnowledge regionKnowledge)
                regionKnowledge.GetRegion(region);

            // IRegionKnowledge<T>
            Type contextType = context.GetType();
            var regionType = GetGenericRegionKnowledge(contextType);
            if (regionType != null)
            {
                var method = contextType.GetMethod("GetRegion", new Type[] { regionType });
                if (method != null)
                {
                    if (region.GetType() != regionType)
                        throw new InvalidOperationException($"Invalid region type. Expected \"{regionType.Name}\", Current \"{region.GetType().Name}\"");

                    method.Invoke(context, new object[] { region });
                }
            }
        }

        /// <summary>
        /// Notifies <see cref="Navigating"/> subscribers.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter type</param>
        /// <param name="regionNavigationType">The navigation type</param>
        protected void OnNavigating(Type sourceType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(sourceType, parameter, regionNavigationType);
            foreach (var handler in this.navigating)
                handler(this, context);
        }

        /// <summary>
        /// Notifies <see cref="Navigated"/> subscribers.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter type</param>
        /// <param name="regionNavigationType">The navigation type</param>
        protected void OnNavigated(Type sourceType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(sourceType, parameter, regionNavigationType);
            foreach (var handler in this.navigated)
                handler(this, context);
        }

        /// <summary>
        ///  Notifies <see cref="NavigationFailed"/> subscribers.
        /// </summary>
        /// <param name="exception">The exception</param>
        protected void OnNavigationFailed(NavigationFailedException exception)
        {
            RemoveNonLoadedRegions();

            var context = new RegionNavigationFailedEventArgs(exception);
            foreach (var handler in this.navigationFailed)
                handler(this, context);
        }
    }
}
