using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationQueueViewModel
    {
        private readonly IRegionNavigationService regionNavigationService;
        private ItemsRegion itemsRegion;

        public ICommand AddItemsCommand { get; set; }

        public ICommand RemoveItemsCommand { get; set; }

        public AnimationQueueViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
            this.itemsRegion = regionNavigationService.GetItemsRegion("ItemsRegion");
            this.itemsRegion.ConfigureAnimation(
                new TranslateAnimation { From = 120, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(500)) },
                new TranslateAnimation
                {
                    From = 0,
                    To = 120,
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                    OnPrepare = (t) =>
                    {
                        //var itemsCount = itemsRegion.History.List.Count;
                        //var ms = itemsCount > 0 ? 300 / itemsCount : 300;
                        //t.Duration = new Duration(TimeSpan.FromMilliseconds(ms));
                    }
                });


            this.AddItemsCommand = new RelayCommand(AddItems);
            this.RemoveItemsCommand = new RelayCommand(RemoveItems);
        }

        private async void RemoveItems()
        {
            var count = itemsRegion.History.List.Count;
            if (count > 0)
            {
                //for (int i = 0; i < count; i++)
                //    await itemsRegion.RemoveAtAsync(0);

                for (int i = count - 1; i >= 0; i--)
                    await itemsRegion.RemoveAtAsync(i);
            }
        }

        private async void AddItems()
        {
            int count = 5;
            var itemsCount = itemsRegion.History.List.Count;
            for (int i = 0; i < count; i++)
                await itemsRegion.AddAsync(typeof(ItemDetailsView), $"Item.{itemsCount + i}");
        }

        //private async void AddItems()
        //{
        //    // view model
        //    int count = 5;
        //    var itemsCount = itemsRegion.History.List.Count;
        //    for (int i = 0; i < count; i++)
        //        await itemsRegion.AddAsync(typeof(ItemDetailsViewModel), $"Item.{itemsCount + i}");
        //}

    }

}
