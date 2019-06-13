using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmLib.Wpf.Tests.Navigation
{

    [TestClass]
    public class SharedSourceTests
    {
        [TestMethod]
        public void Initialization_With_List()
        {
            var item1 = new MySharedItem { Id = 1, Name = "A" };
            var item2 = new MySharedItem { Id = 2, Name = "B" };
            var sharedSource = new SharedSource<MySharedItem>().Load(new List<MySharedItem>
            {
               item1, item2
            });

            Assert.AreEqual(false, item1.IsCanActivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item1.POnNavigatedTo);
            Assert.AreEqual(false, item2.IsCanActivateInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item2.POnNavigatedTo);
        }

        [TestMethod]
        public void Initialization_With_Dictionary()
        {
            var item1 = new MySharedItem { Id = 1, Name = "A" };
            var item2 = new MySharedItem { Id = 2, Name = "B" };
            var sharedSource = new SharedSource<MySharedItem>().Load(new Dictionary<MySharedItem, object>
            {
               { item1, 1 },{ item2, 2 }
            });

            Assert.AreEqual(false, item1.IsCanActivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(1, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(1, item1.POnNavigatedTo);
            Assert.AreEqual(false, item2.IsCanActivateInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(2, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(2, item2.POnNavigatedTo);
        }

        [TestMethod]
        public async Task Insert_Item()
        {
            var collection = new SharedSourceItemCollection<MySharedItem>();
            var item1 = new MySharedItem { Id = 1, Name = "A" };

            item1.CanActivate = false;
            Assert.AreEqual(false, await collection.InsertAsync(0, item1, 1));
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item1.POnNavigatingTo);
            Assert.AreEqual(false, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item1.POnNavigatedTo);

            item1.CanActivate = true;
            Assert.AreEqual(true, await collection.InsertAsync(0, item1, 1));
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(1, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(1, item1.POnNavigatedTo);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(item1, collection[0]);
        }

        [TestMethod]
        public async Task Add_Item()
        {
            var collection = new SharedSourceItemCollection<MySharedItem>();
            var item1 = new MySharedItem { Id = 1, Name = "A" };

            item1.CanActivate = false;
            Assert.AreEqual(false, await collection.AddAsync(item1, 1));
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item1.POnNavigatingTo);
            Assert.AreEqual(false, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item1.POnNavigatedTo);

            item1.CanActivate = true;
            Assert.AreEqual(true, await collection.AddAsync(item1, 1));
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(1, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(1, item1.POnNavigatedTo);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(item1, collection[0]);
        }

        [TestMethod]
        public async Task FindSelectable()
        {
            var item1 = new MySelectableViewModel { Id = 2 };
            var sources = new List<object>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               item1,
               new MySelectableViewModel{ Id = 3 }
           };
            var sharedSource = new SharedSource<object>().Load(sources);
            Assert.AreEqual(5, sharedSource.Items.Count);
            Assert.AreEqual(0, sharedSource.SelectedIndex);

            Assert.AreEqual(false, await sharedSource.Items.AddAsync(new MySelectableViewModel { Id = 2 }, 2));

            Assert.AreEqual(3, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task AddNewItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync(2);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(2, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(2, item1.POnNavigatedTo);

            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);

            var item2 = await sharedSource.AddNewAsync(10);
            Assert.AreEqual(true, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(10, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(10, item2.POnNavigatedTo);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
        }

        [TestMethod]
        public async Task InsertNewItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.InsertNewAsync(0, 5);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(5, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(5, item1.POnNavigatedTo);

            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);

            var item2 = await sharedSource.InsertNewAsync(0, 10);
            Assert.AreEqual(true, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(10, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(10, item2.POnNavigatedTo);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items[0]);
            Assert.AreEqual(item1, sharedSource.Items[1]);
        }

        [TestMethod]
        public async Task MoveItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            item2.Reset();
            item3.Reset();

            // move item2 B (index 1) => to index 2 (last)
            sharedSource.Items.Move(1, 2);

            // A C B
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item3, sharedSource.Items[1]);
            Assert.AreEqual(item2, sharedSource.Items[2]);
            // select item moved item2
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);
            // do not invoke navigation methods
            Assert.AreEqual(false, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, item3.IsCanActivateInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingToInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatedToInvoked);
        }

        [TestMethod]
        public async Task SetItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            item2.Reset();
            item3.Reset();

            // replace item2 by item4
            var item4 = sharedSource.CreateNew();
            sharedSource.Items[1] = item4;

            // A C B
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item4, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            // select item updated
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item4, sharedSource.SelectedItem);
            // navigation methods are not invoked
            Assert.AreEqual(false, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(false, item3.IsCanActivateInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingToInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatedToInvoked);
        }

        [TestMethod]
        public async Task ReplaceItem_With_NavigationHelper()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // replace item2 by item4
            var item4 = sharedSource.CreateNew();
            bool isSetCurrentInvoked = false;
            var setCurrent = new Action(() =>
            {
                sharedSource.Items[1] = item4;
                isSetCurrentInvoked = true;
            });

            item2.Reset();
            item4.Reset();
            item2.CanDeactivate = false;
            Assert.AreEqual(false, await NavigationHelper.ReplaceAsync(item2, item4, "D", setCurrent));
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item4.IsCanActivateInvoked);
            Assert.AreEqual(null, item4.PCanActivate);
            Assert.AreEqual(false, item4.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item4.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item4.POnNavigatingTo);
            Assert.AreEqual(false, item4.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item4.POnNavigatedTo);

            item2.Reset();
            item4.Reset();
            item4.CanActivate = false;
            Assert.AreEqual(false, await NavigationHelper.ReplaceAsync(item2, item4, "D", setCurrent));
            Assert.AreEqual(false, isSetCurrentInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item4.IsCanActivateInvoked);
            Assert.AreEqual("D", item4.PCanActivate);
            Assert.AreEqual(false, item4.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item4.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item4.POnNavigatingTo);
            Assert.AreEqual(false, item4.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item4.POnNavigatedTo);

            // with parameter
            item2.Reset();
            item4.Reset();
            Assert.AreEqual(true, await NavigationHelper.ReplaceAsync(item2, item4, "D", setCurrent));
            Assert.AreEqual(true, isSetCurrentInvoked);
            // A C B
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item4, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            // select item updated
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item4, sharedSource.SelectedItem);
            // do not invoke navigation methods
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(true, item4.IsCanActivateInvoked);
            Assert.AreEqual("D", item4.PCanActivate);
            Assert.AreEqual(false, item4.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item4.IsOnNavigatingToInvoked);
            Assert.AreEqual("D", item4.POnNavigatingTo);
            Assert.AreEqual(true, item4.IsOnNavigatedToInvoked);
            Assert.AreEqual("D", item4.POnNavigatedTo);
        }

        [TestMethod]
        public async Task Remove_At()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync(2);
            var item2 = await sharedSource.AddNewAsync(10);
            var item3 = await sharedSource.AddNewAsync(100);

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);

            item1.CanDeactivate = false;
            Assert.AreEqual(false, await sharedSource.Items.RemoveAtAsync(0));
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);

            item1.CanDeactivate = true;
            Assert.AreEqual(true, await sharedSource.Items.RemoveAtAsync(0));
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items[0]);
            Assert.AreEqual(item3, sharedSource.Items[1]);
            Assert.AreEqual(item2, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task Remove()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync(2);
            var item2 = await sharedSource.AddNewAsync(10);
            var item3 = await sharedSource.AddNewAsync(100);

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);

            item1.CanDeactivate = false;
            Assert.AreEqual(false, await sharedSource.Items.RemoveAsync(item1));
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);

            item1.CanDeactivate = true;
            Assert.AreEqual(true, await sharedSource.Items.RemoveAsync(item1));
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items[0]);
            Assert.AreEqual(item3, sharedSource.Items[1]);
            Assert.AreEqual(item2, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task SelectedIndex_After_Insertion()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var selections = new Dictionary<int, object>();
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                // 0 A => 1 B => 2 C
                selections.Add(e.SelectedIndex, e.SelectedItem);
            };

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            Assert.AreEqual(0, selections.ElementAt(0).Key);
            Assert.AreEqual(item1, selections.ElementAt(0).Value);
            Assert.AreEqual(1, selections.ElementAt(1).Key);
            Assert.AreEqual(item2, selections.ElementAt(1).Value);
            Assert.AreEqual(2, selections.ElementAt(2).Key);
            Assert.AreEqual(item3, selections.ElementAt(2).Value);
            Assert.AreEqual(3, selections.Count);
        }

        [TestMethod]
        public async Task Selection_With_SelectionHandling()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            sharedSource.SelectionHandling = SelectionHandling.None;

            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);

            sharedSource.SelectionHandling = SelectionHandling.Select;
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task SelectedIndex_After_Deletion()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // remove last
            Assert.AreEqual(true, await sharedSource.Items.RemoveAtAsync(2));
            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);

            // remove first
            Assert.AreEqual(true, await sharedSource.Items.RemoveAtAsync(0));
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items[0]);
            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);

            // 
            Assert.AreEqual(true, await sharedSource.Items.RemoveAtAsync(0));
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task ClearAsync()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // C => B => A
            var selections = new Dictionary<int, object>();
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                // 1 B ... 0 A
                selections.Add(e.SelectedIndex, e.SelectedItem);
            };

            Assert.AreEqual(true, await sharedSource.Items.ClearAsync());
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item3.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);
            // item3 removed (old index 2) ... select item2 (index 1)
            Assert.AreEqual(1, selections.ElementAt(0).Key);
            Assert.AreEqual(item2, selections.ElementAt(0).Value);
            // item1 removed change index, select item1 (index 0)
            Assert.AreEqual(0, selections.ElementAt(1).Key);
            Assert.AreEqual(item1, selections.ElementAt(1).Value);
            // empty items
            Assert.AreEqual(-1, selections.ElementAt(2).Key);
            Assert.AreEqual(null, selections.ElementAt(2).Value);
            Assert.AreEqual(3, selections.Count);
        }


        [TestMethod]
        public async Task ClearAsync_With_One_Item_CannotDeactivate()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // C => B X => A
            var selections = new Dictionary<int, object>();
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                // 1 B ... 0 B 
                selections.Add(e.SelectedIndex, e.SelectedItem);
            };

            item2.CanDeactivate = false;

            Assert.AreEqual(false, await sharedSource.Items.ClearAsync());
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item3.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);
            // item3 removed (old index 2) ... select item2 (index 1 B)
            Assert.AreEqual(1, selections.ElementAt(0).Key);
            Assert.AreEqual(item2, selections.ElementAt(0).Value);
            // item1 removed change index, same seleected item item2
            Assert.AreEqual(0, selections.ElementAt(1).Key);
            Assert.AreEqual(item2, selections.ElementAt(1).Value);
            Assert.AreEqual(2, selections.Count);
        }

        //[TestMethod]
        //public async Task ClearWithoutCanDeactivate()
        //{
        //    var sharedSource = new SharedSource<MySharedItem>();
        //    Assert.AreEqual(0, sharedSource.Items.Count);
        //    Assert.AreEqual(-1, sharedSource.SelectedIndex);

        //    var item1 = await sharedSource.AddNewAsync("A");
        //    var item2 = await sharedSource.AddNewAsync("B");
        //    var item3 = await sharedSource.AddNewAsync("C");

        //    Assert.AreEqual(3, sharedSource.Items.Count);
        //    Assert.AreEqual(item1, sharedSource.Items[0]);
        //    Assert.AreEqual(item2, sharedSource.Items[1]);
        //    Assert.AreEqual(item3, sharedSource.Items[2]);
        //    Assert.AreEqual(2, sharedSource.SelectedIndex);
        //    Assert.AreEqual(item3, sharedSource.SelectedItem);

        //    sharedSource.Items.ClearWithoutCheckingCanDeactivate();
        //    Assert.AreEqual(false, item1.IsCanDeactivateInvoked);
        //    Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item2.IsCanDeactivateInvoked);
        //    Assert.AreEqual(true, item2.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item3.IsCanDeactivateInvoked);
        //    Assert.AreEqual(true, item3.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(0, sharedSource.Items.Count);
        //    Assert.AreEqual(-1, sharedSource.SelectedIndex);
        //    Assert.AreEqual(null, sharedSource.SelectedItem);
        //}

        [TestMethod]
        public async Task ClearFast()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items[0]);
            Assert.AreEqual(item2, sharedSource.Items[1]);
            Assert.AreEqual(item3, sharedSource.Items[2]);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            sharedSource.Items.ClearFast();
            Assert.AreEqual(false, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item3.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);
        }

        [TestMethod]
        public async Task Call_OneTime_SelectionChanged_On_Insertion()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            int count = 0;
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                count++;
            };

            Assert.AreEqual(0, count);

            var item1 = await sharedSource.AddNewAsync("A");
            Assert.AreEqual(1, count);

            var item2 = await sharedSource.AddNewAsync("B");
            Assert.AreEqual(2, count);

            var item3 = await sharedSource.AddNewAsync("C");
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public async Task Call_OneTime_SelectionChanged_On_SelectedItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = await sharedSource.AddNewAsync("A");
            var item2 = await sharedSource.AddNewAsync("B");
            var item3 = await sharedSource.AddNewAsync("C");

            int count = 0;
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                count++;
            };
            Assert.AreEqual(0, count);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            sharedSource.SelectedIndex = 0;
            Assert.AreEqual(1, count);
            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);

            sharedSource.SelectedIndex = 1;
            Assert.AreEqual(2, count);
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);

            sharedSource.SelectedItem = item3;
            Assert.AreEqual(3, count);
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);
        }

    }

    public class MySharedItem : INavigationAware, ICanActivate, ICanDeactivate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool CanActivate { get; set; }
        public bool CanDeactivate { get; set; }
        public bool IsCanActivateInvoked { get; private set; }
        public object PCanActivate { get; set; }
        public bool IsOnNavigatedToInvoked { get; private set; }
        public object POnNavigatedTo { get; private set; }
        public bool IsOnNavigatingFromInvoked { get; private set; }
        public bool IsOnNavigatingToInvoked { get; private set; }
        public object POnNavigatingTo { get; private set; }
        public bool IsCanDeactivateInvoked { get; private set; }

        public MySharedItem()
        {
            CanActivate = true;
            CanDeactivate = true;
        }

        public void Reset()
        {
            CanActivate = true;
            CanDeactivate = true;
            IsCanActivateInvoked = false;
            IsCanDeactivateInvoked = false;
            PCanActivate = null;
            IsOnNavigatedToInvoked = false;
            POnNavigatedTo = null;
            IsOnNavigatingFromInvoked = false;
            IsOnNavigatingToInvoked = false;
            POnNavigatingTo = null;
            IsCanActivateInvoked = false;
            PCanActivate = null;
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            IsCanActivateInvoked = true;
            PCanActivate = parameter;
            return Task.FromResult(CanActivate);
        }

        public Task<bool> CanDeactivateAsync()
        {
            IsCanDeactivateInvoked = true;
            return Task.FromResult(CanDeactivate);
        }

        public void OnNavigatedTo(object parameter)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = parameter;
        }

        public void OnNavigatingFrom()
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(object parameter)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = parameter;
        }
    }

    public class MySharedItem2 : MySharedItem
    {

    }
}
