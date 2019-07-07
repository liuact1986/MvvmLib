using MvvmLib.Commands;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// PagedSource for DataGrid, etc.
    /// </summary>
    public interface IPagedSource : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable
    {
        /// <summary>
        /// Allows to get the item at the index from current page.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The item</returns>
        object this[int index] { get; }

        /// <summary>
        /// The source collection.
        /// </summary>
        IEnumerable SourceCollection { get; }

        /// <summary>
        /// The total of items after applying the filter and sorting
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
        /// The current page (page index + 1)
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
        /// The filter.
        /// </summary>
        Predicate<object> Filter { get; set; }

        /// <summary>
        /// The custom sort.
        /// </summary>
        IComparer CustomSort { get; set; }

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
        /// The current item.
        /// </summary>
        object CurrentItem { get; }

        /// <summary>
        /// The current position.
        /// </summary>
        int CurrentPosition { get; set; }

        /// <summary>
        /// The rank (current position + 1).
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        bool CanMoveCurrentToPrevious { get; }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        bool CanMoveCurrentToNext { get; }

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
        /// Checks if can add new item.
        /// </summary>
        bool CanAddNew { get; }

        /// <summary>
        /// Checks if is adding new item.
        /// </summary>
        bool IsAddingNew { get; }

        /// <summary>
        /// The current add item.
        /// </summary>
        object CurrentAddItem { get; }

        /// <summary>
        /// Checks if can edit item.
        /// </summary>
        bool CanEditItem { get; }

        /// <summary>
        /// Checks if is editing item.
        /// </summary>
        bool IsEditingItem { get; }

        /// <summary>
        /// The current edit item.
        /// </summary>
        object CurrentEditItem { get; }

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
        /// Allows to refresh the internal list. Usefull for a <see cref="SourceCollection"/> that does not implement <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        void Refresh();

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

        /// <summary>
        /// Clears the filter.
        /// </summary>
        void ClearFilter();

        /// <summary>
        /// Allows to define a filter with generics.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="filter">The filter</param>
        void FilterBy<T>(Func<T, bool> filter);

        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        bool MoveCurrentToFirst();

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        bool MoveCurrentToPrevious();

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        bool MoveCurrentToNext();

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        bool MoveCurrentToLast();

        /// <summary>
        /// Allows to move to the position.
        /// </summary>
        /// <param name="position">The position</param>
        bool MoveCurrentToPosition(int position);

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        /// <param name="item">The item</param>
        bool MoveCurrentTo(object item);

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
        /// Adds a new item.
        /// </summary>
        /// <returns>The item created</returns>
        object AddNew();

        /// <summary>
        /// Adds the new item.
        /// </summary>
        /// <param name="item">The item</param>
        void AddNewItem(object item);

        /// <summary>
        /// Allows to begin edit. If the current item implements <see cref="IEditableObject"/>, "BeginEdit" is invoked.
        /// </summary>
        void EditCurrentItem();
        /// <summary>
        /// Begins edit the item.
        /// </summary>
        /// <param name="item">The item</param>
        void EditItem(object item);

        /// <summary>
        /// Cancel adding item.
        /// </summary>
        void CancelNew();

        /// <summary>
        /// Cancel edit item.
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Commit adding item.
        /// </summary>
        void CommitNew();

        /// <summary>
        /// Commit edit item.
        /// </summary>
        void CommitEdit();

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if removed</returns>
        bool Remove(object item);

        /// <summary>
        /// Removes the item at the position.
        /// </summary>
        /// <param name="position">The position</param>
        void RemoveAt(int position);

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
