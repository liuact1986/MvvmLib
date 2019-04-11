using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public class DialogService : IDialogService
    {
        private Page GetCurrentPage()
        {
            Page page = null;
            if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
            {
                page = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault();
            }
            else 
            {
                page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            }

            if (page == null)
            {
                page = Application.Current.MainPage;
            }
            return page;
        }

        public async Task DisplayAlertAsync(string title, string message, string cancel)
        {
            var page = GetCurrentPage();
            await page.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            var page = GetCurrentPage();
            return await page.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<string> DisplayActionSheetAsync(string title, string message, string destruction, params string[] buttons)
        {
            var page = GetCurrentPage();
            return await page.DisplayActionSheet(title, message, destruction, buttons);
        }
    }
}
