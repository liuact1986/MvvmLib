using MvvmLib.Message;
using MvvmLib.Mvvm;
using NavigationSample.Wpf.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{

    public class AnimationQueueViewModel : SyncTitleViewModel
    {
        private bool isCancelled;
        public bool IsCancelled
        {
            get { return isCancelled; }
            set { SetProperty(ref isCancelled, value); }
        }

        public ObservableCollection<ItemDetailsViewModel> MyItems { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AnimationQueueViewModel(IEventAggregator eventAggregator)
              : base(eventAggregator)
        {
            this.Title = "Animation with TransitioningItemsControl + ControlledAnimation ";

            this.MyItems = new ObservableCollection<ItemDetailsViewModel>();

            this.AddCommand = new RelayCommand(Add);
            this.InsertCommand = new RelayCommand(Insert);
            this.ClearCommand = new RelayCommand(Clear);
            this.CancelCommand = new RelayCommand(Cancel);

            Singleton<EventAggregator>.Instance.GetEvent<ItemDeletedEvent>().Subscribe(OnDeleteItemEventFired);
        }

        private void Cancel()
        {
            IsCancelled = true;
            IsCancelled = false;
        }

        private void InsertInternal(int index)
        {
            var item = new Item { Name = $"Item Inserted at index {index}" };
            var itemDetailsViewModel = new ItemDetailsViewModel(item);
            MyItems.Insert(index, itemDetailsViewModel);
        }

        private void Insert()
        {
            AddInternal();
            AddInternal();
            InsertInternal(1);
        }

        private void OnDeleteItemEventFired(ItemDetailsViewModel itemDetailsViewModel)
        {
            MyItems.Remove(itemDetailsViewModel);
        }

        private void AddInternal()
        {
            var item = new Item { Name = $"Item {MyItems.Count + 1}" };
            var itemDetailsViewModel = new ItemDetailsViewModel(item);
            MyItems.Add(itemDetailsViewModel);
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
            MyItems.Clear();
        }
    }


    public class ItemDetailsViewModel : BindableBase
    {
        private Item item;
        public Item Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }

        public ICommand DeleteItemCommand { get; set; }

        public ItemDetailsViewModel(Item item)
        {
            this.Item = item;
            this.DeleteItemCommand = new RelayCommand(Delete);
        }

        private async void Delete()
        {
            //await Task.Delay(4000);

            Singleton<EventAggregator>.Instance.GetEvent<ItemDeletedEvent>().Publish(this);

            //await Task.Delay(4000);
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
