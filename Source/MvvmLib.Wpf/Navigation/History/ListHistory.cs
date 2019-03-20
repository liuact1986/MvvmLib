using System.Collections.Generic;
using System.Linq;

namespace MvvmLib.Navigation
{

    public class ListHistory : IListHistory
    {
        List<NavigationEntry> list;
        public IReadOnlyList<NavigationEntry> List => list;

        public NavigationEntry Root => this.list.FirstOrDefault();

        public NavigationEntry Previous => this.CurrentIndex > 0 ? this.list.ElementAt(this.CurrentIndex - 1) : null;

        public NavigationEntry Current => CurrentIndex >= 0 && CurrentIndex < this.list.Count ? this.list[CurrentIndex] : null;

        public int CurrentIndex { get; private set; }

        public ListHistory()
        {
            this.list = new List<NavigationEntry>();
            CurrentIndex = -1;
        }

        public void Select(int index)
        {
            this.CurrentIndex = index;
        }

        public void Insert(int index, NavigationEntry entry)
        {
            this.list.Insert(index, entry);
            this.CurrentIndex = index;
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
            this.CurrentIndex = index - 1;
        }

        public void Clear()
        {
            this.list.Clear();
            this.CurrentIndex = -1;
        }

        public List<object> ToList()
        {
            var result = new List<object>();
            foreach (var item in this.List)
            {
                result.Add(item.ViewOrObject);
            }
            return result;
        }
    }

}
