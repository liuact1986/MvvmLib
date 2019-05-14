using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public interface IListHistory
    {
        NavigationEntry Current { get; }
        int CurrentIndex { get; }
        IReadOnlyList<NavigationEntry> List { get; }
        NavigationEntry Previous { get; }
        NavigationEntry Root { get; }

        void Clear();
        void Insert(int index, NavigationEntry entry);
        void RemoveAt(int index);
        void Select(int index);
    }
}