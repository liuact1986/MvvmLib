using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Navigation
{

    public sealed class ListHistory : IListHistory
    {
        private readonly List<NavigationEntry> list;
        public IReadOnlyList<NavigationEntry> List
        {
            get { return list; }
        }

        public NavigationEntry Root
        {
            get { return this.list.FirstOrDefault(); }
        }

        public NavigationEntry Previous
        {
            get { return this.CurrentIndex > 0 ? this.list.ElementAt(this.CurrentIndex - 1) : null; }
        }

        public NavigationEntry Current
        {
            get { return CurrentIndex >= 0 && CurrentIndex < this.list.Count ? this.list[CurrentIndex] : null; }
        }

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
        }

        public ListHistory()
        {
            this.list = new List<NavigationEntry>();
            currentIndex = -1;
        }

        public void Select(int index)
        {
            currentIndex = index;
        }

        public void Insert(int index, NavigationEntry entry)
        {
            this.list.Insert(index, entry);
            currentIndex = index;
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
            currentIndex = index - 1;
        }

        public void Clear()
        {
            this.list.Clear();
            currentIndex = -1;
        }

        public List<object> ToList()
        {
            var result = new List<object>();
            foreach (var item in this.List)
                result.Add(item.ViewOrObject);

            return result;
        }
    }

}
