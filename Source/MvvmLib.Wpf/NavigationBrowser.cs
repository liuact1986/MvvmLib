using MvvmLib.Mvvm;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allow to browse an items source with <see cref="ICollectionView"/>.
    /// </summary>
    public class NavigationBrowser : INotifyPropertyChanged
    {
        private readonly ObservableCollection<object> itemsSourceForTheView;

        private IEnumerable itemsSource;
        /// <summary>
        /// The items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return itemsSource; }
        }

        private CollectionView collectionView;
        /// <summary>
        /// The collection view.
        /// </summary>
        public CollectionView CollectionView
        {
            get { return collectionView; }
        }

        private object current;
        /// <summary>
        /// The current item
        /// </summary>
        public object Current
        {
            get { return current; }
            private set
            {
                if (value != current)
                {
                    current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.collectionView.CurrentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.collectionView.CurrentPosition < this.collectionView.Count - 1; }
        }

        private readonly IRelayCommand moveCurrentToFirstCommand;
        /// <summary>
        /// Command that allows to move to the first item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToFirstCommand
        {
            get { return moveCurrentToFirstCommand; }
        }

        private readonly IRelayCommand moveCurrentToPreviousCommand;
        /// <summary>
        /// Command that allows to move to the previous item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToPreviousCommand
        {
            get { return moveCurrentToPreviousCommand; }
        }

        private readonly IRelayCommand moveCurrentToNextCommand;
        /// <summary>
        /// Command that allows to move to the next item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToNextCommand
        {
            get { return moveCurrentToNextCommand; }
        }

        private readonly IRelayCommand moveCurrentToLastCommand;
        /// <summary>
        /// Command that allows to move to the last item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToLastCommand
        {
            get { return moveCurrentToLastCommand; }
        }

        private readonly IRelayCommand moveCurrentToPositionCommand;
        /// <summary>
        /// Command that allows to move to the position of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToPositionCommand
        {
            get { return moveCurrentToPositionCommand; }
        }

        private readonly IRelayCommand moveCurrentToCommand;
        /// <summary>
        /// Command that allows to move to the item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToCommand
        {
            get { return moveCurrentToCommand; }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates the <see cref="NavigationBrowser"/>.
        /// </summary>
        /// <param name="itemsSource">The items source</param>
        public NavigationBrowser(IEnumerable itemsSource)
        {
            this.itemsSource = itemsSource;
            if (itemsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)itemsSource).CollectionChanged += OnItemsSourceCollectionChanged;
            }

            this.itemsSourceForTheView = new ObservableCollection<object>();
            // sync
            foreach (var item in ItemsSource)
            {
                var source = NavigationHelper.EnsureNew(item);
                this.itemsSourceForTheView.Add(source);
            }

            this.collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.itemsSourceForTheView);
            this.collectionView.CurrentChanged += OnCurrentChanged;

            moveCurrentToFirstCommand = new RelayCommand(() => MoveCurrentToFirst(), () => CanMoveCurrentToPrevious);
            moveCurrentToPreviousCommand = new RelayCommand(() => MoveCurrentToPrevious(), () => CanMoveCurrentToPrevious);
            moveCurrentToNextCommand = new RelayCommand(() => MoveCurrentToNext(), () => CanMoveCurrentToNext);
            moveCurrentToLastCommand = new RelayCommand(() => MoveCurrentToLast(), () => CanMoveCurrentToNext);
            moveCurrentToPositionCommand = new RelayCommand<object>(args =>
            {
                if (args != null)
                {
                    if (int.TryParse(args.ToString(), out int position))
                    {
                        MoveCurrentToPosition(position);
                    }
                }
            });
            moveCurrentToCommand = new RelayCommand<object>(item => MoveCurrentTo(item));

            SetCurrent();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            SetCurrent();
        }

        private void SetCurrent()
        {
            Current = collectionView.CurrentItem;

            moveCurrentToFirstCommand.RaiseCanExecuteChanged();
            moveCurrentToPreviousCommand.RaiseCanExecuteChanged();
            moveCurrentToNextCommand.RaiseCanExecuteChanged();
            moveCurrentToLastCommand.RaiseCanExecuteChanged();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int oldCount = this.itemsSourceForTheView.Count;
            int index = -1;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        this.itemsSourceForTheView.Insert(index, NavigationHelper.EnsureNew(item));
                        index++;
                    }
                    if (oldCount == 0)
                        MoveCurrentToFirst();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        this.itemsSourceForTheView.RemoveAt(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        this.itemsSourceForTheView[index] = NavigationHelper.EnsureNew(item);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Move is not supported");
                case NotifyCollectionChangedAction.Reset:
                    this.itemsSourceForTheView.Clear();
                    break;
            }

            moveCurrentToFirstCommand.RaiseCanExecuteChanged();
            moveCurrentToPreviousCommand.RaiseCanExecuteChanged();
            moveCurrentToNextCommand.RaiseCanExecuteChanged();
            moveCurrentToLastCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Allows to move to the first item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToFirst()
        {
            this.collectionView.MoveCurrentToFirst();
        }

        /// <summary>
        /// Allows to move to the previous item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToPrevious()
        {
            if (CanMoveCurrentToPrevious)
                this.collectionView.MoveCurrentToPrevious();
        }

        /// <summary>
        /// Allows to move to the next item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToNext()
        {
            if (CanMoveCurrentToNext)
                this.collectionView.MoveCurrentToNext();
        }

        /// <summary>
        /// Allows to move to the last item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToLast()
        {
            this.collectionView.MoveCurrentToLast();
        }

        /// <summary>
        /// Allows to move to the position of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToPosition(int position)
        {
            if (position >= 0 && position < this.collectionView.Count)
                this.collectionView.MoveCurrentToPosition(position);
        }

        /// <summary>
        /// Allows to move to the item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentTo(object item)
        {
            if (item != null)
                this.collectionView.MoveCurrentTo(item);
        }
    }
}
