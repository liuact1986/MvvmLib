using CompositeCommandSample.Common;
using CompositeCommandSample.Views;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace CompositeCommandSample.ViewModels
{
    public class ShellViewModel : ILoadedEventListener
    {
        public CompositeCommand SaveAllCommand { get; }

        private IRegionManager regionManager;

        public ShellViewModel(IApplicationCommands applicationCommands, IRegionManager regionManager)
        {
            SaveAllCommand = applicationCommands.SaveAllCommand;
            this.regionManager = regionManager;
        }

        public async void OnLoaded(object parameter)
        {
            await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabA");
            await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabB");
            await regionManager.GetItemsRegion("TabRegion").AddAsync(typeof(TabView), "TabC");
        }
    }

}
