using CompositeCommandSample.Common;
using CompositeCommandSample.Views;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Collections.Generic;

namespace CompositeCommandSample.ViewModels
{
    public class ShellViewModel
    {
        private readonly IApplicationCommands applicationCommands;

        public CompositeCommand SaveAllCommand { get; }
        public SharedSource<TabViewModel> TabItemsSource { get; }

        public ShellViewModel(IApplicationCommands applicationCommands)
        {
            this.applicationCommands = applicationCommands;
            SaveAllCommand = applicationCommands.SaveAllCommand;

            this.TabItemsSource = NavigationManager.GetOrCreateSharedSource<TabViewModel>();
            this.Load();
        }

        public async void Load()
        {
            await this.TabItemsSource.Items.AddAsync(new TabViewModel(applicationCommands, "TabA"));
            await this.TabItemsSource.Items.AddAsync(new TabViewModel(applicationCommands, "TabB"));
            await this.TabItemsSource.Items.AddAsync(new TabViewModel(applicationCommands, "TabC"));
        }
    }

}
