using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private ItemsRegion tabControlRegion;
        private ItemsRegion tabControlRegion2;

        public IRelayCommand CloseTabItemCommand { get; }

        public IRelayCommand CloseTabItemCommand2 { get; }

        public ViewModelBase(IRegionNavigationService regionNavigationService)
        {
            if (regionNavigationService.IsItemsRegionDiscovered("TabControlRegion"))
            {
                this.tabControlRegion = regionNavigationService.GetItemsRegion("TabControlRegion");
                this.tabControlRegion2 = regionNavigationService.GetItemsRegion("TabControlRegion2");

                CloseTabItemCommand = new RelayCommand<object>(async (item) =>
                {
                    //var index = tabControlRegion.FindIndex(item);
                    //if (index != -1)
                    //    await tabControlRegion.RemoveAtAsync(index);
                    // or
                    await tabControlRegion.RemoveAsync(item);
                });


                CloseTabItemCommand2 = new RelayCommand<object>(async (item) =>
                {
                    await tabControlRegion2.RemoveAsync(item);
                });

            }
        }
    }
}
