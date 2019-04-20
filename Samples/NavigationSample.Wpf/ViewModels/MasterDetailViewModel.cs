using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class MasterDetailViewModel : ILoadedEventListener, IRegionKnowledge<ContentRegion>
    {
        private IRegionNavigationService regionNavigationService;

        public ICommand NavigateCommand { get; }

        public MasterDetailViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
        }

        public async void OnLoaded(FrameworkElement view, object parameter)
        {
            await regionNavigationService.GetContentRegion("Master").NavigateAsync(typeof(PeopleView));
        }

        public void GetRegion(ContentRegion region)
        {
            //region.Animation.DefaultAnimationDuration = 1000;
        }
    }
}
