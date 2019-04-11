using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public interface INavigationStrategy
    {
        IReadOnlyList<Page> ModalStack { get; }
        IReadOnlyList<Page> NavigationStack { get; }

        event EventHandler<NavigationEventArgs> Popped;

        Task PopAsync();
        Task PopAsync(bool animated);
        Task PopModalAsync();
        Task PopModalAsync(bool animated);
        Task PopToRootAsync();
        Task PopToRootAsync(bool animated);
        Task PushAsync(Page page);
        Task PushAsync(Page page, bool animated);
        Task PushModalAsync(Page page);
        Task PushModalAsync(Page page, bool animated);
        void RemovePage(Page page);
    }
}