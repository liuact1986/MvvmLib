using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class MasterDetailViewModel : ILoadedEventListener, IRegionKnowledge<ContentRegion>
    {
        private IRegionManager regionManager;

        public ICommand NavigateCommand { get; }

        public MasterDetailViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            await regionManager.GetContentRegion("Master").NavigateAsync(typeof(PeopleView));
        }

        public void GetRegion(ContentRegion region)
        {
            //region.Animation.DefaultAnimationDuration = 1000;
        }
    }
}
