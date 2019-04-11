using MvvmLib.Navigation;
using NavigationSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MvvmLib.IoC;

namespace NavigationSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            var menuItems = new List<MasterMenuItem>
            {
                new MasterMenuItem { PageType=typeof(PageA), Title="PageA", Parameter="PageA From Menu" },
                new MasterMenuItem { PageType=typeof(PageB), Title="PageB" , Parameter="PageB From Menu" }
            };

            ListViewMenu.ItemsSource = menuItems;

            var navigationManager = App.Current.Injector.GetInstance<INavigationManager>();

            //ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem != null)
                {
                    var menuItem = (MasterMenuItem)e.SelectedItem;

                    await navigationManager.GetNamed("MasterDetail").PushAsync(menuItem.PageType, menuItem.Parameter);
                }
            };
        }
    }


    public class MasterMenuItem
    {
        public Type PageType { get; set; }
        public string Title { get; set; }
        public object Parameter { get; set; }
    }

}