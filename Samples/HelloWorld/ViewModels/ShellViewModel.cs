using HelloWorld.Views;
using MvvmLib.Navigation;

namespace HelloWorld.ViewModels
{
    public class ShellViewModel
    {
        public NavigationSource Navigation { get; }

        public ShellViewModel()
        {
            // 1. With <ContentControl Content="{Binding Navigation.Current}" />
             this.Navigation = NavigationManager.GetOrCreateDefaultNavigationSource("Main");

            // 2 ... or with <ContentControl mvvmLib:NavigationManager.SourceName="Main"/>
            //this.Navigation = NavigationManager.GetNavigationSources("Main")[0];


            this.NavigateToHome();
        }

        public async void NavigateToHome()
        {
            await Navigation.NavigateAsync(typeof(HomeView), "Hello World!");
        }
    }
}
