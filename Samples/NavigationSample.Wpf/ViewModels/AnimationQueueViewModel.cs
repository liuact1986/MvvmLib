using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.ViewModels
{
    public class AnimationQueueViewModel : BindableBase
    {
        private readonly IRegionNavigationService regionNavigationService;
        private ItemsRegion itemsRegion;
        private ContentRegion contentRegion;

        private string statusMessage;

        public string StatusMessage
        {
            get { return statusMessage; }
            set { SetProperty(ref statusMessage, value); }
        }

        public ICommand AddItemsCommand { get; set; }
        public ICommand RemoveItemsCommand { get; set; }
        public ICommand CancelItemsRegionAnimationCommand { get; set; }


        public ICommand NavigateCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public IRelayCommand GoBackCommand { get; private set; }
        public IRelayCommand GoForwardCommand { get; private set; }
        public ICommand CancelContentRegionAnimationCommand { get; set; }


        public AnimationQueueViewModel(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;
            this.itemsRegion = regionNavigationService.GetItemsRegion("ItemsRegion");
            //this.itemsRegion.ConfigureAnimation(
            //    new TranslateAnimation { From = 120, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(500)) },
            //    new TranslateAnimation
            //    {
            //        From = 0,
            //        To = 120,
            //        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
            //        OnPrepare = (t) =>
            //        {
            //            //var itemsCount = itemsRegion.History.List.Count;
            //            //var ms = itemsCount > 0 ? 300 / itemsCount : 300;
            //            //t.Duration = new Duration(TimeSpan.FromMilliseconds(ms));
            //        }
            //    });
            //this.itemsRegion.AnimationCompleted += OnItemsRegionAnimationCompleted;
            //this.itemsRegion.AnimationCancelled += OnItemsRegionAnimationCancelled;

            this.contentRegion = regionNavigationService.GetContentRegion("ContentRegion");
            //this.contentRegion.ConfigureAnimation(
            //   new TranslateAnimation { From = 120, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(500)) },
            //   new TranslateAnimation { From = 0, To = 120, Duration = new Duration(TimeSpan.FromMilliseconds(500)) });
            //this.contentRegion.AnimationCompleted += OnContentRegionAnimationCompleted;
            //this.contentRegion.AnimationCancelled += OnContentRegionAnimationCancelled;

            this.AddItemsCommand = new RelayCommand(AddItems);
            this.RemoveItemsCommand = new RelayCommand(RemoveItems);

            this.NavigateCommand = new RelayCommand<Type>(OnNavigate);
            this.CancelCommand = new RelayCommand(OnCancel);

            // SWITCH THE NAVIGATION MODE
            //this.contentRegion.RegionNavigationMode = RegionNavigationMode.UseNavigationStack;
            //this.itemsRegion.RegionNavigationMode = RegionNavigationMode.UseNavigationStack;


            contentRegion.CanGoBackChanged += (s, e) =>
            {
                GoBackCommand.RaiseCanExecuteChanged();
            };

            contentRegion.CanGoForwardChanged += (s, e) =>
            {
                GoForwardCommand.RaiseCanExecuteChanged();
            };

            GoBackCommand = new RelayCommand(async () => await contentRegion.GoBackAsync(), () => contentRegion.CanGoBack);
            GoForwardCommand = new RelayCommand(async () => await contentRegion.GoForwardAsync(), () => contentRegion.CanGoForward);

            //CancelContentRegionAnimationCommand = new RelayCommand(() => contentRegion.CancelAnimations());
            //CancelItemsRegionAnimationCommand = new RelayCommand(() => itemsRegion.CancelAnimations());
        }

        private void OnItemsRegionAnimationCancelled(object sender, EventArgs e)
        {
            StatusMessage = $"ItemsRegion Animation CANCELLED {DateTime.Now.ToLongTimeString()}";
        }

        private void OnItemsRegionAnimationCompleted(object sender, EventArgs e)
        {
            StatusMessage = $"ItemsRegion Animation COMPLETED {DateTime.Now.ToLongTimeString()}";
        }

        private void OnContentRegionAnimationCompleted(object sender, EventArgs e)
        {
            StatusMessage = $"ContentRegion Animation COMPLETED {DateTime.Now.ToLongTimeString()}";
        }

        private void OnContentRegionAnimationCancelled(object sender, EventArgs e)
        {
            StatusMessage = $"ContentRegion Animation CANCELLED {DateTime.Now.ToLongTimeString()}";
        }

        private async void OnNavigate(Type sourceType)
        {
            await contentRegion.NavigateAsync(sourceType);
        }

        private void OnCancel()
        {
            ((ContentControl)contentRegion.Control).Content = null;
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
