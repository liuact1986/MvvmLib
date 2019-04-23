using MvvmLib.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChildRegionsSample.Views
{
    public partial class Shell : Window
    {
        private readonly RegionsRegistry regionsRegistry;
        private readonly RegionNavigationService regionNavigationService;

        public Shell()
        {
            InitializeComponent();

            regionsRegistry = RegionsRegistry.Instance;

            regionNavigationService = new RegionNavigationService();

            var parent = regionNavigationService.GetContentRegion("Zero");
            //parent.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);

            Zero.Content = new Other();

            //regionNavigationService.GetContentRegion("Zero").ViewLoaded += OnViewLoaded;
            //regionsRegistry.RegionRegistered += OnRegionRegistered;
            //regionsRegistry.RegionUnregistered += OnRegionUnregistered;
        }

        private void OnViewLoaded(object sender, ViewLoadedEventArgs e)
        {
            string message = $"{e.RegionName} {e.View.GetType().FullName} LOADED {DateTime.Now.ToLongTimeString()}";
            Debug.WriteLine(message);
            StatusText.Text = message;
        }

        private void OnRegionRegistered(object sender, RegionRegisteredEventArgs e)
        {
            string message = $"{e.RegionName} REGISTERED {DateTime.Now.ToLongTimeString()}";
            Debug.WriteLine(message);
            StatusText.Text = message;
            ShowRegionRegistry();
        }

        private void OnRegionUnregistered(object sender, RegionUnregisteredEventArgs e)
        {
            string message = $"\"{e.RegionName}\" UNREGISTERED {DateTime.Now.ToLongTimeString()}";
            Debug.WriteLine(message);
            StatusText.Text = message;
            ShowRegionRegistry();
        }

        private async void OnShowChild(object sender, RoutedEventArgs e)
        {
            //<Zero.CONTAINER>
            //
            //    <First.VIEW>
            //      <First.CONTAINER>
            //
            //                 <Second.VIEW>
            //                  <Second.CONTAINER>
            //
            //                               <Three.VIEW />
            //
            //                  </ Second.CONTAINER>
            //                 </ Second.VIEW>
            //
            //      </ First.CONTAINER>
            //     </ First.VIEW>
            //
            //</ Zero.CONTAINER>


            var parent = regionNavigationService.GetContentRegion("Zero");
            //parent.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);

            //parent.AnimationCompleted += OnParentAnimationCompleted;

            await parent.NavigateAsync(typeof(First));
        }

        //OpacityAnimation fadeInAnimation = new OpacityAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(6000)) };
        //OpacityAnimation fadeOutAnimation = new OpacityAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(6000)) };

        //TranslateAnimation translateEntranceAnimation = new TranslateAnimation { From = 300, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(1000)) };
        //TranslateAnimation translateExitAnimation = new TranslateAnimation { From = 0, To = 300, Duration = new Duration(TimeSpan.FromMilliseconds(1000)) };

        private async void OnParentAnimationCompleted(object sender, EventArgs e)
        {
            //regionNavigationService.GetContentRegion("Zero").AnimationCompleted -= OnParentAnimationCompleted;

            if (regionNavigationService.IsContentRegionDiscovered("First"))
            {
                var first = regionNavigationService.GetContentRegion("First");
                //first.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);

                //first.AnimationCompleted += OnFirstAnimationCompleted; ;

                await first.NavigateAsync(typeof(Second));
            }
        }

        private async void OnFirstAnimationCompleted(object sender, EventArgs e)
        {
            //regionNavigationService.GetContentRegion("First").AnimationCompleted -= OnFirstAnimationCompleted;

            if (regionNavigationService.IsContentRegionDiscovered("Second"))
            {
                var second = regionNavigationService.GetContentRegion("Second");
                //second.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);
                await second.NavigateAsync(typeof(Three));
            }
        }

        private void ShowRegionRegistry()
        {
            var contentRegions = regionsRegistry.ContentRegions;

            ContentRegionsList.Items.Clear();
            foreach (var contentRegionsOfRegionName in contentRegions.Values)
            {
                for (int i = 0; i < contentRegionsOfRegionName.Count; i++)
                {
                    var contentRegion = contentRegionsOfRegionName[i];
                    ContentRegionsList.Items.Add($"RegionName:{contentRegion.RegionName}, index: {i}");
                }
            }
        }

        private void OnSetEmptyChild(object sender, RoutedEventArgs e)
        {
            // ((ContentControl)regionNavigationService.GetContentRegion("First").Control).Content = null; // => Second .. SubChildRegion unload
            // ((ContentControl)regionNavigationService.GetContentRegion("Zero").Control).Content = null; // => childRegion and  SubChildRegion unload ?
            Zero.Content = null;
        }

        private void OnSampleModeComboxChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            if (combobox.SelectedIndex == 0)
            {
                SampleConfiguration.SampleMode = SampleMode.NoViewModel;
            }
            else if (combobox.SelectedIndex == 1)
            {
                SampleConfiguration.SampleMode = SampleMode.Selectable;
            }
            else if (combobox.SelectedIndex == 2)
            {
                SampleConfiguration.SampleMode = SampleMode.Singleton;
            }
        }

        private async void OnChainNavigation(object sender, RoutedEventArgs e)
        {
            var parent = regionNavigationService.GetContentRegion("Zero");
            //parent.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);

            if (await parent.NavigateAsync(typeof(First)))
            {
                if (regionNavigationService.IsContentRegionDiscovered("First"))
                {
                    var first = regionNavigationService.GetContentRegion("First");
                    //first.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);
                    if (await first.NavigateAsync(typeof(Second)))
                    {
                        if (regionNavigationService.IsContentRegionDiscovered("Second"))
                        {
                            var second = regionNavigationService.GetContentRegion("Second");
                            //second.ConfigureAnimation(translateEntranceAnimation, translateExitAnimation);
                            await second.NavigateAsync(typeof(Three));
                        }
                    }
                }
            }
        }

        private async void OnGoOther(object sender, RoutedEventArgs e)
        {
            await regionNavigationService.GetContentRegion("Zero").NavigateAsync(typeof(Other));
        }
    }

    public class FirstChildAsSelectableViewModel : ISelectable
    {
        public bool IsTarget(Type viewType, object parameter)
        {
            return true;
        }
    }

    public class SecondAsSelectableViewModel : ISelectable
    {
        public bool IsTarget(Type viewType, object parameter)
        {
            return true;
        }
    }

    public class ThreeAsSelectableViewModel : ISelectable
    {
        public bool IsTarget(Type viewType, object parameter)
        {
            return true;
        }
    }

    public class FirstAsSingletonViewModel : IViewLifetimeStrategy
    {
        public StrategyType Strategy => StrategyType.Singleton;
    }

    public class SecondAsSingletonViewModel : IViewLifetimeStrategy
    {
        public StrategyType Strategy => StrategyType.Singleton;
    }

    public class ThreedAsSingletonViewModel : IViewLifetimeStrategy
    {
        public StrategyType Strategy => StrategyType.Singleton;
    }

    public class SampleConfiguration
    {
        public static SampleMode SampleMode { get; set; } = SampleMode.NoViewModel;
    }

    public enum SampleMode
    {
        NoViewModel,
        Selectable,
        Singleton,
        ViewModel
    }
}
