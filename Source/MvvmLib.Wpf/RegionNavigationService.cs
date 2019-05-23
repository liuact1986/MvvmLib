﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Allows to navigate with content and items regions.
    /// </summary>
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly RegionsRegistry regionsRegistry;
        private readonly Dictionary<string, List<PendingContentNavigation>> pendingContentNavigations
            = new Dictionary<string, List<PendingContentNavigation>>();

        /// <summary>
        /// Creates the regions navigation service.
        /// </summary>
        public RegionNavigationService()
            : this(RegionsRegistry.Instance)
        { }

        /// <summary>
        /// Creates the regions navigation service.
        /// </summary>
        /// <param name="regionsRegistry">The regions registry used by the navigation service</param>
        public RegionNavigationService(RegionsRegistry regionsRegistry)
        {
            this.regionsRegistry = regionsRegistry;
            this.regionsRegistry.RegionRegistered += OnRegionRegistered;
        }



        /// <summary>
        /// Gets the last content region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName)
        {
            var region = regionsRegistry.GetContentRegion(regionName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");

            return region;
        }

        /// <summary>
        /// Gets the content region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>The content region</returns>
        public ContentRegion GetContentRegion(string regionName, string controlName)
        {
            var region = regionsRegistry.GetContentRegion(regionName, controlName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\" and control name \"{controlName}\"");

            return region;
        }

        /// <summary>
        /// Gets the content regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of content regions</returns>
        public IReadOnlyList<ContentRegion> GetContentRegions(string regionName)
        {
            if (regionsRegistry.ContentRegions.TryGetValue(regionName, out List<ContentRegion> regions))
            {
                var readOnlyRegions = regions.AsReadOnly();
                return readOnlyRegions;
            }
            throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");
        }

        /// <summary>
        /// Get the last items region for the region name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A list of items regions</returns>
        public ItemsRegion GetItemsRegion(string regionName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");

            return region;
        }

        /// <summary>
        /// Gets the items regions.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>A readonly list of items regions</returns>
        public IReadOnlyList<ItemsRegion> GetItemsRegions(string regionName)
        {
            if (regionsRegistry.ItemsRegions.TryGetValue(regionName, out List<ItemsRegion> regions))
            {
                var readOnlyRegions = regions.AsReadOnly();
                return readOnlyRegions;
            }
            throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\"");
        }

        /// <summary>
        /// Gets the items region for the region name and the control name.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>The items region</returns>
        public ItemsRegion GetItemsRegion(string regionName, string controlName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName, controlName);
            if (region == null)
                throw new RegionResolutionFailedException($"No region found for the region name \"{regionName}\" and control name \"{controlName}\"");

            return region;
        }

        /// <summary>
        /// Checks if the content region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>True if discovered</returns>
        public bool IsContentRegionDiscovered(string regionName)
        {
            var region = regionsRegistry.GetContentRegion(regionName);
            return region != null;
        }

        /// <summary>
        /// Checks if the content region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>True if discovered</returns>
        public bool IsContentRegionDiscovered(string regionName, string controlName)
        {
            var region = regionsRegistry.GetContentRegion(regionName, controlName);
            return region != null;
        }

        /// <summary>
        /// Checks if the items region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <returns>True if discovered</returns>
        public bool IsItemsRegionDiscovered(string regionName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName);
            return region != null;
        }

        /// <summary>
        /// Checks if the items region is discovered.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <returns>True if discovered</returns>
        public bool IsItemsRegionDiscovered(string regionName, string controlName)
        {
            var region = regionsRegistry.GetItemsRegion(regionName, controlName);
            return region != null;
        }

        private void OnRegionRegistered(object sender, RegionRegisteredEventArgs e)
        {
            TryExecutePending(e);
        }

        private async void TryExecutePending(RegionRegisteredEventArgs e)
        {
            if (pendingContentNavigations.Count > 0)
                if (e.Region is ContentRegion contentRegion)
                {
                    string regionName = contentRegion.RegionName;
                    if (pendingContentNavigations.ContainsKey(regionName))
                    {
                        var pending = pendingContentNavigations[regionName].FirstOrDefault(p => p.ControlName == contentRegion.ControlName);
                        if (pending != null)
                        {
                            try
                            {
                                await contentRegion.NavigateAsync(pending.SourceType, pending.Parameter);

                                pendingContentNavigations[regionName].Remove(pending);
                                if (pendingContentNavigations[regionName].Count == 0)
                                    pendingContentNavigations.Remove(regionName);
                            }
                            catch
                            { }
                            finally
                            {
                                pending.OnCompleted?.Invoke();
                            }
                        }
                    }
                }
        }


        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="controlName">The control name</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        public async Task<bool> NavigateWhenAvailableAsync(string contentRegionName, Type sourceType, string controlName, object parameter, Action onCompleted = null)
        {
            if (contentRegionName == null)
                throw new ArgumentNullException(nameof(contentRegionName));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var region = regionsRegistry.GetContentRegion(contentRegionName);
            if (region != null)
            {
                await region.NavigateAsync(sourceType, parameter);
                onCompleted?.Invoke();
                return true;
            }
            else
            {
                var pendingNavigation = new PendingContentNavigation(contentRegionName, controlName, sourceType, parameter, onCompleted);
                if (!pendingContentNavigations.ContainsKey(contentRegionName))
                {
                    pendingContentNavigations[contentRegionName] = new List<PendingContentNavigation>
                    {
                        pendingNavigation
                    };
                }
                else
                {
                    var existing = pendingContentNavigations[contentRegionName].FirstOrDefault(p => p.ControlName == null);
                    if (existing == null)
                        pendingContentNavigations[contentRegionName].Add(pendingNavigation);
                }
                return false;
            }
        }

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">The control name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        public async Task<bool> NavigateWhenAvailable(string regionName, string controlName, Type sourceType, Action onCompleted = null)
        {
            return await this.NavigateWhenAvailableAsync(regionName, sourceType, controlName, null, onCompleted);
        }

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        public async Task<bool> NavigateWhenAvailable(string regionName, Type sourceType, object parameter, Action onCompleted = null)
        {
            return await this.NavigateWhenAvailableAsync(regionName, sourceType, null, parameter, onCompleted);
        }

        /// <summary>
        /// Executes immediately if the region is discovered or wait and execute the navigation later.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="onCompleted">The callback</param>
        /// <returns>True if executed immediately</returns>
        public async Task<bool> NavigateWhenAvailable(string regionName, Type sourceType, Action onCompleted = null)
        {
            return await this.NavigateWhenAvailableAsync(regionName, sourceType, null, null, onCompleted);
        }

        /// <summary>
        /// Navigates to the source for the content region.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(string contentRegionName, Type sourceType, object parameter)
        {
            if (contentRegionName == null)
                throw new ArgumentNullException(nameof(contentRegionName));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var region = GetContentRegion(contentRegionName);
            return await region.NavigateAsync(sourceType, parameter);
        }

        /// <summary>
        /// Navigates to the source for the content region.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(string contentRegionName, Type sourceType)
        {
            return await NavigateAsync(contentRegionName, sourceType, null);
        }

        /// <summary>
        /// Redirects to the source and remove the previous entry from history.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The type of the view or view model to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> RedirectAsync(string contentRegionName, Type sourceType, object parameter)
        {
            if (contentRegionName == null)
                throw new ArgumentNullException(nameof(contentRegionName));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var region = GetContentRegion(contentRegionName);
            return await region.RedirectAsync(sourceType, parameter);
        }

        /// <summary>
        /// Redirect to the vsource and remove the previous entry from history.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <param name="sourceType">The type of the view or view model to redirect</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> RedirectAsync(string contentRegionName, Type sourceType)
        {
            return await RedirectAsync(contentRegionName, sourceType, null);
        }

        /// <summary>
        /// Navigates to the previous entry.
        /// </summary>
        /// <param name="contentRegionName"></param>
        /// <returns></returns>
        public async Task<bool> GoBackAsync(string contentRegionName)
        {
            if (contentRegionName == null)
                throw new ArgumentNullException(nameof(contentRegionName));

            var region = GetContentRegion(contentRegionName);
            return await region.GoBackAsync();
        }

        /// <summary>
        /// Navigates to the next entry.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoForward(string contentRegionName)
        {
            if (contentRegionName == null)
                throw new ArgumentNullException(nameof(contentRegionName));

            var region = GetContentRegion(contentRegionName);
            return await region.GoForwardAsync();
        }

        /// <summary>
        /// Navigates to the root entry.
        /// </summary>
        /// <param name="contentRegionName">The content region name</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateToRootAsync(string contentRegionName)
        {
            var region = GetContentRegion(contentRegionName);
            return await region.NavigateToRootAsync();
        }

        /// <summary>
        /// Inserts a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(string itemsRegionName,int index, Type sourceType, object parameter)
        {
            if (itemsRegionName == null)
                throw new ArgumentNullException(nameof(itemsRegionName));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var region = GetItemsRegion(itemsRegionName);
            return await region.InsertAsync(index, sourceType, parameter);
        }

        /// <summary>
        /// Inserts the source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(string itemsRegionName, int index, Type sourceType)
        {
            return await InsertAsync(itemsRegionName, index, sourceType, null);
        }

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(string itemsRegionName, Type sourceType, object parameter)
        {
            if (itemsRegionName == null)
                throw new ArgumentNullException(nameof(itemsRegionName));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var region = GetItemsRegion(itemsRegionName);
            return await region.AddAsync(sourceType, parameter);
        }

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(string itemsRegionName, Type sourceType)
        {
            return await AddAsync(itemsRegionName, sourceType, null);
        }

        /// <summary>
        /// Removes the entry from the items region history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="index">The index</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAtAsync(string itemsRegionName, int index)
        {
            if (itemsRegionName == null)
                throw new ArgumentNullException(nameof(itemsRegionName));

            var region = GetItemsRegion(itemsRegionName);
            return await region.RemoveAtAsync(index);
        }

        /// <summary>
        /// Tries to find the source and remove the entry from history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <param name="source">The source</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAsync(string itemsRegionName, object source)
        {
            if (itemsRegionName == null)
                throw new ArgumentNullException(nameof(itemsRegionName));

            var region = GetItemsRegion(itemsRegionName);
            return await region.RemoveAsync(source);
        }

        /// <summary>
        /// Clears the region history.
        /// </summary>
        /// <param name="itemsRegionName">The items region name</param>
        /// <returns>True if success</returns>
        public async Task<bool> Clear(string itemsRegionName)
        {
            if (itemsRegionName == null)
                throw new ArgumentNullException(nameof(itemsRegionName));

            var region = GetItemsRegion(itemsRegionName);
            return await region.Clear();
        }

    }


    /// <summary>
    /// Pending navigation.
    /// </summary>
    public class PendingContentNavigation
    {

        private readonly string regionName;
        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }


        private readonly string controlName;

        /// <summary>
        /// The control name.
        /// </summary>
        public string ControlName
        {
            get { return controlName; }
        }

        private readonly Type sourceType;

        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
        }

        private readonly Action onCompleted;

        /// <summary>
        /// Invoked on navigation completed.
        /// </summary>
        public Action OnCompleted
        {
            get { return onCompleted; }
        }

        /// <summary>
        /// Creates the pending navigation class.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="controlName">the control name</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="onCompleted">The callback</param>
        public PendingContentNavigation(string regionName, string controlName, Type sourceType, object parameter, Action onCompleted)
        {
            this.regionName = regionName;
            this.controlName = controlName ?? string.Empty;
            this.sourceType = sourceType;
            this.parameter = parameter;
            this.onCompleted = onCompleted;
        }

    }


}