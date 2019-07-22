using MvvmLib.Commands;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{

    public class AnimationQueueViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator eventAggregator;

        private bool isCancelled;
        public bool IsCancelled
        {
            get { return isCancelled; }
            set { SetProperty(ref isCancelled, value); }
        }

        public SharedSource<ItemDetailsViewModel> MyItemsSource { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AnimationQueueViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.MyItemsSource = new SharedSource<ItemDetailsViewModel>();

            this.AddCommand = new DelegateCommand(Add);
            this.InsertCommand = new DelegateCommand(Insert);
            this.ClearCommand = new DelegateCommand(Clear);
            this.CancelCommand = new DelegateCommand(Cancel);

            eventAggregator.GetEvent<ItemDeletedEvent>().Subscribe(OnDeleteItemEventFired);
        }

        private void Cancel()
        {
            IsCancelled = true;
            IsCancelled = false;
        }

        private void InsertInternal(int index)
        {
            var item = new Item { Name = $"Item Inserted at index {index}" };
            MyItemsSource.InsertNew(index, item);
        }

        private void Insert()
        {
            AddInternal();
            AddInternal();
            InsertInternal(1);
        }

        private void OnDeleteItemEventFired(ItemDetailsViewModel itemDetailsViewModel)
        {
            MyItemsSource.Remove(itemDetailsViewModel);
        }

        private void AddInternal()
        {
            var item = new Item { Name = $"Item {MyItemsSource.Items.Count + 1}" };
            MyItemsSource.AddNew(item);
        }

        public void Add()
        {
            AddInternal();
            AddInternal();
            AddInternal();
            AddInternal();
        }

        private void Clear()
        {
            MyItemsSource.Clear();
        }

        private void SetTitle()
        {
            eventAggregator.GetEvent<TitleChangedEvent>().Publish("Animation with TransitioningItemsControl + ControlledAnimation ");
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            SetTitle();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }


    public class ItemDetailsViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator eventAggregator;

        private Item item;
        public Item Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }

        public ICommand DeleteItemCommand { get; set; }

        public ItemDetailsViewModel(IEventAggregator eventAggregator)
        {
            this.DeleteItemCommand = new DelegateCommand(Delete);
            this.eventAggregator = eventAggregator;
        }

        private async void Delete()
        {
            //await Task.Delay(4000);

            eventAggregator.GetEvent<ItemDeletedEvent>().Publish(this);

            //await Task.Delay(4000);
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
           
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            this.Item = navigationContext.Parameter as Item;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
           
        }
    }


    public class Item
    {
        static int lastId = 0;
        static int GetId()
        {
            lastId++;
            return lastId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Item()
        {
            Id = GetId();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ItemDeletedEvent : ParameterizedEvent<ItemDetailsViewModel>
    {

    }
}
