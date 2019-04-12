using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public class PageNavigationService : INavigationService
    {
        protected INavigationStrategy navigationStrategy;

        protected const bool DefaultAnimated = true;

        protected bool isPopHandled = false;

        protected NavigationGuard guard;

        public Dictionary<Type, Page> ActivePages { get; }

        protected bool storeActivePages = true;
        public bool StoreActivePages
        {
            get { return storeActivePages; }
            set
            {
                storeActivePages = value;
                if (!storeActivePages)
                {
                    ActivePages.Clear();
                }
            }
        }

        public Page CurrentPage => this.navigationStrategy.NavigationStack.LastOrDefault();

        public Page CurrentModalPage => this.navigationStrategy.ModalStack.LastOrDefault();

        public Page PreviousPage
        {
            get
            {
                var stack = this.navigationStrategy.NavigationStack;
                return stack.Count > 1 ? stack.ElementAt(stack.Count - 2) : null;
            }
        }

        public Page PreviousModalPage
        {
            get
            {
                var stack = this.navigationStrategy.ModalStack;
                return stack.Count > 1 ? stack.ElementAt(stack.Count - 2) : null;
            }
        }

        public Page RootPage => this.navigationStrategy.NavigationStack.FirstOrDefault();

        public bool CanPop => this.navigationStrategy.NavigationStack.Count > 1;

        public bool CanPopModal => this.navigationStrategy.ModalStack.Count > 0;

        public IReadOnlyList<Page> ModalStack => this.navigationStrategy.ModalStack;

        public IReadOnlyList<Page> NavigationStack => this.navigationStrategy.NavigationStack;

        protected readonly List<EventHandler<PageNavigatingEventArgs>> navigating = new List<EventHandler<PageNavigatingEventArgs>>();
        public event EventHandler<PageNavigatingEventArgs> Navigating
        {
            add { if (!navigating.Contains(value)) navigating.Add(value); }
            remove { if (navigating.Contains(value)) navigating.Remove(value); }
        }

        protected readonly List<EventHandler<PageNavigatedEventArgs>> navigated = new List<EventHandler<PageNavigatedEventArgs>>();
        public event EventHandler<PageNavigatedEventArgs> Navigated
        {
            add { if (!navigated.Contains(value)) navigated.Add(value); }
            remove { if (navigated.Contains(value)) navigated.Remove(value); }
        }

        protected readonly List<EventHandler<PageNavigationFailedEventArgs>> navigationFailed = new List<EventHandler<PageNavigationFailedEventArgs>>();
        public event EventHandler<PageNavigationFailedEventArgs> NavigationFailed
        {
            add { if (!navigationFailed.Contains(value)) navigationFailed.Add(value); }
            remove { if (navigationFailed.Contains(value)) navigationFailed.Remove(value); }
        }

        public PageNavigationService(INavigationStrategy navigationStrategy)
        {
            this.guard = new NavigationGuard();
            this.guard.SetCancellationCallback(OnActivationCancel, OnDeactivationCancel);
            this.ActivePages = new Dictionary<Type, Page>();

            this.navigationStrategy = navigationStrategy;
            this.HandleSystemPagePopped();
        }


        #region OnChange

        public void HandleSystemPagePopped()
        {
            this.navigationStrategy.Popped += OnPagePopped;
        }

        public void UnhandleSystemPagePopped()
        {
            this.navigationStrategy.Popped -= OnPagePopped;
        }

        private void OnPagePopped(object sender, NavigationEventArgs e)
        {
            if (isPopHandled)
            {
                isPopHandled = false;
            }
            else
            {
                // check after changes
                CheckAfterPagePopped(e.Page);
            }
        }

        protected virtual async void CheckAfterPagePopped(Page currentPage)
        {
            bool navigationCancelled = false;
            this.RaiseNavigating(currentPage, PageNavigationType.Pop);
            // current page cannot be null

            // deactivate?
            if (await this.CheckCanDeactivateAsync(currentPage, currentPage.BindingContext))
            {
                var previousPage = this.navigationStrategy.NavigationStack.LastOrDefault();
                var previousPageContext = previousPage.BindingContext;
                object parameter = null;
                if (previousPageContext != null && previousPageContext is INavigationParameterKnowledge p)
                {
                    parameter = p.Parameter;
                }

                // activate?
                if (await this.CheckCanActivateAsync(previousPage, previousPageContext, parameter))
                {

                    // on navigating from
                    this.DoOnNavigatingFrom(currentPage, currentPage.BindingContext);

                    // on navigating to
                    this.DoOnNavigatingTo(previousPage, previousPageContext, parameter);

                    // on navigated to
                    this.DoOnNavigatedTo(previousPage, previousPageContext, parameter);

                    this.RaiseNavigated(previousPage, parameter, PageNavigationType.Pop);
                }
                else
                {
                    navigationCancelled = true;
                }
            }
            else
            {
                navigationCancelled = true;
            }

            if (navigationCancelled)
            {
                await this.navigationStrategy.PushAsync(currentPage);
            }
        }

        #endregion // On Change

        protected virtual async Task<bool> CheckCanDeactivateAsync(object page, object context)
        {
            var canDeactivateView = page is IDeactivatable ? await this.guard.CheckCanDeactivateAsync((IDeactivatable)page) : true;
            if (!canDeactivateView)
            {
                return false;
            }
            var canDeactivateViewModel = context != null && context is IDeactivatable ?
                await this.guard.CheckCanDeactivateAsync((IDeactivatable)context)
                : true;
            return canDeactivateViewModel;
        }

        protected virtual async Task<bool> CheckCanActivateAsync(object page, object context, object parameter)
        {
            var canActivateView = page is IActivatable ?
                await this.guard.CheckCanActivateAsync((IActivatable)page, parameter)
                : true;
            if (!canActivateView)
            {
                return false;
            }

            var canActivateViewModel = context != null && context is IActivatable ?
                await this.guard.CheckCanActivateAsync((IActivatable)context, parameter)
                : true;
            return canActivateViewModel;
        }

        protected void AddOrUpdateActivePage(Type pageType, Page page)
        {
            if (StoreActivePages)
            {
                ActivePages[pageType] = page;
            }
        }

        protected virtual Page CreatePage(Type pageType)
        {
            var page = ViewResolver.Resolve(pageType) as Page;
            return page;
        }

        protected virtual Page GetOrCreatePage(Type pageType)
        {
            if (this.ActivePages.TryGetValue(pageType, out Page page))
            {
                return page;
            }
            else
            {
                page = CreatePage(pageType);
                AddOrUpdateActivePage(pageType, page);
                return page;
            }
        }

        protected virtual void DoOnNavigatingFrom(object page, object context)
        {
            if (page is INavigatable)
            {
                ((INavigatable)page).OnNavigatingFrom();
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatingFrom();
            }
        }

        protected virtual void DoOnNavigatingTo(object page, object context, object parameter)
        {
            if (page is INavigatable)
            {
                ((INavigatable)page).OnNavigatingTo(parameter);
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatingTo(parameter);
            }
        }

        protected virtual void DoOnNavigatedTo(object page, object context, object parameter)
        {
            if (page is INavigatable)
            {
                ((INavigatable)page).OnNavigatedTo(parameter);
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatedTo(parameter);
            }
        }

        protected virtual object GetOrSetContext(Type pageType, Page page)
        {
            if (page.BindingContext != null)
            {
                return page.BindingContext;
            }
            else
            {
                var context = ViewModelLocationProvider.ResolveViewModelType(pageType); // singleton or new instance
                if (context != null)
                {
                    page.BindingContext = context;
                }
                return context;
            }
        }

        public void RemovePage(Page page)
        {
            this.navigationStrategy.RemovePage(page);
        }

        #region On Demand

        protected virtual async Task ProcessPushAsync(Type pageType, object parameter, bool animated, bool modal)
        {
            var currentPage = modal ? CurrentModalPage : CurrentPage;
            if (currentPage != null)
            {
                this.RaiseNavigating(currentPage, PageNavigationType.Push);
            }
            // deactivate?
            if (currentPage == null || await this.CheckCanDeactivateAsync(currentPage, currentPage.BindingContext))
            {
                var page = this.GetOrCreatePage(pageType);
                if (page == null) { throw new ArgumentException($"Page cannot be null \"{pageType.Name}\""); }

                object context = this.GetOrSetContext(pageType, page);
                if (context != null)
                {
                    if (context is INavigationParameterKnowledge)
                    {
                        ((INavigationParameterKnowledge)context).Parameter = parameter;
                    }
                    if (context is IPageKnowledge)
                    {
                        ((IPageKnowledge)context).GetPage(page);
                    }
                }

                // activate?
                if (await this.CheckCanActivateAsync(page, context, parameter))
                {
                    // on navigating from
                    if (currentPage != null)
                    {
                        this.DoOnNavigatingFrom(currentPage, currentPage.BindingContext);
                    }

                    // on navigating to
                    this.DoOnNavigatingTo(page, context, parameter);

                    if (modal)
                    {
                        await this.navigationStrategy.PushModalAsync(page, animated);
                    }
                    else
                    {
                        await this.navigationStrategy.PushAsync(page, animated);
                    }

                    // on navigated to
                    this.DoOnNavigatedTo(page, context, parameter);

                    AddOrUpdateActivePage(pageType, page);

                    var navigationType = modal ? PageNavigationType.PushModal : PageNavigationType.Push;
                    this.RaiseNavigated(page, parameter, navigationType);
                }
            }

        }

        public async Task PushAsync(Type pageType, object parameter, bool animated)
        {
            await this.ProcessPushAsync(pageType, parameter, animated, false);
        }

        public async Task PushAsync(Type pageType, object parameter)
        {
            await this.ProcessPushAsync(pageType, parameter, DefaultAnimated, false);
        }

        public async Task PushAsync(Type pageType)
        {
            await this.ProcessPushAsync(pageType, null, DefaultAnimated, false);
        }

        public async Task PushAsync(Type pageType, bool animated)
        {
            await this.ProcessPushAsync(pageType, null, animated, false);
        }

        public async Task PushModalAsync(Type pageType, object parameter, bool animated)
        {
            await this.ProcessPushAsync(pageType, parameter, animated, true);
        }

        public async Task PushModalAsync(Type pageType, bool animated)
        {
            await this.ProcessPushAsync(pageType, null, animated, true);
        }

        public async Task PushModalAsync(Type pageType, object parameter)
        {
            await this.ProcessPushAsync(pageType, parameter, DefaultAnimated, true);
        }

        public async Task PushModalAsync(Type pageType)
        {
            await this.ProcessPushAsync(pageType, null, DefaultAnimated, true);
        }

        protected virtual async Task ProcessPopAsync(object parameter, bool animated)
        {
            if (CanPop)
            {
                var currentPage = CurrentPage;
                this.RaiseNavigating(currentPage, PageNavigationType.Pop);

                // current page cannot be null

                // deactivate?
                if (await this.CheckCanDeactivateAsync(currentPage, currentPage.BindingContext))
                {
                    var previousPage = PreviousPage;

                    // activate?
                    if (await this.CheckCanActivateAsync(previousPage, previousPage.BindingContext, parameter))
                    {
                        this.isPopHandled = true;

                        // on navigating from
                        this.DoOnNavigatingFrom(currentPage, currentPage.BindingContext);

                        // on navigating to
                        this.DoOnNavigatingTo(previousPage, previousPage.BindingContext, parameter);

                        await this.navigationStrategy.PopAsync(animated);

                        // on navigated to
                        this.DoOnNavigatedTo(previousPage, previousPage.BindingContext, parameter);

                        this.RaiseNavigated(previousPage, parameter, PageNavigationType.Pop);
                    }
                }
            }
        }

        protected virtual async Task ProcessPopModalAsync(object parameter, bool animated)
        {
            if (CanPopModal)
            {
                var currentPage = CurrentModalPage;
                this.RaiseNavigating(currentPage, PageNavigationType.PopModal);

                // current page cannot be null

                // deactivate?
                if (await this.CheckCanDeactivateAsync(currentPage, currentPage.BindingContext))
                {
                    var previousPage = PreviousModalPage;

                    // activate?
                    if (previousPage == null || await this.CheckCanActivateAsync(previousPage, previousPage.BindingContext, parameter))
                    {
                        this.isPopHandled = true;

                        // on navigating from
                        this.DoOnNavigatingFrom(currentPage, currentPage.BindingContext);

                        // on navigating to
                        if (previousPage != null)
                        {
                            this.DoOnNavigatingTo(previousPage, previousPage.BindingContext, parameter);
                        }

                        await this.navigationStrategy.PopModalAsync(animated);

                        // on navigated to
                        if (previousPage != null)
                        {
                            this.DoOnNavigatedTo(previousPage, previousPage.BindingContext, parameter);
                        }

                        this.RaiseNavigated(previousPage, parameter, PageNavigationType.PopModal);
                    }
                }
            }
        }

        public async Task PopAsync(object parameter, bool animated)
        {
            await this.ProcessPopAsync(parameter, animated);
        }

        public async Task PopAsync(object parameter)
        {
            await this.ProcessPopAsync(parameter, DefaultAnimated);
        }

        public async Task PopAsync(bool animated)
        {
            await this.ProcessPopAsync(null, animated);
        }

        public async Task PopAsync()
        {
            await this.ProcessPopAsync(null, DefaultAnimated);
        }

        public async Task PopModalAsync(object parameter, bool animated)
        {
            await this.ProcessPopModalAsync(parameter, animated);
        }

        public async Task PopModalAsync(object parameter)
        {
            await this.ProcessPopModalAsync(parameter, DefaultAnimated);
        }

        public async Task PopModalAsync(bool animated)
        {
            await this.ProcessPopModalAsync(null, animated);
        }

        public async Task PopModalAsync()
        {
            await this.ProcessPopModalAsync(null, DefaultAnimated);
        }

        private async Task ProcessPopToRootAsync(object parameter, bool animated)
        {
            if (CanPop)
            {
                var currentPage = CurrentPage;
                this.RaiseNavigating(currentPage, PageNavigationType.PopToRoot);
                // current page cannot be null

                // deactivate?
                if (await this.CheckCanDeactivateAsync(currentPage, currentPage.BindingContext))
                {
                    var rootPage = RootPage;

                    // activate?
                    if (await this.CheckCanActivateAsync(rootPage, rootPage.BindingContext, parameter))
                    {
                        // on navigating from
                        this.DoOnNavigatingFrom(currentPage, currentPage.BindingContext);

                        // on navigating to
                        this.DoOnNavigatingTo(rootPage, rootPage.BindingContext, parameter);

                        await this.navigationStrategy.PopToRootAsync(animated);

                        // on navigated to
                        this.DoOnNavigatedTo(rootPage, rootPage.BindingContext, parameter);

                        this.RaiseNavigated(rootPage, parameter, PageNavigationType.PopToRoot);
                    }
                }
            }
        }

        public async Task PopToRootAsync(object parameter, bool animated)
        {
            await this.ProcessPopToRootAsync(parameter, animated);
        }

        public async Task PopToRootAsync(object parameter)
        {
            await this.ProcessPopToRootAsync(parameter, DefaultAnimated);
        }

        public async Task PopToRootAsync(bool animated)
        {
            await this.ProcessPopToRootAsync(null, animated);
        }

        public async Task PopToRootAsync()
        {
            await this.ProcessPopToRootAsync(null, DefaultAnimated);
        }

        #endregion // On Demand

        protected virtual void OnDeactivationCancel(IDeactivatable source)
        {
            RaiseNavigationCancelled(source);
        }

        protected virtual void OnActivationCancel(IActivatable source, object parameter)
        {
            RaiseNavigationCancelled(source, parameter);
        }

        protected void RaiseNavigating(Page page, PageNavigationType navigationType)
        {
            var context = new PageNavigatingEventArgs(page, navigationType);
            foreach (var handler in this.navigating)
            {
                handler(this, context);
            }
        }

        protected void RaiseNavigated(Page page, object parameter, PageNavigationType navigationType)
        {
            var context = new PageNavigatedEventArgs(page, parameter, navigationType);
            foreach (var handler in this.navigated)
            {
                handler(this, context);
            }
        }

        protected void RaiseNavigationCancelled(object source, object parameter = null)
        {
            var context = new PageNavigationFailedEventArgs(source, parameter);
            foreach (var handler in this.navigationFailed)
            {
                handler(this, context);
            }
        }
    }
}
