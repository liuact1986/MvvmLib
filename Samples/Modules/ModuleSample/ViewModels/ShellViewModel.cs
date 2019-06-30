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

            // if (!ModuleManager.Modules["ModuleA"].IsLoaded)
            ModuleManager.LoadModule("ModuleA");

            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        private void Navigate(string sourceName)
        {
            this.Navigation.Navigate(sourceName, null);
        }
    }
}
