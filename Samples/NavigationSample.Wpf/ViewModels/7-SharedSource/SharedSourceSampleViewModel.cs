using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class SharedSourceSampleViewModel : INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        public SharedSource<MyItemDetailsViewModel> DetailsSource { get; }

        public ICommand AddCommand { get; }

        public SharedSourceSampleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            DetailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            AddCommand = new DelegateCommand(Add);
        }

        private void Load()
        {
            DetailsSource.Load(new List<MyItemDetailsViewModel>
            {
                new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }),
                new MyItemDetailsViewModel(new MyItem { Name = "Item.2" })
            });
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("SharedSource for ItemsControls, Selectors, etc.");
        }

        private void Add()
        {
            DetailsSource.Add(new MyItemDetailsViewModel(new MyItem { Name = $"Item.{DetailsSource.Items.Count + 1}" }));
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
            Load();
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {

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
                Item.Name = isSelected ? $"{baseName} SELECTED" : baseName;
            }
        }

        private SharedSource<MyItemDetailsViewModel> detailsSource;

        public MyItemDetailsViewModel(MyItem item)
        {
            baseName = item.Name;
            this.item = item;

            detailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            CloseCommand = new DelegateCommand(Close);
        }

        private void Close()
        {
            detailsSource.Remove(this);
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
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
