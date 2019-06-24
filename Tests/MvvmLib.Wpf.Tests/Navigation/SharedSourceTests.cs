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
        public void Initialization_With_Collection()
        {
            var item1 = new MySharedItem { Id = 1, Name = "A" };
            var item2 = new MySharedItem { Id = 2, Name = "B" };
            var sharedSource = new SharedSource<MySharedItem>().Load(new InitItemCollection<MySharedItem>
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
        public void Insert_Item()
        {
            var s = new SharedSource<MySharedItem>();
            var item1 = new MySharedItem { Id = 1, Name = "A" };

            item1.CActivate = false;
            s.Insert(0, item1, 1);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item1.POnNavigatingTo);
            Assert.AreEqual(false, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item1.POnNavigatedTo);

            item1.CActivate = true;
            s.Insert(0, item1, 1);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(1, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(1, item1.POnNavigatedTo);

            Assert.AreEqual(1, s.Items.Count);
            Assert.AreEqual(item1, s.Items.ElementAt(0));
        }

        [TestMethod]
        public void Add_Item()
        {
            var s = new SharedSource<MySharedItem>();
            var item1 = new MySharedItem { Id = 1, Name = "A" };

            item1.CActivate = false;
            s.Add(item1, 1);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(null, item1.POnNavigatingTo);
            Assert.AreEqual(false, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(null, item1.POnNavigatedTo);

            item1.CActivate = true;
            s.Add(item1, 1);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(1, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(1, item1.POnNavigatedTo);

            Assert.AreEqual(1, s.Items.Count);
            Assert.AreEqual(item1, s.Items.ElementAt(0));
        }

        [TestMethod]
        public void FindSelectable()
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

            sharedSource.Add(new MySelectableViewModel { Id = 2 }, 2);

            Assert.AreEqual(3, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void FindSelectable_With_Interface()
        {
            var item1 = new MySelectableViewModel { Id = 2 };
            var sources = new List<IMyViewModel>
           {
               new MyViewModelCanDeactivate(),
               new MySelectableViewModel{ Id = 1 },
               new MyViewModelCanActivate(),
               item1,
               new MySelectableViewModel{ Id = 3 }
           };
            var sharedSource = new SharedSource<IMyViewModel>().Load(sources);
            Assert.AreEqual(5, sharedSource.Items.Count);
            Assert.AreEqual(0, sharedSource.SelectedIndex);

            sharedSource.Add(new MySelectableViewModel { Id = 2 }, 2);

            Assert.AreEqual(3, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void AddNewItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew(2);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(2, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(2, item1.POnNavigatedTo);

            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));

            var item2 = sharedSource.AddNew(10);
            Assert.AreEqual(true, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(10, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(10, item2.POnNavigatedTo);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
        }

        [TestMethod]
        public void InsertNewItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.InsertNew(0, 5);
            Assert.AreEqual(true, item1.IsCanActivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingToInvoked);
            Assert.AreEqual(5, item1.POnNavigatingTo);
            Assert.AreEqual(true, item1.IsOnNavigatedToInvoked);
            Assert.AreEqual(5, item1.POnNavigatedTo);

            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item1, sharedSource.SelectedItem);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));

            var item2 = sharedSource.InsertNew(0, 10);
            Assert.AreEqual(true, item2.IsCanActivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingToInvoked);
            Assert.AreEqual(10, item2.POnNavigatingTo);
            Assert.AreEqual(true, item2.IsOnNavigatedToInvoked);
            Assert.AreEqual(10, item2.POnNavigatedTo);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(1));
        }

        [TestMethod]
        public void MoveItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            item2.Reset();
            item3.Reset();

            // move item2 B(index 1) => to index 2(last)
            sharedSource.Move(1, 2);

            // A C B
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(2));
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

        //[TestMethod]
        //public void SetItem()
        //{
        //    var sharedSource = new SharedSource<MySharedItem>();
        //    Assert.AreEqual(0, sharedSource.Items.Count);
        //    Assert.AreEqual(-1, sharedSource.SelectedIndex);

        //    var item1 = sharedSource.AddNew("A");
        //    var item2 = sharedSource.AddNew("B");
        //    var item3 = sharedSource.AddNew("C");

        //    Assert.AreEqual(3, sharedSource.Items.Count);
        //    Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
        //    Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
        //    Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
        //    Assert.AreEqual(2, sharedSource.SelectedIndex);
        //    Assert.AreEqual(item3, sharedSource.SelectedItem);

        //    item2.Reset();
        //    item3.Reset();

        //   // replace item2 by item4
        //    var item4 = sharedSource.CreateNew();
        //    sharedSource.SelectedItem = item4;

        //    // A C B
        //    Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
        //    Assert.AreEqual(item4, sharedSource.Items.ElementAt(1));
        //    Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
        //    // select item updated
        //    Assert.AreEqual(1, sharedSource.SelectedIndex);
        //    Assert.AreEqual(item4, sharedSource.SelectedItem);
        //    // navigation methods are not invoked
        //    Assert.AreEqual(false, item2.IsCanActivateInvoked);
        //    Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item2.IsOnNavigatingToInvoked);
        //    Assert.AreEqual(false, item2.IsOnNavigatedToInvoked);
        //    Assert.AreEqual(false, item3.IsCanActivateInvoked);
        //    Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item3.IsOnNavigatingFromInvoked);
        //    Assert.AreEqual(false, item3.IsOnNavigatingToInvoked);
        //    Assert.AreEqual(false, item3.IsOnNavigatedToInvoked);
        //}

        [TestMethod]
        public void ReplaceItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // replace item2 by item4
            var item4 = sharedSource.CreateNew();

            item2.Reset();
            item4.Reset();
            item2.CDeactivate = false;
            sharedSource.Replace(1, item4, "D");
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
            item4.CActivate = false;
            sharedSource.Replace(1, item4, "D");
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
            sharedSource.Replace(1, item4, "D");
            //  A C B
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item4, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
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
        public void Remove_At()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            item1.CDeactivate = false;
            sharedSource.RemoveAt(0);
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);

            item1.CDeactivate = true;
            sharedSource.RemoveAt(0);
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void Remove()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            item1.CDeactivate = false;
            sharedSource.Remove(item1);
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item1.IsOnNavigatingFromInvoked);

            item1.CDeactivate = true;
            sharedSource.Remove(item1);
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void SelectedIndex_After_Insertion()
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

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
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
        public void Selection_With_SelectionHandling()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            sharedSource.SelectionHandling = SelectionHandling.None;

            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");

            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);

            sharedSource.SelectionHandling = SelectionHandling.Select;
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void SelectedIndex_After_Deletion()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            // remove last
            sharedSource.RemoveAt(2);
            Assert.AreEqual(2, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(1, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);

            // remove first
            sharedSource.RemoveAt(0);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);


            sharedSource.RemoveAt(0);
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);
        }

        [TestMethod]
        public void ClearAsync()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            //  C => B => A
            var selections = new Dictionary<int, object>();
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                //  1 B... 0 A
                selections.Add(e.SelectedIndex, e.SelectedItem);
            };

            sharedSource.Clear();
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item3.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);
            Assert.AreEqual(null, sharedSource.SelectedItem);
            //   item3 removed(old index 2) ... select item2(index 1)
            Assert.AreEqual(1, selections.ElementAt(0).Key);
            Assert.AreEqual(item2, selections.ElementAt(0).Value);
            // item1 removed change index, select item1(index 0)
            Assert.AreEqual(0, selections.ElementAt(1).Key);
            Assert.AreEqual(item1, selections.ElementAt(1).Value);
            // empty items
            Assert.AreEqual(-1, selections.ElementAt(2).Key);
            Assert.AreEqual(null, selections.ElementAt(2).Value);
            Assert.AreEqual(3, selections.Count);
        }


        [TestMethod]
        public void ClearAsync_With_One_Item_CannotDeactivate()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            //  C => B X => A
            var selections = new Dictionary<int, object>();
            sharedSource.SelectedItemChanged += (s, e) =>
            {
                //  1 B... 0 B
                selections.Add(e.SelectedIndex, e.SelectedItem);
            };

            item2.CDeactivate = false;

            sharedSource.Clear();
            Assert.AreEqual(true, item1.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item1.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item2.IsCanDeactivateInvoked);
            Assert.AreEqual(false, item2.IsOnNavigatingFromInvoked);
            Assert.AreEqual(true, item3.IsCanDeactivateInvoked);
            Assert.AreEqual(true, item3.IsOnNavigatingFromInvoked);
            Assert.AreEqual(1, sharedSource.Items.Count);
            Assert.AreEqual(0, sharedSource.SelectedIndex);
            Assert.AreEqual(item2, sharedSource.SelectedItem);
            // item3 removed(old index 2) ... select item2(index 1 B)
            Assert.AreEqual(1, selections.ElementAt(0).Key);
            Assert.AreEqual(item2, selections.ElementAt(0).Value);
            // item1 removed change index, same seleected item item2
            Assert.AreEqual(0, selections.ElementAt(1).Key);
            Assert.AreEqual(item2, selections.ElementAt(1).Value);
            Assert.AreEqual(2, selections.Count);
        }

        [TestMethod]
        public void ClearFast()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

            Assert.AreEqual(3, sharedSource.Items.Count);
            Assert.AreEqual(item1, sharedSource.Items.ElementAt(0));
            Assert.AreEqual(item2, sharedSource.Items.ElementAt(1));
            Assert.AreEqual(item3, sharedSource.Items.ElementAt(2));
            Assert.AreEqual(2, sharedSource.SelectedIndex);
            Assert.AreEqual(item3, sharedSource.SelectedItem);

            sharedSource.ClearFast();
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
        public void Call_OneTime_SelectionChanged_On_Insertion()
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

            var item1 = sharedSource.AddNew("A");
            Assert.AreEqual(1, count);

            var item2 = sharedSource.AddNew("B");
            Assert.AreEqual(2, count);

            var item3 = sharedSource.AddNew("C");
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void Call_OneTime_SelectionChanged_On_SelectedItem()
        {
            var sharedSource = new SharedSource<MySharedItem>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            var item1 = sharedSource.AddNew("A");
            var item2 = sharedSource.AddNew("B");
            var item3 = sharedSource.AddNew("C");

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


        [TestMethod]
        public void Updates_Parameter()
        {
            var sharedSource = new SharedSource<MyViewModelThatChangeParameter>();
            Assert.AreEqual(0, sharedSource.Items.Count);
            Assert.AreEqual(-1, sharedSource.SelectedIndex);

            sharedSource.AddNew("p");

            Assert.AreEqual("p-canactivateviewmodel--onavigatingtoviewmodel--navigatedtoviewmodel-", MyViewModelThatChangeParameter.Parameter);

            MyViewModelThatChangeParameter.Parameter = null;

            sharedSource.RemoveAt(0);

            Assert.AreEqual("p-canactivateviewmodel--onavigatingtoviewmodel--navigatedtoviewmodel--candeactivateviewmodel--onavigatingfromviewmodel-", MyViewModelThatChangeParameter.Parameter);
        }
    }

    public class MySharedItem : INavigationAware, ICanActivate, ICanDeactivate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool CActivate { get; set; }
        public bool CDeactivate { get; set; }
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
            CActivate = true;
            CDeactivate = true;
        }

        public void Reset()
        {
            CActivate = true;
            CDeactivate = true;
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

        public void CanActivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanActivateInvoked = true;
            PCanActivate = navigationContext.Parameter;
            c(CActivate);
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> c)
        {
            IsCanDeactivateInvoked = true;
            c(CDeactivate);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsOnNavigatedToInvoked = true;
            POnNavigatedTo = navigationContext.Parameter;
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {
            IsOnNavigatingFromInvoked = true;
        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            IsOnNavigatingToInvoked = true;
            POnNavigatingTo = navigationContext.Parameter;
        }
    }

    public class MySharedItem2 : MySharedItem
    {

    }
}
