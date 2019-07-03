using MvvmLib.Commands;
using MvvmLib.Modules;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModuleSample.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly IModuleManager moduleManager;

        public NavigationSource Navigation { get; }
        public ICommand NavigateCommand { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public ShellViewModel(IModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;

            this.Navigation = new NavigationSource();

            NavigateCommand = new RelayCommand<string>(NavigateToModule);

            moduleManager.ModuleLoaded += OnModuleLoaded;

            ////ModuleManager.Default.ModuleLoaded += OnModuleLoaded;
        }

        private void OnModuleLoaded(object sender, ModuleLoadedEventArgs e)
        {

        }

        //private void NavigateToModule(string sourceName)
        //{
        //    var moduleName = GetModuleName(sourceName);
        //    if (moduleName == null)
        //        return;

        //    LoadModule(moduleName);

        //    this.Navigation.Navigate(sourceName, null);
        //}

        private async void NavigateToModule(string sourceName)
        {
            var moduleName = GetModuleName(sourceName);
            if (moduleName == null)
                return;

            IsBusy = true;
            await Task.Run(() =>
            {
                LoadModule(moduleName);

                ////Thread.Sleep(2000);
            }).ContinueWith(r =>
            {
                this.Navigation.Navigate(sourceName, null);
                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string GetModuleName(string sourceName)
        {
            switch (sourceName)
            {
                case "ViewA":
                case "ViewB":
                    return "ModuleA";
                case "ViewC":
                    return "ModuleB";
            }
            return null;
        }

        private void LoadModule(string moduleName)
        {
            if (!moduleManager.IsModuleLoaded(moduleName))
                moduleManager.LoadModule(moduleName);
        }
    }
}
