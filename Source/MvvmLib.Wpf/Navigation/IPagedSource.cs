using MvvmLib.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// DataPager for DataGrid, etc.
    /// </summary>
    public interface IPagedSource : INotifyPropertyChanged
    {
        /// <summary>
        /// The source collection.
        /// </summary>
        IEnumerable Source { get; }

        /// <summary>
        /// The items displayed.
        /// </summary>
        ObservableCollection<object> Items { get; }

        /// <summary>
        /// The items count.
        /// </summary>
        int ItemsCount { get; }

        /// <summary>
        /// The number of items by page.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// The number of pages.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// The page index.
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// The current page (page index + 1)
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// The position of the first item of the page.
        /// </summary>
        int Start { get; }

        /// <summary>
        /// The position of the last item of the page.
        /// </summary>
        int End { get; }

        /// <summary>
        /// The filter.
        /// </summary>
        Func<object, bool> Filter { get; set; }

        /// <summary>
        /// The custom sorter.
        /// </summary>
        IComparer<object> CustomSorter { get; set; }

        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        bool CanMoveToPreviousPage { get; }
        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        bool CanMoveToNextPage { get; }

        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        IRelayCommand MoveToFirstPageCommand { get; }

        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        IRelayCommand MoveToPreviousPageCommand { get; }

        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        IRelayCommand MoveToNextPageCommand { get; }

        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        IRelayCommand MoveToLastPageCommand { get; }

        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        IRelayCommand MoveToPageCommand { get; }

        /// <summary>
        /// Invoked on refreshed.
        /// </summary>
        event EventHandler Refreshed;

        /// <summary>
        /// Invoked on page changing.
        /// </summary>
        event EventHandler<PageChangeEventArgs> PageChanging;

        /// <summary>
        /// Invoked on page changed.
        /// </summary>
        event EventHandler<PageChangeEventArgs> PageChanged;

        /// <summary>
        /// Allows to move to the first page.
        /// </summary>
        void MoveToFirstPage();

        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        void MoveToPreviousPage();

        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        void MoveToNextPage();

        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        void MoveToLastPage();

        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        void MoveToPage(int pageIndex);
    }

}
