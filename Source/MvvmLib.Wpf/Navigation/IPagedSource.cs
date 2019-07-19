using MvvmLib.Commands;
using System;
using System.Collections;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Paged CollectionView interface.
    /// </summary>
    public interface IPagedCollectionView
    {
        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        bool CanMoveToPreviousPage { get; }

        /// <summary>
        /// Checks if can move to previous page.
        /// </summary>
        bool CanMoveToNextPage { get; }

        /// <summary>
        /// The total of items after applying the filter and sorting.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// The number of items for the current page.
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// The desired number of items per page.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// The number of pages.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// The page index.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// The current page (page index + 1).
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// The position of the first item of the page.
        /// </summary>
        int Start { get; }

        /// <summary>
        /// The position of the last item of the page.
        /// </summary>
        int End { get; }

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
        bool MoveToFirstPage();

        /// <summary>
        /// Allows to move to the previous page.
        /// </summary>
        bool MoveToPreviousPage();

        /// <summary>
        /// Allows to move to the next page.
        /// </summary>
        bool MoveToNextPage();

        /// <summary>
        /// Allows to move to the last page.
        /// </summary>
        bool MoveToLastPage();

        /// <summary>
        /// Allows to move to the page.
        /// </summary>
        bool MoveToPage(int pageIndex);
    }


    /// <summary>
    /// A PagedSource is a Paged CollectionView.
    /// </summary>
    public interface IPagedSource : IPagedCollectionView, ICollectionView, IEditableCollectionViewAddNewItem, INotifyPropertyChanged
    {
        /// <summary>
        /// Allows to get the item at the index from current page.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item</returns>
        object this[int index] { get; }

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        IRelayCommand SortByCommand { get; }

        /// <summary>
        /// Allows to group by property name.
        /// </summary>
        IRelayCommand SortByDescendingCommand { get; }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        bool CanMoveCurrentToPrevious { get; }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        bool CanMoveCurrentToNext { get; }

        /// <summary>
        /// The custom sort.
        /// </summary>
        IComparer CustomSort { get; set; }

        /// <summary>
        /// The rank (current position + 1).
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        IRelayCommand MoveCurrentToFirstCommand { get; }

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        IRelayCommand MoveCurrentToPreviousCommand { get; }

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        IRelayCommand MoveCurrentToNextCommand { get; }

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        IRelayCommand MoveCurrentToLastCommand { get; }

        /// <summary>
        /// Allows to move to the position.
        /// </summary>
        IRelayCommand MoveCurrentToPositionCommand { get; }

        /// <summary>
        /// Allows to move to the rank (index + 1).
        /// </summary>
        IRelayCommand MoveCurrentToRankCommand { get; }

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        IRelayCommand MoveCurrentToCommand { get; }

        /// <summary>
        /// Checks if can edit item.
        /// </summary>
        bool CanEditItem { get; }

        /// <summary>
        /// Allows to add a new item.
        /// </summary>
        IRelayCommand AddNewCommand { get; }

        /// <summary>
        /// Allows to begin edit the current item.
        /// </summary>
        IRelayCommand EditCommand { get; }

        /// <summary>
        /// Allows to delete the current item.
        /// </summary>
        IRelayCommand DeleteCommand { get; }

        /// <summary>
        /// Allows to save changes for current item.
        /// </summary>
        IRelayCommand SaveCommand { get; }

        /// <summary>
        /// Allows to cancel add new or edit item.
        /// </summary>
        IRelayCommand CancelCommand { get; }

        /// <summary>
        /// Gets a value that indicates if DeferRefresh() is in use.
        /// </summary>
        bool IsRefreshDeferred { get; }

        /// <summary>
        /// Invoked on refreshed.
        /// </summary>
        event EventHandler Refreshed;
   
        /// <summary>
        /// Clears the filter.
        /// </summary>
        void ClearFilter();

        /// <summary>
        /// Allows to define a filter with generics.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter</param>
        void FilterBy<T>(Predicate<T> filter);

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        void SortByDescending(string propertyName, bool clearSortDescriptions);

        /// <summary>
        /// Allows to add a sort description.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="clearSortDescriptions">Allows to clear sort descriptions</param>
        void SortBy(string propertyName, bool clearSortDescriptions);

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <param name="itemType">The item type</param>
        /// <returns>The item created</returns>
        object CreateNew(Type itemType);

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. Allows to inject dependencies if the factory is overridden with an IoC Container. 
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The item created</returns>
        T CreateNew<T>();

        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        void EditCurrentItem();

        /// <summary>
        /// Ends the edition.
        /// </summary>
        void Save();

        /// <summary>
        /// Allows to cancel edition and changes.
        /// </summary>
        void Cancel();
    }

}
