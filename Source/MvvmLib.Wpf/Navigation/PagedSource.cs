using MvvmLib.Commands;
using MvvmLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// A PagedSource is a Paged CollectionView.
    /// </summary>
    public class PagedSource : IPagedSource
    {
        private ArrayList shadowCollection;
        private ArrayList internalList;
        private bool handleCollectionChanged;
        private Type elementType;
        private bool oldCanMoveCurrentToPrevious;
        private bool oldCanMoveCurrentToNext;

        /// <summary>
        /// Allows to get the item at the index from current page.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item</returns>
        public object this[int index]
        {
            get { return internalList[index]; }
        }

        private IRelayCommand sortByCommand;
        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        public IRelayCommand SortByCommand
        {
            get
            {
                if (sortByCommand == null)
                    sortByCommand = new RelayCommand<string>(ExecuteSortByCommand);
                return sortByCommand;
            }
        }

        private IRelayCommand sortByDescendingCommand;
        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        public IRelayCommand SortByDescendingCommand
        {
            get
            {
                if (sortByDescendingCommand == null)
                    sortByDescendingCommand = new RelayCommand<string>(ExecuteSortByDescendingCommand);
                return sortByDescendingCommand;
            }
        }

        #region Paging

        private int totalCount;
        /// <summary>
        /// The total of items after applying the filter and sorting
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }
        }

        private int itemCount;
        /// <summary>
        /// The number of items for the current page.
        /// </summary>
        public int ItemCount
        {
            get { return itemCount; }
        }

        private int pageSize;
        /// <summary>
        /// The desired number of items per page.
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (!Equals(pageSize, value))
                {
                    pageSize = value;
                    Refresh();
                }
            }
        }

        private int pageCount;
        /// <summary>
        /// The number of pages.
        /// </summary>
        public int PageCount
        {
            get { return pageCount; }
        }

        private int pageIndex;
        /// <summary>
        /// The page index.
        /// </summary>
        public int PageIndex
        {
            get { return pageIndex; }
        }

        private int currentPage;
        /// <summary>
        /// The current page (page index + 1)
        /// </summary>
        public int CurrentPage
        {
            get { return currentPage; }
        }

        private int start;
        /// <summary>
        /// The position of the first item of the page.
        /// </summary>
        public int Start
        {
            get { return start; }
        }

        private int end;
        /// <summary>
        /// The position of the last item of the page.
        /// </summary>
        public int End
        {
            get { return end; }
        }

        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        public bool CanMoveToPreviousPage
        {
            get { return this.pageIndex > 0; }
        }

        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        public bool CanMoveToNextPage
        {
            get { return this.pageIndex < this.pageCount - 1; }
        }

        private IRelayCommand moveToFirstPageCommand;
        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        public IRelayCommand MoveToFirstPageCommand
        {
            get
            {
                if (moveToFirstPageCommand == null)
                    moveToFirstPageCommand = new RelayCommand(ExecuteMoveToFirstPageCommand, CanExecuteMoveToFirstPageCommand);

                return moveToFirstPageCommand;
            }
        }

        private IRelayCommand moveToPreviousPageCommand;
        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        public IRelayCommand MoveToPreviousPageCommand
        {
            get
            {
                if (moveToPreviousPageCommand == null)
                    moveToPreviousPageCommand = new RelayCommand(ExecuteMoveToPreviousPageCommand, CanExecuteMoveToPreviousPageCommand);
                return moveToPreviousPageCommand;
            }
        }

        private IRelayCommand moveToNextPageCommand;
        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        public IRelayCommand MoveToNextPageCommand
        {
            get
            {
                if (moveToNextPageCommand == null)
                    moveToNextPageCommand = new RelayCommand(ExecuteMoveToNextPageCommand, CanExecuteMoveToNextPageCommand);
                return moveToNextPageCommand;
            }
        }

        private IRelayCommand moveToLastPageCommand;
        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        public IRelayCommand MoveToLastPageCommand
        {
            get
            {
                if (moveToLastPageCommand == null)
                    moveToLastPageCommand = new RelayCommand(ExecuteMoveToLastPageCommand, CanExecuteMoveToLastPageCommand);
                return moveToLastPageCommand;
            }
        }

        private IRelayCommand moveToPageCommand;
        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        public IRelayCommand MoveToPageCommand
        {
            get
            {
                if (moveToPageCommand == null)
                    moveToPageCommand = new RelayCommand<object>(ExecuteMoveToPageCommand);
                return moveToPageCommand;
            }
        }

        #endregion // Paging

        #region Move

        private int deferLevel;
        /// <summary>
        /// Gets a value that indicates if DeferRefresh() is in use.
        /// </summary>
        public bool IsRefreshDeferred
        {
            get { return deferLevel > 0; }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveCurrentToPrevious
        {
            get { return this.currentPosition > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveCurrentToNext
        {
            get { return this.currentPosition < this.internalList.Count - 1; }
        }

        private CultureInfo culture;
        /// <summary>
        /// The culture. Used by <see cref="SortDescriptions"/>.
        /// </summary>
        public CultureInfo Culture
        {
            get { return culture; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!Equals(culture, value))
                {
                    culture = value;
                    OnPropertyChanged(nameof(Culture));
                }
            }
        }

        private IEnumerable sourceCollection;
        /// <summary>
        /// The source collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get { return sourceCollection; }
        }

        private Predicate<object> filter;
        /// <summary>
        /// The filter.
        /// </summary>
        public Predicate<object> Filter
        {
            get { return filter; }
            set
            {
                if (!Equals(filter, value))
                {
                    filter = value;
                    Refresh();
                }
            }
        }

        private IComparer customSort;
        /// <summary>
        /// The custom sort.
        /// </summary>
        public IComparer CustomSort
        {
            get { return customSort; }
            set
            {
                if (!Equals(customSort, value))
                {
                    customSort = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Checks if filtering is available.
        /// </summary>
        public bool CanFilter
        {
            get { return true; }
        }

        private SortDescriptionCollection sortDescriptions;
        /// <summary>
        /// The sort descriptions.
        /// </summary>
        public SortDescriptionCollection SortDescriptions
        {
            get { return sortDescriptions; }
        }

        /// <summary>
        /// Checks if sorting is available.
        /// </summary>
        public bool CanSort
        {
            get { return true; }
        }

        /// <summary>
        /// Checks if grouping is available.
        /// </summary>
        public bool CanGroup
        {
            get { return false; }
        }

        /// <summary>
        /// The group descriptions.
        /// </summary>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        /// <summary>
        /// The groups.
        /// </summary>
        public ReadOnlyObservableCollection<object> Groups
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Checks if collection is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.internalList.Count == 0; }
        }

        private int rank;
        /// <summary>
        /// The rank (current position + 1).
        /// </summary>
        public int Rank
        {
            get { return rank; }
        }

        private object currentItem;
        /// <summary>
        /// The current item.
        /// </summary>
        public object CurrentItem
        {
            get { return currentItem; }
        }

        private int currentPosition;
        /// <summary>
        /// The current position.
        /// </summary>
        public int CurrentPosition
        {
            get { return currentPosition; }
        }

        private IRelayCommand moveCurrentToFirstCommand;
        /// <summary>
        /// Allows to move to the first item.
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
        /// Allows to move to the previous item.
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
        /// Allows to move to the next item.
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
        /// Allows to move to the last item.
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
        /// Allows to move to the position.
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
        /// Allows to move to the item.
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

        /// <summary>
        /// Checks if after the last item.
        /// </summary>
        public bool IsCurrentAfterLast
        {
            get { return this.currentPosition > this.internalList.Count; }
        }

        /// <summary>
        /// Checks if is before the first item.
        /// </summary>
        public bool IsCurrentBeforeFirst
        {
            get { return this.currentPosition < 0; }
        }

        #endregion // Move

        #region Editing

        /// <summary>
        /// The new item place holder. At beginning or at the end (default).
        /// </summary>
        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get { return NewItemPlaceholderPosition.None; }
            set
            {
                if (value != NewItemPlaceholderPosition.None)
                    throw new NotSupportedException("NewItemPlaceholder is not currently supported");
            }
        }

        private bool canAddNew;
        /// <summary>
        /// Checks if can add new item.
        /// </summary>
        public bool CanAddNew
        {
            get { return canAddNew; }
        }

        private bool isAddingNew;
        /// <summary>
        /// Checks if is adding new item.
        /// </summary>
        public bool IsAddingNew
        {
            get { return isAddingNew; }
        }

        private object currentAddItem;
        /// <summary>
        /// The current add item.
        /// </summary>
        public object CurrentAddItem
        {
            get { return currentAddItem; }
        }

        private bool canEditItem;
        /// <summary>
        /// Checks if can edit item.
        /// </summary>
        public bool CanEditItem
        {
            get { return canEditItem; }
        }

        private bool isEditingItem;
        /// <summary>
        /// Checks if is editing item.
        /// </summary>
        public bool IsEditingItem
        {
            get { return isEditingItem; }
        }

        private object currentEditItem;
        /// <summary>
        /// The current edit item.
        /// </summary>
        public object CurrentEditItem
        {
            get { return currentEditItem; }
        }

        private bool canRemove;
        /// <summary>
        /// Checks if can remove.
        /// </summary>
        public bool CanRemove
        {
            get { return canRemove; }
        }

        /// <summary>
        /// Checks if can cancel edit.
        /// </summary>
        public bool CanCancelEdit
        {
            get { return currentEditItem is IEditableObject; }
        }

        /// <summary>
        /// Checks if can add new item.
        /// </summary>
        public bool CanAddNewItem
        {
            get { return !IsEditingItem; }
        }

        private IRelayCommand addNewCommand;
        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        public IRelayCommand AddNewCommand
        {
            get
            {
                if (addNewCommand == null)
                    addNewCommand = new RelayCommand(ExecuteAddCommand);
                return addNewCommand;
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
                    editCommand = new RelayCommand(ExecuteEditCommand);
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
                    deleteCommand = new RelayCommand(ExecuteDeleteCommand);
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
        /// Allows to cancel add new or edit item.
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

        #endregion // Editing

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on current changing. Allows to cancel changing.
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Invoked on current changed.
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Invoked on collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Invoked on refreshed.
        /// </summary>
        public event EventHandler Refreshed;

        /// <summary>
        /// Invoked on page changing.
        /// </summary>
        public event EventHandler<PageChangeEventArgs> PageChanging;

        /// <summary>
        /// Invoked on page changed.
        /// </summary>
        public event EventHandler<PageChangeEventArgs> PageChanged;

        /// <summary>
        /// Creates the <see cref="PagedSource"/>.
        /// </summary>
        /// <param name="sourceCollection">The source collection</param>
        public PagedSource(IEnumerable sourceCollection)
        {
            var pageSize = EnumerableHelper.Count(sourceCollection);
            Initialize(sourceCollection, pageSize);
        }

        /// <summary>
        /// Creates the <see cref="PagedSource"/>.
        /// </summary>
        /// <param name="sourceCollection">The source collection</param>
        /// <param name="pageSize">The page size</param>
        public PagedSource(IEnumerable sourceCollection, int pageSize)
        {
            Initialize(sourceCollection, pageSize);
        }

        private void Initialize(IEnumerable sourceCollection, int pageSize)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (pageSize <= 0)
                throw new ArgumentException("The page size cannot be smaller than 1");

            this.sortDescriptions = new SortDescriptionCollection();

            // 3 collections:
            // (1) source collection
            // (2) collection filtered and sorted: shadow collection
            // (3) collection displayed for page size: internal list
            this.sourceCollection = sourceCollection;
            this.shadowCollection = new ArrayList();
            this.internalList = new ArrayList();

            // sync source collection and shadow collection
            foreach (var item in sourceCollection)
            {
                this.shadowCollection.Add(item);
            }

            // editable?
            var type = sourceCollection.GetType();
            if (!type.IsArray && !(type.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>)))
            {
                var elementType = GetElementType(type);
                this.elementType = elementType;
                if (elementType != typeof(object))
                {
                    this.canAddNew = true;
                    this.canEditItem = true;
                    this.canRemove = true;
                }
            }

            // for automatic refresh on source collection changed (item add, removed, reset ...
            if (sourceCollection is INotifyCollectionChanged)
            {
                this.handleCollectionChanged = true;
                ((INotifyCollectionChanged)sourceCollection).CollectionChanged += OnSourceCollectionChanged;
            }

            ((INotifyCollectionChanged)this.sortDescriptions).CollectionChanged += OnSortDesciptionsCollectionChanged;

            this.pageSize = pageSize;
            this.totalCount = shadowCollection.Count;
            this.pageCount = GetPageCount(pageSize);

            // no filter, no sort, move to first page
            MoveToPageInternal(0);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.handleCollectionChanged)
                this.Refresh();
        }

        private void OnSortDesciptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Refresh();
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
        }

        private void OnRefreshed()
        {
            this.Refreshed?.Invoke(this, EventArgs.Empty);
        }

        private void OnPageChanged(int pageIndex)
        {
            PageChanged?.Invoke(this, new PageChangeEventArgs(pageIndex));
        }

        private void OnPageChanging(int pageIndex)
        {
            PageChanging?.Invoke(this, new PageChangeEventArgs(pageIndex));
        }

        private void OnCurrentChanged()
        {
            this.CurrentChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion // Events

        #region Commands

        private void ExecuteSortByCommand(string args)
        {
            SortBy(args, true);
        }

        private void ExecuteSortByDescendingCommand(string args)
        {
            SortByDescending(args, true);
        }

        private void ExecuteMoveToFirstPageCommand()
        {
            this.MoveToFirstPage();
        }

        private bool CanExecuteMoveToFirstPageCommand()
        {
            return CanMoveToPreviousPage;
        }

        private void ExecuteMoveToPreviousPageCommand()
        {
            this.MoveToPreviousPage();
        }

        private bool CanExecuteMoveToPreviousPageCommand()
        {
            return CanMoveToPreviousPage;
        }

        private void ExecuteMoveToNextPageCommand()
        {
            this.MoveToNextPage();
        }

        private bool CanExecuteMoveToNextPageCommand()
        {
            return CanMoveToNextPage;
        }

        private void ExecuteMoveToLastPageCommand()
        {
            this.MoveToLastPage();
        }

        private bool CanExecuteMoveToLastPageCommand()
        {
            return CanMoveToNextPage;
        }

        private void ExecuteMoveToPageCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int currentPage))
                {
                    MoveToPage(currentPage - 1);
                }
            }
        }

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
            AddNew();
        }

        private void ExecuteEditCommand()
        {
            EditCurrentItem();
        }

        private void ExecuteDeleteCommand()
        {
            if (this.currentPosition != -1)
                RemoveAt(currentPosition);
        }

        private void ExecuteSaveCommand()
        {
            Save();
        }

        private void ExecuteCancelCommand()
        {
            Cancel();
        }

        #endregion // Commands

        /// <summary>
        /// Checks if the view contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(object item)
        {
            return this.internalList.Contains(item);
        }

        /// <summary>
        /// Allows to defer refresh. Avoid multiple calls of <see cref="CurrentChanged"/>.
        /// </summary>
        /// <returns>The defer helper.</returns>
        public IDisposable DeferRefresh()
        {
            ++deferLevel;
            return new DeferHelper(this);
        }

        void EndDefer()
        {
            --deferLevel;

            if (deferLevel == 0)
            {
                Refresh();
            }
        }

        /// <summary>
        /// The enumerator. Allows to bind directly to the <see cref="PagedSource"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        public bool MoveCurrentToFirst()
        {
            if (this.internalList.Count > 0)
            {
                return this.MoveCurrentToPositionInternal(0);
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        public bool MoveCurrentToPrevious()
        {
            if (CanMoveCurrentToPrevious)
            {
                return MoveCurrentToPositionInternal(currentPosition - 1);
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        public bool MoveCurrentToNext()
        {
            if (CanMoveCurrentToNext)
            {
                return this.MoveCurrentToPositionInternal(currentPosition + 1);
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        public bool MoveCurrentToLast()
        {
            if (CanMoveCurrentToNext)
            {
                return this.MoveCurrentToPositionInternal(internalList.Count - 1);
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the position.
        /// </summary>
        /// <param name="position">The position</param>
        public bool MoveCurrentToPosition(int position)
        {
            if (position >= 0 && position < this.internalList.Count)
            {
                return this.MoveCurrentToPositionInternal(position);
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        /// <param name="item">The item</param>
        public bool MoveCurrentTo(object item)
        {
            int index = internalList.IndexOf(item);
            if (index != -1)
            {
                if (CheckCanChangeCurrent())
                {
                    SetCurrent(index, item);
                    return true;
                }
            }
            return false;
        }

        private bool MoveCurrentToPositionInternal(int position)
        {
            if (position < 0 || position > internalList.Count - 1)
                throw new IndexOutOfRangeException();

            var item = internalList[position];
            if (CheckCanChangeCurrent())
            {
                SetCurrent(position, item);
                return true;
            }
            return false;
        }

        private bool CheckCanChangeCurrent()
        {
            var eventArgs = new CurrentChangingEventArgs();
            CurrentChanging?.Invoke(this, eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }
            return true;
        }

        private void SetCurrent(int position, object item)
        {
            this.currentPosition = position;
            rank = position != -1 ? position + 1 : -1;
            this.currentItem = item;
            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(Rank));
            OnPropertyChanged(nameof(CurrentItem));

            CheckCanMoveCurrent();

            OnCurrentChanged();
        }

        private void CheckCanMoveCurrent()
        {
            if (CanMoveCurrentToPrevious != oldCanMoveCurrentToPrevious)
            {
                moveCurrentToFirstCommand?.RaiseCanExecuteChanged();
                moveCurrentToPreviousCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveCurrentToPrevious = CanMoveCurrentToPrevious;
            }
            if (CanMoveCurrentToNext != oldCanMoveCurrentToNext)
            {
                moveCurrentToNextCommand?.RaiseCanExecuteChanged();
                moveCurrentToLastCommand?.RaiseCanExecuteChanged();
                this.oldCanMoveCurrentToNext = CanMoveCurrentToNext;
            }
        }


        #region Paging

        private int GetPageCount(int pageSize)
        {
            // number of pages with collection filtered
            var pageCount = Math.Max(1, (int)Math.Ceiling((double)totalCount / pageSize));
            return pageCount;
        }

        private void CheckCanMove(bool oldCanMoveToPreviousPage, bool oldCanMoveToNextPage)
        {
            if (CanMoveToPreviousPage != oldCanMoveToPreviousPage)
            {
                moveToFirstPageCommand?.RaiseCanExecuteChanged();
                moveToPreviousPageCommand?.RaiseCanExecuteChanged();
            }
            if (CanMoveToNextPage != oldCanMoveToNextPage)
            {
                moveToNextPageCommand?.RaiseCanExecuteChanged();
                moveToLastPageCommand?.RaiseCanExecuteChanged();
            }
        }

        private void MoveToPageInternal(int pageIndex)
        {
            OnPageChanging(pageIndex);

            var oldCanMoveToPreviousPage = CanMoveToPreviousPage;
            var oldCanMoveToNextPage = CanMoveToNextPage;

            int startIndex = pageIndex * pageSize; // 0 ...5 ...
            int endCount = startIndex + pageSize;  // 5 ... 10
            if (endCount > shadowCollection.Count)
            {
                // 5.. (3 items) .. 10 => 8
                endCount = shadowCollection.Count;
            }
            TakeItems(startIndex, endCount);


            int itemCount = this.internalList.Count;

            this.itemCount = itemCount;
            this.start = itemCount > 0 ? startIndex + 1 : 0;
            this.end = endCount; // end index + 1
            this.pageIndex = pageIndex;
            this.currentPage = pageIndex + 1;

            OnPropertyChanged(nameof(ItemCount));
            OnPropertyChanged(nameof(Start));
            OnPropertyChanged(nameof(End));
            OnPropertyChanged(nameof(PageIndex));
            OnPropertyChanged(nameof(CurrentPage));

            CheckCanMove(oldCanMoveToPreviousPage, oldCanMoveToNextPage);

            OnPageChanged(pageIndex);

            if (itemCount > 0)
                this.MoveCurrentToPositionInternal(0);
            else
                SetCurrent(-1, null); // check current changing ?
        }

        private void TakeItems(int startIndex, int count)
        {
            this.internalList.Clear();
            for (int index = startIndex; index < count; index++)
            {
                var item = shadowCollection[index];
                this.internalList.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        public bool MoveToFirstPage()
        {
            if (CanMoveToPreviousPage)
            {
                this.MoveToPageInternal(0);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        public bool MoveToPreviousPage()
        {
            if (CanMoveToPreviousPage)
            {
                this.MoveToPageInternal(pageIndex - 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        public bool MoveToNextPage()
        {
            if (CanMoveToNextPage)
            {
                this.MoveToPageInternal(pageIndex + 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        public bool MoveToLastPage()
        {
            if (CanMoveToNextPage)
            {
                this.MoveToPageInternal(pageCount - 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        public bool MoveToPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < pageCount)
            {
                this.MoveToPageInternal(pageIndex);
                return true;
            }
            return false;
        }

        #endregion // Paging

        /// <summary>
        /// Clears the filter.
        /// </summary>
        public void ClearFilter()
        {
            this.Filter = null;
        }

        /// <summary>
        /// Allows to define a filter with generics.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter</param>
        public void FilterBy<T>(Func<T, bool> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            this.Filter = new Predicate<object>(p => filter((T)p));
        }

        private void SortBy(string propertyName, ListSortDirection sortDirection, bool clearSortDescriptions)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (clearSortDescriptions)
                this.SortDescriptions.Clear();

            this.SortDescriptions.Add(new SortDescription(propertyName, sortDirection));
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortByDescending(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Descending, clearSortDescriptions);
        }

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        public void SortBy(string propertyName, bool clearSortDescriptions)
        {
            SortBy(propertyName, ListSortDirection.Ascending, clearSortDescriptions);
        }

        private void ApplySort()
        {
            if (customSort != null)
            {
                this.shadowCollection.Sort(customSort);
            }
            else if (this.sortDescriptions.Count > 0)
            {
                var sortComparer = new SortPropertyComparer(sortDescriptions, culture);
                this.shadowCollection.Sort(sortComparer);
            }
        }

        private void ApplyFilter()
        {
            this.shadowCollection.Clear();
            foreach (var item in this.sourceCollection)
            {
                if (filter == null || filter(item))
                    this.shadowCollection.Add(item);
            }
        }

        /// <summary>
        /// Allows to refresh the internal list. Usefull for a <see cref="SourceCollection"/> that does not implement <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        public void Refresh()
        {
            if (IsRefreshDeferred)
                return;

            // apply filter apply on all  => add only items from source collection that pass filter => require to reset list then filter
            ApplyFilter();
            // apply sort on all items
            ApplySort();

            this.totalCount = shadowCollection.Count;
            OnPropertyChanged(nameof(TotalCount));

            // page count updated on total items changed or page size changed
            this.pageCount = GetPageCount(pageSize);
            OnPropertyChanged(nameof(PageCount));

            MoveToPageInternal(0);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnRefreshed();
        }

        #region editing

        private Type GetElementType(Type type)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == typeof(IList<>)
                    || interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>)))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
            return typeof(object);
        }

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <param name="itemType">The item type</param>
        /// <returns>The item created</returns>
        public object CreateNew(Type itemType)
        {
            var item = SourceResolver.CreateInstance(itemType);
            return item;
        }

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The item created</returns>
        public T CreateNew<T>()
        {
            return (T)CreateNew(typeof(T));
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <returns>The item created</returns>
        public object AddNew()
        {
            if (!canAddNew)
                throw new InvalidOperationException("Add new is not supported");

            var item = CreateNew(elementType);
            AddNewItem(item);
            return item;
        }

        /// <summary>
        /// Adds the new item.
        /// </summary>
        /// <param name="newItem">The new item</param>
        public object AddNewItem(object newItem)
        {
            if (!canAddNew)
                throw new InvalidOperationException("Add new is not supported");
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            // implicit commit
            if (IsAddingNew)
                CommitNew();
            if (IsEditingItem)
                CommitEdit();

            this.handleCollectionChanged = false;

            ((IList)sourceCollection).Add(newItem);

            this.Refresh();

            if (newItem is IEditableObject)
            {
                ((IEditableObject)newItem).BeginEdit();
            }

            this.currentAddItem = newItem;
            this.isAddingNew = true;
            OnPropertyChanged(nameof(CurrentAddItem));
            OnPropertyChanged(nameof(IsAddingNew));

            MoveToLastPage();
            MoveCurrentToLast();

            this.handleCollectionChanged = true;

            return newItem;
        }


        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        public void EditCurrentItem()
        {
            if (this.CurrentItem != null)
                this.EditItem(this.CurrentItem);
        }

        /// <summary>
        /// Begins edit the item.
        /// </summary>
        /// <param name="item">The item</param>
        public void EditItem(object item)
        {
            if (!canEditItem)
                throw new InvalidOperationException("Edit item is not supported");
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // implicit commit
            if (IsAddingNew)
                CommitNew();
            if (IsEditingItem)
                CommitEdit();

            if (item is IEditableObject)
            {
                ((IEditableObject)item).BeginEdit();
            }

            this.currentEditItem = item;
            this.isEditingItem = true;
            OnPropertyChanged(nameof(CurrentEditItem));
            OnPropertyChanged(nameof(IsEditingItem));
        }

        /// <summary>
        /// Cancel adding item.
        /// </summary>
        public void CancelNew()
        {
            if (!this.isAddingNew)
                throw new InvalidOperationException("Not currently adding new item");
            if (isEditingItem)
                throw new InvalidOperationException("An item is currently editing");

            this.handleCollectionChanged = false;
            ((IList)sourceCollection).Remove(this.currentAddItem);

            this.Refresh();

            this.currentAddItem = null;
            this.isAddingNew = false;
            OnPropertyChanged(nameof(CurrentAddItem));
            OnPropertyChanged(nameof(IsAddingNew));

            this.MoveToLastPage();
            this.MoveCurrentToLast();

            this.handleCollectionChanged = true;
        }

        /// <summary>
        /// Cancel edit item.
        /// </summary>
        public void CancelEdit()
        {
            if (!this.isEditingItem)
                throw new InvalidOperationException("Not currently editing item");
            if (isAddingNew)
                throw new InvalidOperationException("An item is currently adding");

            if (currentEditItem is IEditableObject)
            {
                ((IEditableObject)currentEditItem).CancelEdit();
            }

            this.currentEditItem = null;
            this.isEditingItem = false;
            OnPropertyChanged(nameof(CurrentEditItem));
            OnPropertyChanged(nameof(IsEditingItem));
        }

        /// <summary>
        /// Commit adding item.
        /// </summary>
        public void CommitNew()
        {
            if (!this.isAddingNew)
                throw new InvalidOperationException("Not currently adding new item");
            if (isEditingItem)
                throw new InvalidOperationException("An item is currently editing");

            this.currentAddItem = null;
            this.isAddingNew = false;
            OnPropertyChanged(nameof(CurrentAddItem));
            OnPropertyChanged(nameof(IsAddingNew));
        }

        /// <summary>
        /// Commit edit item.
        /// </summary>
        public void CommitEdit()
        {
            if (!this.isEditingItem)
                throw new InvalidOperationException("Not currently editing item");
            if (isAddingNew)
                throw new InvalidOperationException("An item is currently adding. Call CommitNew or CancelNew before add a new item");

            if (currentEditItem is IEditableObject)
            {
                ((IEditableObject)currentEditItem).EndEdit();
            }

            this.currentEditItem = null;
            this.isEditingItem = false;
            OnPropertyChanged(nameof(CurrentEditItem));
            OnPropertyChanged(nameof(IsEditingItem));
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if removed</returns>
        public void Remove(object item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            int position = internalList.IndexOf(item);
            if (position != -1)
            {
                RemoveInternal(position, item);
            }
        }

        /// <summary>
        /// Removes the item at the position.
        /// </summary>
        /// <param name="position">The position</param>
        public void RemoveAt(int position)
        {
            if (position < 0 || position > this.internalList.Count - 1)
                throw new IndexOutOfRangeException();

            var item = internalList[position];
            RemoveInternal(position, item);
        }

        private void RemoveInternal(int position, object item)
        {
            if(!this.canRemove)
                throw new InvalidOperationException("Remove item is not supported");

            int oldPageIndex = pageIndex;
            int oldPosition = this.currentPosition;

            int indexInSourceCollection = EnumerableHelper.IndexOf(sourceCollection, item);

            this.handleCollectionChanged = false;
            ((IList)sourceCollection).RemoveAt(indexInSourceCollection);

            this.Refresh();

            if (pageCount > oldPageIndex)
            {
                if (oldPageIndex > 0)
                    MoveToPage(oldPageIndex);
                SelectItemAfterDeletion(oldPosition, position);
            }
            else
            {
                MoveToLastPage();
                MoveCurrentToLast();
            }

            this.handleCollectionChanged = true;
        }

        private void SelectItemAfterDeletion(int oldPosition, int position)
        {
            int itemCount = this.internalList.Count;
            if (itemCount == 0)
            {
                SetCurrent(-1, null);
                return;
            }

            // remove selected
            // [A] B [C] => [A] C (old 1, new 1)
            // [A] B => A         (old 1, new 0)
            //  A => -1
            if (oldPosition == position)
            {
                if (this.internalList.Count > position)
                    this.MoveCurrentToPositionInternal(oldPosition);
                else
                    this.MoveCurrentToPositionInternal(oldPosition - 1);
            }
            else
            {
                // [x B] C [D] => selectedindex - 1
                // [A B] C [x] => selected index
                if (position < oldPosition)
                    this.MoveCurrentToPositionInternal(oldPosition - 1);
                else
                    this.MoveCurrentToPositionInternal(oldPosition);
            }
        }

        /// <summary>
        /// Ends the edition.
        /// </summary>
        public void Save()
        {
            if (this.IsAddingNew)
                this.CommitNew();
            else if (this.IsEditingItem)
                this.CommitEdit();
        }

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        public void Cancel()
        {
            if (this.IsAddingNew)
                this.CancelNew();
            else if (this.IsEditingItem)
                this.CancelEdit();
        }

        #endregion

        class DeferHelper : IDisposable
        {
            private PagedSource collectionView;

            public DeferHelper(PagedSource collectionView)
            {
                this.collectionView = collectionView;
            }

            public void Dispose()
            {
                if (collectionView != null)
                {
                    collectionView.EndDefer();
                    collectionView = null;
                }

                GC.SuppressFinalize(this);
            }
        }
    }

}
