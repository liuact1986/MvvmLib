using CompositeCommandSample.Common;
using CompositeCommandSample.Views;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Windows;

namespace CompositeCommandSample.ViewModels
{
    public class ShellViewModel : ILoadedEventListener
    {
        public CompositeCommand SaveAllCommand { get; }

        private IRegionNavigationService regionNavigationService;

        public ShellViewModel(IApplicationCommands applicationCommands, IRegionNavigationService regionNavigationService)
        {
            SaveAllCommand = applicationCommands.SaveAllCommand;
            this.regionNavigationService = regionNavigationService;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            await regionNavigationService.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabA");
            await regionNavigationService.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabB");
            await regionNavigationService.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabC");
        }
    }

}
