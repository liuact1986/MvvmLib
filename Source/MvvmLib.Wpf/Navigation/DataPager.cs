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
    /// DataPager from DataGrid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPager<T> : INotifyPropertyChanged
    {
        private List<T> innerList;

        private IEnumerable source;
        /// <summary>
        /// The source collection.
        /// </summary>
        public IEnumerable Source
        {
            get { return source; }
        }

        private ObservableCollection<T> items;
        /// <summary>
        /// The items displayed.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return items; }
        }

        private int itemsCount;
        /// <summary>
        /// The items count.
        /// </summary>
        public int ItemsCount
        {
            get { return itemsCount; }
        }

        private int pageSize;
        /// <summary>
        /// The page size.
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
            set { pageIndex = value; }
        }

        private int currentPage;
        /// <summary>
        /// The current page (page index + 1)
        /// </summary>
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
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

        private Func<T, bool> filter;
        /// <summary>
        /// The filter.
        /// </summary>
        public Func<T, bool> Filter
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

        private IComparer<T> customSorter;
        /// <summary>
        /// The custom sorter.
        /// </summary>
        public IComparer<T> CustomSorter
        {
            get { return customSorter; }
            set
            {
                if (!Equals(customSorter, value))
                {
                    customSorter = value;
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
        /// Creates the <see cref="DataPager{T}"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="pageSize">The page size</param>
        public DataPager(IEnumerable<T> source, int pageSize)
            : this(source, pageSize, 0)
        { }

        /// <summary>
        /// Creates the <see cref="DataPager{T}"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="pageIndex">The page index</param>
        public DataPager(IEnumerable<T> source, int pageSize, int pageIndex)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.source = source;
            this.innerList = new List<T>();
            this.items = new ObservableCollection<T>();
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

            var pageCount = Math.Max(1, (int)Math.Ceiling((double)itemCount / PageSize));
            return pageCount;
        }

        private void FilterAndSortItems()
        {
            this.innerList.Clear();
            foreach (T item in this.source)
            {
                if (filter == null || filter(item))
                    this.innerList.Add(item);
            }
            if (customSorter != null)
            {
                this.innerList.Sort(customSorter);
            }
        }

        private void RefreshInternal(int pageIndex)
        {
            FilterAndSortItems();

            this.itemsCount = this.innerList.Count;
            OnPropertyChanged(nameof(ItemsCount));

            this.pageCount = GetPageCount(itemsCount, pageSize);
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
            var oldCanMoveToPreviousPage = CanMoveToPreviousPage;
            var oldCanMoveToNextPage = CanMoveToNextPage;

            if (pageSize <= 0)
                return;
            if (pageIndex < 0 || pageIndex > pageCount - 1)
                return;

            int startIndex = pageIndex * pageSize; // 0 ...5 ...
            int endIndex = startIndex + pageSize;  // 5 ... 10
            if (endIndex < itemsCount)
            {
                Take(startIndex, endIndex);
            }
            else
            {
                // 5.. (3 items) .. 10
                endIndex = itemsCount;
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
        }

        private void Take(int startIndex, int endIndex)
        {
            this.items.Clear();
            for (int i = startIndex; i < endIndex; i++)
            {
                this.items.Add(innerList[i]);
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
    }

}
