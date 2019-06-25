using MvvmLib.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allow to browse an items source with <see cref="ICollectionView"/>. 
    /// Avoid calling <see cref="ICanActivate"/> and <see cref="ICanDeactivate"/> Guards and <see cref="INavigationAware"/> methods for <see cref="NavigationSource"/> and <see cref="SharedSource{T}"/>.
    /// </summary>
    public class NavigationBrowser : INotifyPropertyChanged
    {
        private readonly ObservableCollection<object> innerList;
        private Type elementType;
        private bool implementsINotifyCollectionChanged;

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

        private int rank;
        /// <summary>
        /// The rank (index + 1).
        /// </summary>
        public int Rank
        {
            get { return rank; }
        }

        private bool canAddEditDelete;
        /// <summary>
        /// Checks if Add, Edit, Delete is possible for the <see cref="ItemsSource"/>. 
        /// Available for collections that implement <see cref="IList{T}" /> with an element type not object.
        /// </summary>
        public bool CanAddEditDelete
        {
            get { return canAddEditDelete; }
        }

        private NavigationBrowserState state;
        /// <summary>
        /// The current state.
        /// </summary>
        public NavigationBrowserState State
        {
            get { return state; }
            set
            {
                if (!Equals(state, value))
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.CollectionView.CurrentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.collectionView.CurrentPosition < this.collectionView.Count - 1; }
        }


        private IRelayCommand moveCurrentToFirstCommand;
        /// <summary>
        /// Allows to move to the first item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToFirstCommand
        {
            get
            {
                if (moveCurrentToFirstCommand == null)
                    moveCurrentToFirstCommand = new RelayCommand(ExecuteMoveCurrentToFirstCommand, CanExecuteMoveCurrentToFirstCommand);

                return moveCurrentToFirstCommand;
            }
        }

        private IRelayCommand moveCurrentToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToPreviousCommand
        {
            get
            {
                if (moveCurrentToPreviousCommand == null)
                    moveCurrentToPreviousCommand = new RelayCommand(ExecuteMoveCurrentToPreviousCommand, CanExecuteMoveToPreviousCommand);
                return moveCurrentToPreviousCommand;
            }
        }

        private IRelayCommand moveCurrentToNextCommand;
        /// <summary>
        /// Allows to move to the next item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToNextCommand
        {
            get
            {
                if (moveCurrentToNextCommand == null)
                    moveCurrentToNextCommand = new RelayCommand(ExecuteMoveCurrentToNextCommand, CanExecuteMoveToNextCommand);
                return moveCurrentToNextCommand;
            }
        }


        private IRelayCommand moveCurrentToLastCommand;
        /// <summary>
        /// Allows to move to the last item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToLastCommand
        {
            get
            {
                if (moveCurrentToLastCommand == null)
                    moveCurrentToLastCommand = new RelayCommand(ExecuteMoveCurrentToLastCommand, CanExecuteMoveCurrentToLastCommand);
                return moveCurrentToLastCommand;
            }
        }

        private IRelayCommand moveCurrentToPositionCommand;
        /// <summary>
        /// Allows to move to the position of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToPositionCommand
        {
            get
            {
                if (moveCurrentToPositionCommand == null)
                    moveCurrentToPositionCommand = new RelayCommand<object>(ExecuteMoveCurrentToPositionCommand);
                return moveCurrentToPositionCommand;
            }
        }

        private IRelayCommand moveCurrentToRankCommand;
        /// <summary>
        /// Allows to move to the rank (index + 1).
        /// </summary>
        public IRelayCommand MoveCurrentToRankCommand
        {
            get
            {
                if (moveCurrentToRankCommand == null)
                    moveCurrentToRankCommand = new RelayCommand<object>(ExecuteMoveCurrentToRankCommand);
                return moveCurrentToRankCommand;
            }
        }

        private IRelayCommand moveCurrentToCommand;
        /// <summary>
        /// Allows to move to the item of the <see cref="CollectionView"/>.
        /// </summary>
        public IRelayCommand MoveCurrentToCommand
        {
            get
            {
                if (moveCurrentToCommand == null)
                    moveCurrentToCommand = new RelayCommand<object>(ExecuteMoveCurrentToCommand);
                return moveCurrentToCommand;
            }
        }

        private IRelayCommand addCommand;
        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        public IRelayCommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddEditDeleteCommand);
                return addCommand;
            }
        }

        private IRelayCommand editCommand;
        /// <summary>
        /// Allows to begin edit the current item.
        /// </summary>
        public IRelayCommand EditCommand
        {
            get
            {
                if (editCommand == null)
                    editCommand = new RelayCommand(ExecuteEditCommand, CanExecuteAddEditDeleteCommand);
                return editCommand;
            }
        }

        private IRelayCommand deleteCommand;
        /// <summary>
        /// Allows to delete the current item.
        /// </summary>
        public IRelayCommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new RelayCommand(ExecuteDeleteCommand, CanExecuteAddEditDeleteCommand);
                return deleteCommand;
            }
        }

        private IRelayCommand saveCommand;
        /// <summary>
        /// Allows to save changes for current item.
        /// </summary>
        public IRelayCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(ExecuteSaveCommand);
                return saveCommand;
            }
        }

        private IRelayCommand cancelCommand;
        /// <summary>
        /// Allows to cancel edit the current item.
        /// </summary>
        public IRelayCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new RelayCommand(ExecuteCancelCommand);
                return cancelCommand;
            }
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
            if (itemsSource == null)
                throw new ArgumentNullException(nameof(itemsSource));

            this.itemsSource = itemsSource;

            var type = itemsSource.GetType();
            if (itemsSource is INotifyCollectionChanged)
            {
                this.implementsINotifyCollectionChanged = true;
                ((INotifyCollectionChanged)itemsSource).CollectionChanged += OnItemsSourceCollectionChanged;
            }

            this.innerList = new ObservableCollection<object>();
            // sync
            foreach (var item in ItemsSource)
            {
                var source = NavigationHelper.EnsureNewView(item);
                this.innerList.Add(source);
            }

            this.collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.innerList);

            this.collectionView.CurrentChanged -= OnCurrentChanged;
            this.collectionView.CurrentChanged += OnCurrentChanged;

            var elementType = GetElementTypeForReadableGenericListOrCollection(type);
            if (elementType != null && elementType != typeof(object))
            {
                this.canAddEditDelete = true;
                this.elementType = elementType;
            }

            SetCurrent();

            State = NavigationBrowserState.IsReading;
        }

        private Type GetElementTypeForReadableGenericListOrCollection(Type type)
        {
            if (type.IsArray)
                return null;

            if (type.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
                return null;

            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == typeof(IList<>)
                    || interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
            return null;
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            SetCurrent();
        }

        #endregion // Events

        #region Commands

        private void ExecuteMoveCurrentToFirstCommand()
        {
            this.MoveCurrentToFirst();
        }

        private bool CanExecuteMoveCurrentToFirstCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        private void ExecuteMoveCurrentToPreviousCommand()
        {
            this.MoveCurrentToPrevious();
        }

        private bool CanExecuteMoveToPreviousCommand()
        {
            return CanMoveCurrentToPrevious;
        }

        private void ExecuteMoveCurrentToNextCommand()
        {
            this.MoveCurrentToNext();
        }

        private bool CanExecuteMoveToNextCommand()
        {
            return CanMoveCurrentToNext;
        }

        private void ExecuteMoveCurrentToLastCommand()
        {
            this.MoveCurrentToLast();
        }


        private bool CanExecuteMoveCurrentToLastCommand()
        {
            return CanMoveCurrentToNext;
        }

        private void ExecuteMoveCurrentToCommand(object args)
        {
            MoveCurrentTo(args);
        }

        private void ExecuteMoveCurrentToPositionCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int position))
                {
                    MoveCurrentToPosition(position);
                }
            }
        }

        private void ExecuteMoveCurrentToRankCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int rank))
                {
                    MoveCurrentToPosition(rank - 1);
                }
            }
        }

        private void ExecuteAddCommand()
        {
            Add();
        }

        private void ExecuteDeleteCommand()
        {
            Delete();
        }

        private void ExecuteEditCommand()
        {
            Edit();
        }

        private void ExecuteSaveCommand()
        {
            Save();
        }

        private void ExecuteCancelCommand()
        {
            Cancel();
        }

        private bool CanExecuteAddEditDeleteCommand()
        {
            return CanAddEditDelete;
        }

        #endregion // Commands

        private void SetCurrent()
        {
            Current = collectionView.CurrentItem;
            rank = collectionView.CurrentPosition + 1;
            OnPropertyChanged(nameof(Rank));

            moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
            moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
            moveCurrentToNextCommand?.RaiseCanExecuteChanged();
            moveCurrentToLastCommand?.RaiseCanExecuteChanged();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var oldCanMoveCurrentToPrevious = CanMoveCurrentToPrevious;
            var oldCanMoveCurrentToNext = CanMoveCurrentToNext;
            int index;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        this.innerList.Insert(index, NavigationHelper.EnsureNewView(item));
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        this.innerList.RemoveAt(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        this.innerList[index] = NavigationHelper.EnsureNewView(item);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Move is not supported");
                case NotifyCollectionChangedAction.Reset:
                    this.innerList.Clear();
                    break;
            }

            CheckCanMoveCurrent(oldCanMoveCurrentToPrevious, oldCanMoveCurrentToNext);
        }

        private void CheckCanMoveCurrent(bool oldCanMoveCurrentToPrevious, bool oldCanMoveCurrentToNext)
        {
            if (CanMoveCurrentToPrevious != oldCanMoveCurrentToPrevious)
            {
                moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
                moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
            }
            if (CanMoveCurrentToNext != oldCanMoveCurrentToNext)
            {
                moveCurrentToNextCommand?.RaiseCanExecuteChanged();
                moveCurrentToLastCommand?.RaiseCanExecuteChanged();
            }
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
            this.collectionView.MoveCurrentToPrevious();
        }

        /// <summary>
        /// Allows to move to the next item of the <see cref="CollectionView"/>.
        /// </summary>
        public void MoveCurrentToNext()
        {
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
        /// <param name="position">The position</param>
        public void MoveCurrentToPosition(int position)
        {
            if (position >= 0 && position < this.collectionView.Count)
                this.collectionView.MoveCurrentToPosition(position);
        }

        /// <summary>
        /// Allows to move to the item of the <see cref="CollectionView"/>.
        /// </summary>
        /// <param name="item">The item</param>
        public void MoveCurrentTo(object item)
        {
            if (item != null)
                this.collectionView.MoveCurrentTo(item);
        }

        /// <summary>
        /// Allows to add a new item. The item is created with <see cref="SourceResolver"/>.
        /// </summary>
        public void Add()
        {
            if (this.canAddEditDelete)
            {
                var item = SourceResolver.CreateInstance(this.elementType);
                ((IList)ItemsSource).Add(item);
                if (!this.implementsINotifyCollectionChanged)
                    this.innerList.Add(item);
                this.MoveCurrentTo(item);
                this.Edit();
            }
            State = NavigationBrowserState.IsAdding;
        }

        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        public void Edit()
        {
            if (current is IEditableObject)
            {
                ((IEditableObject)current).BeginEdit();
            }
            State = NavigationBrowserState.IsEditing;
        }

        /// <summary>
        /// Allows to delete the current item.
        /// </summary>
        public void Delete()
        {
            if (this.canAddEditDelete)
            {
                if (this.collectionView.CurrentPosition != -1)
                {
                    ((IList)ItemsSource).RemoveAt(this.collectionView.CurrentPosition);
                    if (!this.implementsINotifyCollectionChanged)
                        this.innerList.RemoveAt(this.collectionView.CurrentPosition);
                }
            }
            State = NavigationBrowserState.IsReading;
        }

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        public void Cancel()
        {
            if (current is IEditableObject)
            {
                ((IEditableObject)current).CancelEdit();
            }
            State = NavigationBrowserState.IsReading;
        }

        /// <summary>
        /// Ends the edition.
        /// </summary>
        public void Save()
        {
            if (current is IEditableObject)
            {
                ((IEditableObject)current).EndEdit();
            }
            State = NavigationBrowserState.IsReading;
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public void Clear()
        {
            if (this.canAddEditDelete)
            {
                ((IList)ItemsSource).Clear();
                if (!this.implementsINotifyCollectionChanged)
                    this.innerList.Clear();
            }
        }
    }

    /// <summary>
    /// The <see cref="NavigationBrowser"/> states.
    /// </summary>
    public enum NavigationBrowserState
    {
        /// <summary>
        /// Reading.
        /// </summary>
        IsReading,
        /// <summary>
        /// Adding.
        /// </summary>
        IsAdding,
        /// <summary>
        /// Editing.
        /// </summary>
        IsEditing
    }
}
