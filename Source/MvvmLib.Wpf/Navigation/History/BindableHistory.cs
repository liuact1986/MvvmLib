using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    public class NavigationEntryBaseEventArgs : EventArgs
    {
        private readonly NavigationEntry entry;
        public NavigationEntry Entry
        {
            get { return entry; }
        }

        private readonly int? index;
        public int? Index
        {
            get { return index; }
        }

        public NavigationEntryBaseEventArgs(NavigationEntry entry, int? index)
        {
            this.entry = entry;
            this.index = index;
        }
    }

    public class NavigationEntryAddedEventArgs : NavigationEntryBaseEventArgs
    {
        public NavigationEntryAddedEventArgs(NavigationEntry entry, int? index) : base(entry, index)
        {
        }
    }

    public class NavigationEntryRemovedEventArgs : NavigationEntryBaseEventArgs
    {
        public NavigationEntryRemovedEventArgs(NavigationEntry entry, int? index) : base(entry, index)
        {
        }
    }

    public class NavigationEntryUpdatedEventArgs : NavigationEntryBaseEventArgs
    {
        private NavigationEntry originalEntry;
        public NavigationEntry OriginalEntry
        {
            get { return originalEntry; }
        }

        public NavigationEntryUpdatedEventArgs(NavigationEntry originaleEntry, NavigationEntry entry, int? index) : base(entry, index)
        {
            this.originalEntry = originalEntry;
        }
    }

    public interface INotifyHistoryChanged
    {
        event EventHandler<NavigationEntryAddedEventArgs> EntryAdded;
        event EventHandler<NavigationEntryRemovedEventArgs> EntryRemoved;
        event EventHandler<NavigationEntryUpdatedEventArgs> EntryUpdated;
    }

    public class BindableHistory : Collection<NavigationEntry>, INotifyHistoryChanged, INotifyPropertyChanged
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public event EventHandler<NavigationEntryAddedEventArgs> EntryAdded;
        public event EventHandler<NavigationEntryRemovedEventArgs> EntryRemoved;
        public event EventHandler<NavigationEntryUpdatedEventArgs> EntryUpdated;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void InsertItem(int index, NavigationEntry item)
        {
            base.InsertItem(index, item);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            this.EntryAdded?.Invoke(this, new NavigationEntryAddedEventArgs(item, index));
        }

        protected override void RemoveItem(int index)
        {
            var entry = base.Items[index];

            base.RemoveItem(index);

            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            this.EntryRemoved?.Invoke(this, new NavigationEntryRemovedEventArgs(entry, index));
        }

        protected override void SetItem(int index, NavigationEntry item)
        {
            var originalEntry = base.Items[index];
            base.SetItem(index, item);

            OnPropertyChanged(IndexerName);
            this.EntryUpdated?.Invoke(this, new NavigationEntryUpdatedEventArgs(originalEntry, item, index));
        }

        protected override void ClearItems()
        {
            int count = Items.Count;
            for (int i = count - 1; i >= 0; i--)
                this.RemoveAt(i);
        }
    }

}
