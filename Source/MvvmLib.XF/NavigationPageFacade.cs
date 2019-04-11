using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{

    public class NavigationPageFacade : INavigationStrategy
    {
        private NavigationPage page;

        public IReadOnlyList<Page> NavigationStack => this.page.Navigation.NavigationStack;
        public IReadOnlyList<Page> ModalStack => this.page.Navigation.ModalStack;

        public event EventHandler<NavigationEventArgs> Popped;

        public NavigationPageFacade(NavigationPage page)
        {
            this.page = page;
            this.page.Popped += OnPagePopped;
        }

        private void OnPagePopped(object sender, NavigationEventArgs e)
        {
            this.Popped?.Invoke(this, e);
        }

        public async Task PopToRootAsync(bool animated)
        {
            await this.page.Navigation.PopToRootAsync(animated);
        }

        public async Task PopToRootAsync()
        {
            await this.page.Navigation.PopToRootAsync();
        }

        public async Task PushAsync(Page page)
        {
            await this.page.Navigation.PushAsync(page);
        }

        public async Task PushAsync(Page page, bool animated)
        {
            await this.page.Navigation.PushAsync(page, animated);
        }

        public async Task PushModalAsync(Page page)
        {
            await this.page.Navigation.PushModalAsync(page);
        }

        public async Task PushModalAsync(Page page, bool animated)
        {
            await this.page.Navigation.PushModalAsync(page, animated);
        }

        public async Task PopAsync()
        {
            await this.page.Navigation.PopAsync();
        }

        public async Task PopAsync(bool animated)
        {
            await this.page.Navigation.PopAsync(animated);
        }

        public async Task PopModalAsync()
        {
            await this.page.Navigation.PopModalAsync();
        }

        public async Task PopModalAsync(bool animated)
        {
            await this.page.Navigation.PopModalAsync(animated);
        }

        public void RemovePage(Page page)
        {
            this.page.Navigation.RemovePage(page);
        }
    }

}
