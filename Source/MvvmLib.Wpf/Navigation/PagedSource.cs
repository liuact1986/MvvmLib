using MvvmLib.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// PagedSource for DataGrid, etc.
    /// </summary>
    public class PagedSource : IPagedSource
    {
        private ObservableCollection<object> items;
        private ArrayList internalList;

        private readonly IEnumerable sourceCollection;
        /// <summary>
        /// The source collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get { return sourceCollection; }
        }

        private int itemCount;
        /// <summary>
        /// The item count.
        /// </summary>
        public int ItemCount
        {
            get { return itemCount; }
        }

        private int pageSize;
        /// <summary>
        /// The number of items by page.
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (!Equals(pageSize, value))
                {
                    pageSize = value;
                    RefreshInternal(0);
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
                    RefreshInternal(0);
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
                    RefreshInternal(0);
                }
            }
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

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Invoked on collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Creates the <see cref="PagedSource"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="pageSize">The page size</param>
        public PagedSource(IEnumerable<object> source, int pageSize)
            : this(source, pageSize, 0)
        { }

        /// <summary>
        /// Creates the <see cref="PagedSource"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="pageIndex">The page index</param>
        public PagedSource(IEnumerable source, int pageSize, int pageIndex)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.sourceCollection = source;
            this.internalList = new ArrayList();
            this.items = new ObservableCollection<object>();
            this.pageSize = pageSize;

            if (source is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)source).CollectionChanged += OnSourceCollectionChanged;
            }

            RefreshInternal(pageIndex);
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        #endregion // Events

        #region Commands

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

        #endregion // Commands

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshInternal(pageIndex);
        }

        private int GetPageCount(int itemCount, int pageSize)
        {
            if (pageSize <= 0)
                return 0;

            var pageCount = Math.Max(1, (int)Math.Ceiling((double)itemCount / pageSize));
            return pageCount;
        }

        private void FilterAndSortItems()
        {
            this.internalList.Clear();
            foreach (var item in this.sourceCollection)
            {
                if (filter == null || filter(item))
                    this.internalList.Add(item);
            }
            if (customSort != null)
            {
                this.internalList.Sort(customSort);
            }
        }

        private void RefreshInternal(int pageIndex)
        {
            FilterAndSortItems();

            this.itemCount = this.internalList.Count;
            OnPropertyChanged(nameof(ItemCount));

            this.pageCount = GetPageCount(itemCount, pageSize);
            OnPropertyChanged(nameof(PageCount));

            if (pageCount > 0)
                MoveToPage(pageIndex);

            OnRefreshed();
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

            if (pageSize <= 0)
                return;
            if (pageIndex < 0 || pageIndex > pageCount - 1)
                return;

            int startIndex = pageIndex * pageSize; // 0 ...5 ...
            int endIndex = startIndex + pageSize;  // 5 ... 10
            if (endIndex >= itemCount)
            {
                // 5.. (3 items) .. 10
                endIndex = itemCount;
            }

            Take(startIndex, endIndex);

            this.start = startIndex + 1;
            this.end = endIndex;
            this.pageIndex = pageIndex;
            this.currentPage = pageIndex + 1;

            OnPropertyChanged(nameof(Start));
            OnPropertyChanged(nameof(End));
            OnPropertyChanged(nameof(PageIndex));
            OnPropertyChanged(nameof(CurrentPage));

            CheckCanMove(oldCanMoveToPreviousPage, oldCanMoveToNextPage);

            OnPageChanged(pageIndex);
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void Take(int startIndex, int endIndex)
        {
            this.items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            for (int index = startIndex; index < endIndex; index++)
            {
                var item = internalList[index];
                this.items.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
        }

        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        public void MoveToFirstPage()
        {
            this.MoveToPage(0);
        }

        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        public void MoveToPreviousPage()
        {
            this.MoveToPage(this.PageIndex - 1);
        }

        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        public void MoveToNextPage()
        {
            this.MoveToPage(this.PageIndex + 1);
        }

        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        public void MoveToLastPage()
        {
            this.MoveToPage(this.PageCount - 1);
        }

        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        public void MoveToPage(int pageIndex)
        {
            MoveToPageInternal(pageIndex);
        }

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

        /// <summary>
        /// Gets the enumerator. Allows to bind to the paged source directly.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
