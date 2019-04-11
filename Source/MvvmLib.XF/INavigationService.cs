using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public interface INavigationService
    {
        Dictionary<Type, Page> ActivePages { get; }
        bool CanPop { get; }
        bool CanPopModal { get; }
        Page CurrentModalPage { get; }
        Page CurrentPage { get; }
        IReadOnlyList<Page> ModalStack { get; }
        IReadOnlyList<Page> NavigationStack { get; }
        Page PreviousModalPage { get; }
        Page PreviousPage { get; }
        Page RootPage { get; }
        bool StoreActivePages { get; set; }

        event EventHandler<PageNavigatedEventArgs> Navigated;
        event EventHandler<PageNavigatingEventArgs> Navigating;
        event EventHandler<PageNavigationFailedEventArgs> NavigationFailed;

        void HandleSystemPagePopped();
        Task PopAsync();
        Task PopAsync(bool animated);
        Task PopAsync(object parameter);
        Task PopAsync(object parameter, bool animated);
        Task PopModalAsync();
        Task PopModalAsync(bool animated);
        Task PopModalAsync(object parameter);
        Task PopModalAsync(object parameter, bool animated);
        Task PopToRootAsync();
        Task PopToRootAsync(bool animated);
        Task PopToRootAsync(object parameter);
        Task PopToRootAsync(object parameter, bool animated);
        Task PushAsync(Type pageType);
        Task PushAsync(Type pageType, bool animated);
        Task PushAsync(Type pageType, object parameter);
        Task PushAsync(Type pageType, object parameter, bool animated);
        Task PushModalAsync(Type pageType);
        Task PushModalAsync(Type pageType, bool animated);
        Task PushModalAsync(Type pageType, object parameter);
        Task PushModalAsync(Type pageType, object parameter, bool animated);
        void RemovePage(Page page);
        void UnhandleSystemPagePopped();
    }
}