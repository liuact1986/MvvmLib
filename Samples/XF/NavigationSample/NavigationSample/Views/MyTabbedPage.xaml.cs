using MvvmLib.Navigation;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavigationSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTabbedPage : TabbedPage
    {
        public MyTabbedPage()
        {
            InitializeComponent();

            //var navigationService = NavigationManager.Register(tabNav, "tabNav");

            //this.Disappearing += (s, e) =>
            //{
            //    NavigationManager.Unregister("tabNav");
            //};
        }
    }
}