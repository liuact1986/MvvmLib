using MvvmLib.Commands;
using MvvmLib.Modules;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Windows.Input;

namespace ModuleSample.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public NavigationSource Navigation { get; }
        public ICommand NavigateCommand { get; set; }

        public ShellViewModel()
        {
            this.Navigation = new NavigationSource();

            ModuleManager.LoadModule("ModuleA");

            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        private async void Navigate(string sourceName)
        {
            await this.Navigation.NavigateAsync(sourceName, null);
        }
    }
}
