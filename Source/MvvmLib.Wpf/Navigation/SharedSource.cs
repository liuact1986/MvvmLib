using MvvmLib.Commands;
using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Source for Models and ViewModels with a collection of Items and SelectedItem/SelectedIndex. 
    /// It supports Views but its not the usage. 
    /// This is source for ItemsControls, Selectors (ListBox, TabControl), etc. 
    /// The SelectedItem can be binded to the content of ContentControls.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SharedSource<T> : IMovableSource, INotifyPropertyChanged, ISharedSource
    {
        private readonly ILogger DefaultLogger = new DebugLogger();
        private bool handleSelectionChanged;

        private ILogger logger;
        /// <summary>
        /// The logger used.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        private ObservableCollection<T> items;
        /// <summary>
        /// The items collection.
        /// </summary>
        public IReadOnlyCollection<T> Items
        {
            get { return items; }
        }

        private readonly List<NavigationEntry> entries;
        /// <summary>
        /// The entry collection.
        /// </summary>
        public IReadOnlyCollection<NavigationEntry> Entries
        {
            get { return entries; }
        }

        private T selectedItem;
        /// <summary>
        /// The selected item.
        /// </summary>
        public T SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (!Equals(selectedItem, value))
                {
                    SetSelectedItem(value);
                }
            }
        }


        private int selectedIndex;
        /// <summary>
        /// The selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (!Equals(selectedIndex, value))
                {
                    var oldCanMoveToPrevious = CanMoveToPrevious;
                    var oldCanMoveToNext = CanMoveToNext;

                    SetSelectedIndex(value);

                    CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
                }
            }
        }

        private SelectionHandling selectionHandling;
        /// <summary>
        /// Allows to select inserted item (Select by default).
        /// </summary>
        public SelectionHandling SelectionHandling
        {
            get { return selectionHandling; }
            set { selectionHandling = value; }
        }

        /// <summary>
        /// Checks if can move to previous item.
        /// </summary>
        public bool CanMoveToPrevious
        {
            get { return this.selectedIndex > 0; }
        }

        /// <summary>
        /// Checks if can move to next item.
        /// </summary>
        public bool CanMoveToNext
        {
            get { return this.selectedIndex < this.items.Count - 1; }
        }


        private IRelayCommand moveToFirstCommand;
        /// <summary>
        /// Allows to move to the first source.
        /// </summary>
        public IRelayCommand MoveToFirstCommand
        {
            get
            {
                if (moveToFirstCommand == null)
                    moveToFirstCommand = new RelayCommand(ExecuteMoveToFirstCommand, CanExecuteMoveToFirstCommand);
                return moveToFirstCommand;
            }
        }

        private IRelayCommand moveToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous source.
        /// </summary>
        public IRelayCommand MoveToPreviousCommand
        {
            get
            {
                if (moveToPreviousCommand == null)
                    moveToPreviousCommand = new RelayCommand(ExecuteMoveToPreviousCommand, CanExecuteMoveToPreviousCommand);
                return moveToPreviousCommand;
            }
        }

        private IRelayCommand moveToNextCommand;
        /// <summary>
        /// Allows to move to the next source.
        /// </summary>
        public IRelayCommand MoveToNextCommand
        {
            get
            {
                if (moveToNextCommand == null)
                    moveToNextCommand = new RelayCommand(ExecuteMoveToNextCommand, CanExecuteMoveToNextCommand);
                return moveToNextCommand;
            }
        }

        private IRelayCommand moveToLastCommand;
        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        public IRelayCommand MoveToLastCommand
        {
            get
            {
                if (moveToLastCommand == null)
                    moveToLastCommand = new RelayCommand(ExecuteMoveToLastCommand, CanExecuteMoveToLastCommand);
                return moveToLastCommand;
            }
        }

        private IRelayCommand moveToIndexCommand;
        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        public IRelayCommand MoveToIndexCommand
        {
            get
            {
                if (moveToIndexCommand == null)
                    moveToIndexCommand = new RelayCommand<object>(ExecuteMoveToIndexCommand);
                return moveToIndexCommand;
            }
        }

        private IRelayCommand moveToCommand;
        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        public IRelayCommand MoveToCommand
        {
            get
            {
                if (moveToCommand == null)
                    moveToCommand = new RelayCommand<object>(ExecuteMoveToCommand);
                return moveToCommand;
            }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on selected item changed.
        /// </summary>
        public event EventHandler<SharedSourceSelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Invoked on <see cref="CanMoveToPrevious"/> changed.
        /// </summary>
        public event EventHandler<CanMoveToEventArgs> CanMoveToPreviousChanged;

        /// <summary>
        /// Invoked on <see cref="CanMoveToNext"/> changed.
        /// </summary>
        public event EventHandler<CanMoveToEventArgs> CanMoveToNextChanged;

        /// <summary>
        /// Creates the <see cref="SharedSource{T}"/>.
        /// </summary>
        public SharedSource()
        {
            this.items = new ObservableCollection<T>();
            this.entries = new List<NavigationEntry>();
            this.selectionHandling = SelectionHandling.Select;
            this.handleSelectionChanged = true;
            this.selectedIndex = -1;
        }

        #region Commands

        private void ExecuteMoveToFirstCommand()
        {
            MoveToFirst();
        }

        private bool CanExecuteMoveToFirstCommand()
        {
            return CanMoveToPrevious;
        }

        private void ExecuteMoveToPreviousCommand()
        {
            MoveToPrevious();
        }

        private bool CanExecuteMoveToPreviousCommand()
        {
            return CanMoveToPrevious;
        }

        private void ExecuteMoveToNextCommand()
        {
            MoveToNext();
        }

        private bool CanExecuteMoveToNextCommand()
        {
            return CanMoveToNext;
        }

        private void ExecuteMoveToLastCommand()
        {
            MoveToLast();
        }

        private bool CanExecuteMoveToLastCommand()
        {
            return CanMoveToNext;
        }

        private void ExecuteMoveToIndexCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int position))
                {
                    MoveTo(position);
                }
            }
        }

        private void ExecuteMoveToCommand(object args)
        {
            MoveTo(args);
        }

        #endregion // Commands

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSelectedItemChanged(int index, object item)
        {
            SelectedItemChanged?.Invoke(this, new SharedSourceSelectedItemChangedEventArgs(index, item));
        }


        private void OnCanMoveToPreviousChanged(bool canMove)
        {
            moveToFirstCommand?.RaiseCanExecuteChanged();
            moveToPreviousCommand?.RaiseCanExecuteChanged();

            CanMoveToPreviousChanged?.Invoke(this, new CanMoveToEventArgs(canMove));
        }

        private void OnCanMoveToNextChanged(bool canMove)
        {
            moveToNextCommand?.RaiseCanExecuteChanged();
            moveToLastCommand?.RaiseCanExecuteChanged();

            CanMoveToNextChanged?.Invoke(this, new CanMoveToEventArgs(canMove));
        }

        #endregion // Events

        private void SetCurrent(T value)
        {
            selectedItem = value;
            OnPropertyChanged(nameof(SelectedItem));
        }

        private void SetCurrentIndex(int index)
        {
            selectedIndex = index;
            OnPropertyChanged(nameof(SelectedIndex));
        }

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the list of items.
        /// </summary>
        /// <param name="items">The items</param>
        public SharedSource<T> Load(IEnumerable<T> items)
        {
            this.handleSelectionChanged = false;
            this.ClearFast();
            int index = 0;
            foreach (var originalItem in items)
            {
                var item = NavigationHelper.EnsureNewView(originalItem);
                var type = item.GetType();
                this.InsertItemInternal(index, type, (T)item, new NavigationContext(type, null));
                index++;
            }
            this.handleSelectionChanged = true;
            if (this.items.Count > 0)
                TrySelectItem(0);

            return this;
        }

        /// <summary>
        /// Initializes the <see cref="SharedSource{T}"/> with the InitItemCollection.
        /// </summary>
        /// <param name="initItems">The collection of implementation types with parameters</param>
        public SharedSource<T> Load(InitItemCollection<T> initItems)
        {
            this.handleSelectionChanged = false;
            this.ClearFast();
            int index = 0;
            foreach (var initItem in initItems)
            {
                var originalItem = initItem.Key;
                var item = NavigationHelper.EnsureNewView(originalItem);
                var type = item.GetType();
                this.InsertItemInternal(index, type, (T)item, new NavigationContext(type, initItem.Value));
                index++;
            }
            this.handleSelectionChanged = true;
            if (this.items.Count > 0)
                TrySelectItem(0);

            return this;
        }

        private void TrySelectItem(int index)
        {
            if (handleSelectionChanged && selectionHandling == SelectionHandling.Select)
                SetSelectedIndex(index);
        }

        private void TrySetIsSelected(object item, bool isSelected)
        {
            var view = item as FrameworkElement;
            if (view != null)
            {
                if (view.DataContext is IIsSelected)
                    ((IIsSelected)view.DataContext).IsSelected = isSelected;
            }
            else
            {
                if (item is IIsSelected)
                    ((IIsSelected)item).IsSelected = isSelected;
            }
        }

        private void SelectItemAfterDeletion(int index)
        {
            if (this.items.Count == 0)
            {
                this.SetSelectedIndex(-1);
                return;
            }

            // remove selected
            // [A] B [C] => [A] C (old 1, new 1)
            // [A] B => A         (old 1, new 0)
            //  A => -1
            var oldIndex = this.selectedIndex;
            if (oldIndex == index)
            {
                if (this.items.Count > index)
                    this.SetSelectedIndex(oldIndex);
                else
                    this.SetSelectedIndex(oldIndex - 1);
            }
            else
            {
                // [x B] C [D] => selectedindex - 1
                // [A B] C [x] => selected index
                if (index < oldIndex)
                    this.SetSelectedIndex(oldIndex - 1);
                else
                    this.SetSelectedIndex(oldIndex);
            }
        }

        private void UpdateIsSelected()
        {
            if (selectedIndex == -1)
                return;

            int index = 0;
            foreach (var item in items)
            {
                if (index == selectedIndex)
                    TrySetIsSelected(item, true);
                else
                    TrySetIsSelected(item, false);
                index++;
            }
        }

        private void SetSelectedIndex(int value)
        {
            if (value < -1 || value > items.Count - 1)
                throw new IndexOutOfRangeException();

            SetCurrentIndex(value);

            if (value == -1)
                SetCurrent(default(T));
            else
            {
                var newSelectedItem = this.items[value];
                if (!Equals(newSelectedItem, selectedItem))
                    SetCurrent(newSelectedItem);
            }

            OnSelectedItemChanged(selectedIndex, selectedItem);
            UpdateIsSelected();
        }

        private void SetSelectedItem(T value)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            if (Equals(value, default(T)))
            {
                SetCurrentIndex(-1);
                SetCurrent(value);
            }
            else
            {
                var index = this.items.IndexOf(value);
                if (index == -1)
                    throw new InvalidOperationException("Only existing source can be set to current");

                if (!Equals(selectedIndex, index))
                    SetCurrentIndex(index);

                SetCurrent(value);
            }

            OnSelectedItemChanged(selectedIndex, selectedItem);
            UpdateIsSelected();

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        private void CheckCanMoveTo(bool oldCanMoveToPrevious, bool oldCanMoveToNext)
        {
            if (CanMoveToPrevious != oldCanMoveToPrevious)
                OnCanMoveToPreviousChanged(CanMoveToPrevious);
            if (CanMoveToNext != oldCanMoveToNext)
                OnCanMoveToNextChanged(CanMoveToNext);
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <returns>The instance created</returns>
        public T CreateNew()
        {
            var item = SourceResolver.CreateInstance(typeof(T));
            return (T)item;
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <returns>The instance created</returns>
        public T CreateNew<TImplementation>() where TImplementation : T
        {
            var item = SourceResolver.CreateInstance(typeof(TImplementation));
            return (T)item;
        }

        /// <summary>
        /// Creates a new instance with the <see cref="SourceResolver"/> factory.
        /// </summary>
        /// <param name="implementationType">The implementation type</param>
        /// <returns></returns>
        public T CreateNew(Type implementationType)
        {
            var item = SourceResolver.CreateInstance(implementationType);
            return (T)item;
        }

        private void InsertItemInternal(int index, Type type, T item, NavigationContext navigationContext)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            var view = item as FrameworkElement;
            if (view != null)
            {
                NavigationHelper.OnNavigatingTo(view.DataContext, navigationContext);
                this.items.Insert(index, item);
                NavigationHelper.OnNavigatedTo(view.DataContext, navigationContext);
            }
            else
            {
                NavigationHelper.OnNavigatingTo(item, navigationContext);
                this.items.Insert(index, item);
                NavigationHelper.OnNavigatedTo(item, navigationContext);
            }
            this.entries.Insert(index, new NavigationEntry(type, navigationContext.Parameter));

            TrySelectItem(index);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        private void CheckAndInsertItemInternal(int index, T item, object parameter)
        {
            if (index < 0 || index > this.items.Count)
                throw new IndexOutOfRangeException();

            var type = item.GetType();
            var selectable = NavigationHelper.FindSelectable(items, type, parameter);
            if (selectable != null)
            {
                SelectedItem = (T)selectable;
                return;
            }

            var navigationContext = new NavigationContext(type, parameter);
            NavigationHelper.CanActivate(item, navigationContext, canActivate =>
            {
                if (canActivate)
                    InsertItemInternal(index, type, item, navigationContext);
            });
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameter">The parameter</param>
        public T InsertNew<TImplementation>(int index, object parameter) where TImplementation : T
        {
            var item = this.CreateNew<TImplementation>();
            CheckAndInsertItemInternal(index, item, parameter);
            return item;
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        public T InsertNew<TImplementation>(int index) where TImplementation : T
        {
            return InsertNew<TImplementation>(index, null);
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="parameter">The parameter</param>
        public T InsertNew(int index, object parameter)
        {
            var item = this.CreateNew();
            CheckAndInsertItemInternal(index, item, parameter);
            return item;
        }

        /// <summary>
        /// Inserts an item at the index. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="index">The index</param>
        public T InsertNew(int index)
        {
            return InsertNew(index, null);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="parameter">The parameter</param>
        public T AddNew<TImplementation>(object parameter) where TImplementation : T
        {
            return InsertNew<TImplementation>(this.items.Count, parameter);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        public T AddNew<TImplementation>() where TImplementation : T
        {
            return InsertNew<TImplementation>(this.items.Count, null);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public T AddNew(object parameter)
        {
            return InsertNew(this.items.Count, parameter);
        }

        /// <summary>
        /// Adds an item. <see cref="ICanActivate"/> guard is checked and if the item is a ViewModel that implement <see cref="INavigationAware"/>, the ViewModel is notified.
        /// </summary>
        public T AddNew()
        {
            return InsertNew(this.items.Count, null);
        }

        /// <summary>
        /// Inserts the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter</param>
        public void Insert(int index, T item, object parameter)
        {
            CheckAndInsertItemInternal(index, item, parameter);
        }

        /// <summary>
        /// Inserts the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        public void Insert(int index, T item)
        {
            this.CheckAndInsertItemInternal(index, item, null);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="parameter">The parameter</param>
        public void Add(T item, object parameter)
        {
            this.CheckAndInsertItemInternal(this.items.Count, item, parameter);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(T item)
        {
            this.CheckAndInsertItemInternal(this.items.Count, item, null);
        }

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveAt(int index)
        {
            if (index < 0 && index > this.items.Count - 1)
                throw new IndexOutOfRangeException();

            var item = this.items[index];

            var entry = this.entries[index];
            var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
            NavigationHelper.CanDeactivate(item, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                {
                    var oldCanMoveToPrevious = CanMoveToPrevious;
                    var oldCanMoveToNext = CanMoveToNext;

                    NavigationHelper.OnNavigatingFrom(item, navigationContext);

                    this.items.RemoveAt(index);
                    this.entries.RemoveAt(index);

                    SelectItemAfterDeletion(index);

                    CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
                }
            });
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item</param>
        public void Remove(T item)
        {
            int index = items.IndexOf(item);
            if (index == -1)
                Logger.Log($"Unable to find the index for the item \"{item}\"", Category.Warn, Priority.High);
            else
                RemoveAt(index);
        }

        /// <summary>
        /// Moves item  at old index to new index.
        /// </summary>
        /// <param name="oldIndex">The old index</param>
        /// <param name="newIndex">The new index</param>
        public void Move(int oldIndex, int newIndex)
        {
            var entry = entries[oldIndex];
            entries.RemoveAt(oldIndex);
            entries.Insert(newIndex, entry);

            items.Move(oldIndex, newIndex);

            TrySelectItem(newIndex);
        }

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="newItem">The new item</param>
        /// <param name="parameter">The parameter</param>
        public void Replace(int index, T newItem, object parameter)
        {
            if (index < 0 || index > this.items.Count - 1)
                throw new IndexOutOfRangeException();

            var oldItem = items[index];
            var type = newItem.GetType();
            var navigationContext = new NavigationContext(type, parameter);
            NavigationHelper.CanDeactivate(oldItem, navigationContext, canDeactivate =>
             {
                 if (canDeactivate)
                 {
                     NavigationHelper.CanActivate(newItem, navigationContext, canActivate =>
                     {
                         if (canActivate)
                         {
                             var view = newItem as FrameworkElement;
                             if (view != null)
                             {
                                 NavigationHelper.OnNavigatingFrom(oldItem, navigationContext);

                                 NavigationHelper.OnNavigatingTo(view.DataContext, navigationContext);

                                 this.items[index] = newItem;
                                 this.entries[index] = new NavigationEntry(type, navigationContext.Parameter);

                                 TrySelectItem(index);

                                 NavigationHelper.OnNavigatedTo(view.DataContext, navigationContext);
                             }
                             else
                             {
                                 NavigationHelper.OnNavigatingFrom(oldItem, navigationContext);

                                 NavigationHelper.OnNavigatingTo(newItem, navigationContext);

                                 this.items[index] = newItem;
                                 this.entries[index] = new NavigationEntry(type, navigationContext.Parameter);

                                 TrySelectItem(index);

                                 NavigationHelper.OnNavigatedTo(newItem, navigationContext);
                             }
                         }
                     });
                 }
             });
        }

        /// <summary>
        /// Replaces the old item at thhe index by the new item.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="newItem">The new item</param>
        public void Replace(int index, T newItem)
        {
            Replace(index, newItem, null);
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void Clear()
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            int count = items.Count;
            // 3 ... 2 ... 1 ... 0 
            for (int index = count - 1; index >= 0; index--)
                this.RemoveAt(index);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        /// <summary>
        /// Clears all items without checking <see cref="ICanDeactivate"/> and <see cref="INavigationAware.OnNavigatingFrom"/>.
        /// </summary>
        public void ClearFast()
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            this.items.Clear();
            this.entries.Clear();

            SelectItemAfterDeletion(-1);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        /// <summary>
        /// Allows to move to the first item.
        /// </summary>
        public void MoveToFirst()
        {
            if (CanMoveToPrevious)
                this.MoveTo(0);
        }

        /// <summary>
        /// Allows to move to the previous item.
        /// </summary>
        public void MoveToPrevious()
        {
            if (CanMoveToPrevious)
                this.MoveTo(this.selectedIndex - 1);
        }

        /// <summary>
        /// Allows to move to the next item.
        /// </summary>
        public void MoveToNext()
        {
            if (CanMoveToNext)
                this.MoveTo(this.selectedIndex + 1);
        }

        /// <summary>
        /// Allows to move to the last item.
        /// </summary>
        public void MoveToLast()
        {
            if (CanMoveToNext)
                this.MoveTo(this.items.Count - 1);
        }

        /// <summary>
        /// Allows to move to the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            if (index >= 0 && index < this.items.Count)
            {
                var oldCanMoveToPrevious = CanMoveToPrevious;
                var oldCanMoveToNext = CanMoveToNext;

                this.SetSelectedIndex(index);

                CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
            }
        }

        /// <summary>
        /// Allows to move to the item.
        /// </summary>
        /// <param name="item">The item</param>
        public void MoveTo(object item)
        {
            if (item is T)
            {
                var index = this.items.IndexOf((T)item);
                this.MoveTo(index);
            }
        }

        /// <summary>
        /// Synchronizes the shared source with the shared source provided.
        /// </summary>
        public void Sync(SharedSource<T> sharedSource)
        {
            var entries = sharedSource.Entries;
            var items = sharedSource.Items;
            var selectedIndex = sharedSource.SelectedIndex;

            this.handleSelectionChanged = false;
            this.ClearFast();
            int index = 0;
            foreach (var entry in entries)
            {
                var type = entry.SourceType;
                if (!typeof(T).IsAssignableFrom(type))
                    throw new ArgumentException($"The type '{type.Name}' not inherits from '{typeof(T).Name}'");

                var originalItem = items.ElementAt(index);
                var item = NavigationHelper.EnsureNewView(originalItem);

                this.items.Insert(index, (T)item);
                this.entries.Insert(index, new NavigationEntry(type, entry.Parameter));
                index++;
            }
            this.handleSelectionChanged = true;
            if (selectedIndex >= 0 && this.items.Count > 0)
                TrySelectItem(selectedIndex);
        }
    }
}
