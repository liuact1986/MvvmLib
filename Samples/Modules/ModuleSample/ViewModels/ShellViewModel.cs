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

            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        private void Navigate(string sourceName)
        {
            LoadModuleForSourceName(sourceName);

            this.Navigation.Navigate(sourceName, null);
        }

        private void LoadModuleForSourceName(string sourceName)
        {
            switch (sourceName)
            {
                case "ViewA":
                case "ViewB":
                    LoadModule("ModuleA");
                    break;
                case "ViewC":
                    LoadModule("ModuleB");
                    break;
                default:
                    break;
            }
        }

        private void LoadModule(string moduleName)
        {
            if (!ModuleManager.IsModuleLoaded(moduleName))
                ModuleManager.LoadModule(moduleName);
        }
    }
}
