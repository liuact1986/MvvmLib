using MvvmLib.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Source for Models and ViewModels with a collection of Items and SelectedItem/SelectedIndex. 
    /// It supports Views but its not the usage. 
    /// This is source for ItemsControls, Selectors (ListBox, TabControl), etc. 
    /// The SelectedItem can be binded to the content of ContentControls.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INavigatableCollectionSource<T> : IMovableSource, INotifyPropertyChanged, ISharedSource
    {
        /// <summary>
        /// The logger used.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// The items collection.
        /// </summary>
        IReadOnlyCollection<T> Items { get; }

        /// <summary>
        /// The entry collection.
        /// </summary>
        IReadOnlyCollection<NavigationEntry> Entries { get; }

        /// <summary>
        /// The selected item.
        /// </summary>
        T SelectedItem { get; set; }

        /// <summary>
        /// The selected index.
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// Allows to select inserted item (Select by default).
        /// </summary>
        SelectionHandling SelectionHandling { get; set; }

        /// <summary>
        /// Invoked on selected item changed.
        /// </summary>
        event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the list of items.
        /// </summary>
        /// <param name="items">The items</param>
        SharedSource<T> Load(IEnumerable<T> items);

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the InitItemCollection.
        /// </summary>
        /// <param name="initItems">The collection of implementation types with parameters</param>
        SharedSource<T> Load(InitItemCollection<T> initItems);

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameter">The parameter</param>
        T InsertNew<TImplementation>(int index, object parameter) where TImplementation : T;

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        T InsertNew<TImplementation>(int index) where TImplementation : T;

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="parameter">The parameter</param>
        T InsertNew(int index, object parameter);

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        T InsertNew(int index);

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameter">The parameter</param>
        T AddNew<TImplementation>(object parameter) where TImplementation : T;

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        T AddNew<TImplementation>() where TImplementation : T;

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        T AddNew(object parameter);

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        T AddNew();

        /// <summary>
        /// Inserts the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter</param>
        void Insert(int index, T item, object parameter);

        /// <summary>
        /// Inserts the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        void Insert(int index, T item);

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter</param>
        void Add(T item, object parameter);

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item</param>
        void Add(T item);

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        void RemoveAt(int index);

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        void Remove(T item);

        /// <summary>
        /// Moves item  at old index to new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        void Move(int oldIndex, int newIndex);

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="newItem">The new item</param>
        /// <param name="parameter">The parameter</param>
        void Replace(int index, T newItem, object parameter);

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="newItem">The new item</param>
        void Replace(int index, T newItem);

        /// <summary>
        /// Clears all items.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all items without checking <see cref="ICanDeactivate"/> and <see cref="INavigationAware.OnNavigatingFrom"/>.
        /// </summary>
        void ClearFast();

        /// <summary>
        /// Synchronizes the shared source with the shared source provided.
        /// </summary>
        void Sync(SharedSource<T> sharedSource);

    }

}
