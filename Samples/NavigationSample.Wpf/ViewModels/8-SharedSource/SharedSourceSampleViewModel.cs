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
        public SharedSource<MyItemDetailsViewModel> DetailsSource { get; }

        public ICommand AddCommand { get; }

        public SharedSourceSampleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("SharedSource for ItemsControls, Selectors, etc.");

            DetailsSource = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            AddCommand = new RelayCommand(Add);
        }

        private void Add()
        {
            DetailsSource.Items.Add(new MyItemDetailsViewModel(new MyItem { Name = $"Item.{DetailsSource.Items.Count + 1}" }));
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            DetailsSource.Load(new List<MyItemDetailsViewModel>
            {
                new MyItemDetailsViewModel(new MyItem { Name = "Item.1" }),
                new MyItemDetailsViewModel(new MyItem { Name = "Item.2" })
            });
        }

        public void OnNavigatedTo(object parameter)
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

            CloseCommand = new RelayCommand(Close);
        }

        private void Close()
        {
            detailsSource.Items.Remove(this);
        }

        public void CanDeactivate(Action<bool> continuationCallback)
        {
            var result = MessageBox.Show($"Close {baseName}?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            continuationCallback(result);
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
