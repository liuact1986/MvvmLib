using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;
using System.Collections.Generic;

namespace MvvmLib.Core.Tests.Mvvm
{
    public class Item : BindableBase, ISyncItem<Item>
    {
        public string Id { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.SetProperty(ref _title, value); }
        }
        // other properties ...

        public bool NeedSync(Item other)
        {
            return Id == other.Id && (this.Title != other.Title); // Testing for each property
        }

        public void Sync(Item other)
        {
            Title = other.Title;
            // etc.
        }

        public bool Equals(Item other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(this.Id))
                return 0;
            
            return this.Id.GetHashCode();
        }
    }

    public class ItemService
    {
        public bool Has(List<Item> items, string id)
        {
            foreach (var item in items)
                if (item.Id == id)
                    return true;

            return false;
        }

        public Item Get(List<Item> items, string id)
        {
            foreach (var item in items)
                if (item.Id == id)
                    return item;

            return null;
        }
    }

    [TestClass]
    public class SyncTests
    {


        [TestMethod]
        public void TestSync()
        {
            var oldItems = new List<Item> {
                new Item { Id = "1", Title = "Title 1" },
                new Item { Id = "2", Title = "Title 2" },
                new Item { Id = "3", Title = "Title 3" }
            };

            // item id 1 removed
            // item 2 updated
            // item 3 not updated
            // item 4 added
            var newItems = new List<Item>{
                new Item{ Id = "2", Title = "Title 2!!!!!!"},
                new Item{ Id = "3", Title = "Title 3"},
                new Item{ Id = "4", Title = "Title 4"}
            };

            SyncUtils.Sync(oldItems, newItems);

            var service = new ItemService();
            Assert.IsFalse(service.Has(oldItems, "1"));

            var item2 = service.Get(oldItems, "2");
            Assert.AreEqual("Title 2!!!!!!", item2.Title);

            var item3 = service.Get(oldItems, "3");
            Assert.AreEqual("Title 3", item3.Title);

            Assert.IsTrue(service.Has(oldItems, "4"));
        }
    }
}
