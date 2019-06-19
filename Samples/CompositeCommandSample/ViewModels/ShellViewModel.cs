using CompositeCommandSample.Common;
using MvvmLib.Commands;
using MvvmLib.Navigation;

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

        public void Load()
        {
            this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabA"));
            this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabB"));
            this.TabItemsSource.Items.Add(new TabViewModel(applicationCommands, "TabC"));
        }
    }

}
