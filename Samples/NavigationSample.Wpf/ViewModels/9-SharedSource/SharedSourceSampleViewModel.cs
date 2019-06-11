﻿using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class SharedSourceSampleViewModel : SyncTitleViewModel
    {
        public SharedSource<MyItemDetailsViewModel> DetailsSource { get; }

        public ICommand AddCommand { get; }

        public SharedSourceSampleViewModel(IEventAggregator eventAggregator)
              : base(eventAggregator)
        {
            this.Title = "SharedSource for ItemsControls, Selectors, etc.";

            //DetailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            DetailsSource = NavigationManager.GetSharedSource<MyItemDetailsViewModel>().With(new List<MyItemDetailsViewModel>
            {
                new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }),
                new MyItemDetailsViewModel(new MyItem { Name = "Item.2" })
            });


            AddCommand = new RelayCommand(Add);
        }

        private async void Add()
        {
            await DetailsSource.Items.AddAsync(new MyItemDetailsViewModel(new MyItem { Name = $"Item.{DetailsSource.Items.Count + 1}" }));
        }
    }

    public class MyItemDetailsViewModel : BindableBase, ICanDeactivate, IIsSelected
    {
        private string baseName;
        private MyItem item;
        public MyItem Item
        {
            get { return item; }
            set { item = value; }
        }

        public ICommand CloseCommand { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                Item.Name = isSelected ? $"{baseName} ACTIVE" : baseName;
            }
        }

        private SharedSource<MyItemDetailsViewModel> detailsSource;

        public MyItemDetailsViewModel(MyItem item)
        {
            baseName = item.Name;
            this.item = item;

            detailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            CloseCommand = new RelayCommand(Close);
        }

        private async void Close()
        {
            await detailsSource.Items.RemoveAsync(this);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show($"Close {baseName}?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }

    public class MyItem : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

    }
}
