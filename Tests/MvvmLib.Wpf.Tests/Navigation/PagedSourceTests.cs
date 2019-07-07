using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Navigation;

namespace MvvmLib.Wpf.Tests.Navigation
{
    [TestClass]
    public class PagedSourceTests
    {
        [TestMethod]
        public void Initialized_With_No_PageSize_Returns_All_Items_And_One_Page_With_LIST()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
        }

        [TestMethod]
        public void Initializing_With_Pagesize_smaller_than_1_Throw()
        {
            var list = new List<string> { "A", "B", "C" };

            bool failed = false;
            try
            {
                var pagedSource = new PagedSource(list, 0);
            }
            catch (Exception ex)
            {
                failed = true;
            }

            Assert.AreEqual(true, failed);
        }

        [TestMethod]
        public void Initialized_With_PageSize_Returns_All_Items_And_One_Page_With_LIST()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
        }

        [TestMethod]
        public void Initialized_With_PageSize_2_Returns_All_Items_And_One_Page_With_LIST()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 2);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
        }

        [TestMethod]
        public void MoveToNextPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual(2, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual("B", pagedSource[0]);

            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageIndex);
            Assert.AreEqual(3, pagedSource.CurrentPage);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(3, pagedSource.End);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
        }

        [TestMethod]
        public void MoveToNextPage_With_PageSize_2()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 2);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(3, pagedSource.End);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
        }

        [TestMethod]
        public void MoveToPreviousPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // B
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // C
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToPreviousPage()); // B
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual(2, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual("B", pagedSource[0]);

            Assert.AreEqual(true, pagedSource.MoveToPreviousPage()); // A
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
        }

        [TestMethod]
        public void MoveToPreviousPage_With_PageSize_2()
        {
            var list = new List<string> { "A", "B", "C", "D", "E" };
            var pagedSource = new PagedSource(list, 2);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // C D
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual("D", pagedSource[1]);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(4, pagedSource.End);

            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // E
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageIndex);
            Assert.AreEqual(3, pagedSource.CurrentPage);
            Assert.AreEqual("E", pagedSource[0]);
            Assert.AreEqual(5, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);

            // back
            Assert.AreEqual(true, pagedSource.MoveToPreviousPage()); // C D
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual("D", pagedSource[1]);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(4, pagedSource.End);

            Assert.AreEqual(true, pagedSource.MoveToPreviousPage()); // A B
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
        }

        [TestMethod]
        public void MoveToFirstPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // B
            Assert.AreEqual(true, pagedSource.MoveToNextPage()); // C
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToFirstPage()); // A
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
        }

        [TestMethod]
        public void MoveToLastPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(true, pagedSource.MoveToLastPage()); // C
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageIndex);
            Assert.AreEqual(3, pagedSource.CurrentPage);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(3, pagedSource.End);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
        }

        [TestMethod]
        public void SetCurrent_ToFirstItem_On_MoveToPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);

            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("B", pagedSource.CurrentItem);

            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("C", pagedSource.CurrentItem);
        }

        [TestMethod]
        public void SetCurrent_ToFirstItem_On_MoveToPage_With_PageSize_2()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 2);

            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);

            Assert.AreEqual(true, pagedSource.MoveToNextPage());
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("C", pagedSource.CurrentItem);
        }

        [TestMethod]
        public void SetCurrent_With_Empty_List()
        {
            var list = new List<string>();
            var pagedSource = new PagedSource(list, 1);
            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(0, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(0, pagedSource.Start);
            Assert.AreEqual(0, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(-1, pagedSource.CurrentPosition);
            Assert.AreEqual(null, pagedSource.CurrentItem);
        }


        [TestMethod]
        public void MoveToPage_With_PageSize_1()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(false, pagedSource.MoveToPage(-1));
            Assert.AreEqual(false, pagedSource.MoveToPage(3));
            Assert.AreEqual(true, pagedSource.MoveToPage(2)); // C
            Assert.AreEqual(true, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageIndex);
            Assert.AreEqual(3, pagedSource.CurrentPage);
            Assert.AreEqual(3, pagedSource.Start);
            Assert.AreEqual(3, pagedSource.End);
            Assert.AreEqual("C", pagedSource[0]);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
        }

        [TestMethod]
        public void MoveToPage_With_Empty_List()
        {
            var list = new List<string>();
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);

            Assert.AreEqual(false, pagedSource.MoveToPage(-1));
            Assert.AreEqual(false, pagedSource.MoveToPage(1));
            Assert.AreEqual(true, pagedSource.MoveToPage(0));

            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(0, pagedSource.Start);
            Assert.AreEqual(0, pagedSource.End);
            Assert.AreEqual(-1, pagedSource.CurrentPosition);
            Assert.AreEqual(null, pagedSource.CurrentItem);
        }

        [TestMethod]
        public void Refresh_With_List()
        {
            var list = new List<string>();
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.TotalCount);

            list.Add("A");

            Assert.AreEqual("A", ((IList)pagedSource.SourceCollection)[0]);
            Assert.AreEqual(0, pagedSource.TotalCount);
            Assert.AreEqual(0, pagedSource.ItemCount);

            pagedSource.Refresh();
            Assert.AreEqual(1, pagedSource.TotalCount);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.PageCount);

            list.Add("B");
            pagedSource.Refresh();
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(2, pagedSource.PageCount);
        }

        [TestMethod]
        public void Refresh_Is_Auto_With_ObservableCollection()
        {
            var list = new ObservableCollection<string>();
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.TotalCount);

            list.Add("A");

            Assert.AreEqual("A", ((IList)pagedSource.SourceCollection)[0]);
            Assert.AreEqual(1, pagedSource.TotalCount);
            Assert.AreEqual("A", pagedSource[0]);

            list.Add("B");
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(2, pagedSource.PageCount);
        }

        [TestMethod]
        public void Refresh_On_PageSize_Changed()
        {
            var list = new ObservableCollection<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list, 1);

            Assert.AreEqual(1, pagedSource.PageSize);
            Assert.AreEqual(3, pagedSource.TotalCount);
            Assert.AreEqual(3, pagedSource.PageCount);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(1, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);

            pagedSource.PageSize = 2;

            Assert.AreEqual(2, pagedSource.PageSize);
            Assert.AreEqual(3, pagedSource.TotalCount);
            Assert.AreEqual(2, pagedSource.PageCount);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(true, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
        }

        [TestMethod]
        public void Apply_Filter()
        {
            var list = new List<int> { 10, 20, 30, 40, 50 };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(20, pagedSource[1]);
            Assert.AreEqual(30, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(50, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(10, pagedSource.CurrentItem);

            pagedSource.Filter = new Predicate<object>(p => (int)p > 30);
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(40, pagedSource[0]);
            Assert.AreEqual(50, pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(40, pagedSource.CurrentItem);

            pagedSource.ClearFilter();
            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(20, pagedSource[1]);
            Assert.AreEqual(30, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(50, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(10, pagedSource.CurrentItem);
        }

        [TestMethod]
        public void Apply_Sort()
        {
            var list = new List<int> { 20, 10, 50, 40, 30 };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(20, pagedSource[0]);
            Assert.AreEqual(10, pagedSource[1]);
            Assert.AreEqual(50, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(30, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);

            pagedSource.CustomSort = new IntSorter();
            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(20, pagedSource[1]);
            Assert.AreEqual(30, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(50, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(10, pagedSource.CurrentItem);

            pagedSource.CustomSort = null;
            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(20, pagedSource[0]);
            Assert.AreEqual(10, pagedSource[1]);
            Assert.AreEqual(50, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(30, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);
        }

        [TestMethod]
        public void Apply_Sort_With_Sort_Descriptions()
        {
            var list = new List<MySortItem>
            {
                new MySortItem { MyInt = 20, MyString = "C" },
                new MySortItem { MyInt = 40, MyString = "A" },
                new MySortItem { MyInt = 30, MyString = "A" },
                new MySortItem { MyInt = 10, MyString = "C" }
            };
            var pagedSource = new PagedSource(list);

            pagedSource.SortDescriptions.Add(new SortDescription("MyInt", ListSortDirection.Ascending));
            Assert.AreEqual(10, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual(20, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual(30, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual(40, ((MySortItem)pagedSource[3]).MyInt);

            pagedSource.SortDescriptions.Clear();
            Assert.AreEqual(20, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual(40, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual(30, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual(10, ((MySortItem)pagedSource[3]).MyInt);
        }

        [TestMethod]
        public void Apply_Sort_With_Sort_Descriptions_Descending()
        {
            var list = new List<MySortItem>
            {
                new MySortItem { MyInt = 20, MyString = "C" },
                new MySortItem { MyInt = 40, MyString = "A" },
                new MySortItem { MyInt = 30, MyString = "A" },
                new MySortItem { MyInt = 10, MyString = "C" }
            };
            var pagedSource = new PagedSource(list);

            pagedSource.SortDescriptions.Add(new SortDescription("MyInt", ListSortDirection.Descending));
            Assert.AreEqual(40, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual(30, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual(20, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual(10, ((MySortItem)pagedSource[3]).MyInt);
        }

        [TestMethod]
        public void Apply_Sort_With_Multiple_Sort_Descriptions()
        {
            var list = new List<MySortItem>
            {
                new MySortItem { MyInt = 20, MyString = "C" },
                new MySortItem { MyInt = 40, MyString = "A" },
                new MySortItem { MyInt = 30, MyString = "A" },
                new MySortItem { MyInt = 10, MyString = "C" }
            };
            var pagedSource = new PagedSource(list);

            pagedSource.SortDescriptions.Add(new SortDescription("MyString", ListSortDirection.Ascending));
            Assert.AreEqual("A", ((MySortItem)pagedSource[0]).MyString);
            Assert.AreEqual(40, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual("A", ((MySortItem)pagedSource[1]).MyString);
            Assert.AreEqual(30, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[2]).MyString);
            Assert.AreEqual(20, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[3]).MyString);
            Assert.AreEqual(10, ((MySortItem)pagedSource[3]).MyInt);

            pagedSource.SortDescriptions.Add(new SortDescription("MyInt", ListSortDirection.Ascending));
            Assert.AreEqual("A", ((MySortItem)pagedSource[0]).MyString);
            Assert.AreEqual(30, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual("A", ((MySortItem)pagedSource[1]).MyString);
            Assert.AreEqual(40, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[2]).MyString);
            Assert.AreEqual(10, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[3]).MyString);
            Assert.AreEqual(20, ((MySortItem)pagedSource[3]).MyInt);
        }

        [TestMethod]
        public void Apply_CustomSort_And_Ignore_Sort_Descriptions()
        {
            var list = new List<MySortItem>
            {
                new MySortItem { MyInt = 20, MyString = "C" },
                new MySortItem { MyInt = 40, MyString = "A" },
                new MySortItem { MyInt = 30, MyString = "A" },
                new MySortItem { MyInt = 10, MyString = "C" }
            };
            var pagedSource = new PagedSource(list);

            pagedSource.SortDescriptions.Add(new SortDescription("MyInt", ListSortDirection.Ascending));
            Assert.AreEqual(10, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual(20, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual(30, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual(40, ((MySortItem)pagedSource[3]).MyInt);

            pagedSource.CustomSort = new MySortItemSorter();
            Assert.AreEqual("A", ((MySortItem)pagedSource[0]).MyString);
            Assert.AreEqual(40, ((MySortItem)pagedSource[0]).MyInt);
            Assert.AreEqual("A", ((MySortItem)pagedSource[1]).MyString);
            Assert.AreEqual(30, ((MySortItem)pagedSource[1]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[2]).MyString);
            Assert.AreEqual(20, ((MySortItem)pagedSource[2]).MyInt);
            Assert.AreEqual("C", ((MySortItem)pagedSource[3]).MyString);
            Assert.AreEqual(10, ((MySortItem)pagedSource[3]).MyInt);
        }

        [TestMethod]
        public void Apply_Filter_And_Sorter()
        {
            var list = new List<int> { 20, 10, 50, 40, 30 };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(20, pagedSource[0]);
            Assert.AreEqual(10, pagedSource[1]);
            Assert.AreEqual(50, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(30, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);

            pagedSource.Filter = new Predicate<object>(p => (int)p > 30);
            pagedSource.CustomSort = new IntSorter();
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(40, pagedSource[0]);
            Assert.AreEqual(50, pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(40, pagedSource.CurrentItem);

            pagedSource.CustomSort = null;
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(2, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(50, pagedSource[0]);
            Assert.AreEqual(40, pagedSource[1]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(2, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(50, pagedSource.CurrentItem);

            pagedSource.ClearFilter();
            Assert.AreEqual(5, pagedSource.TotalCount);
            Assert.AreEqual(5, pagedSource.PageSize);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(5, pagedSource.ItemCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(20, pagedSource[0]);
            Assert.AreEqual(10, pagedSource[1]);
            Assert.AreEqual(50, pagedSource[2]);
            Assert.AreEqual(40, pagedSource[3]);
            Assert.AreEqual(30, pagedSource[4]);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(5, pagedSource.End);
            Assert.AreEqual(false, pagedSource.CanMoveToPreviousPage);
            Assert.AreEqual(false, pagedSource.CanMoveToNextPage);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);
        }

        [TestMethod]
        public void MoveCurrentToNext_With_Empty_List()
        {
            var list = new List<string>();
            var pagedSource = new PagedSource(list, 10);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(0, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(0, pagedSource.Start);
            Assert.AreEqual(-1, pagedSource.CurrentPosition);
            Assert.AreEqual(null, pagedSource.CurrentItem);
        }


        [TestMethod]
        public void MoveCurrentToNext()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext());
            Assert.AreEqual(1, pagedSource.CurrentPosition);
            Assert.AreEqual("B", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext());
            Assert.AreEqual(2, pagedSource.CurrentPosition);
            Assert.AreEqual("C", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
        }

        [TestMethod]
        public void MoveCurrentToPrevious()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext()); // B
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext()); // C
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToPrevious()); // B
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);
            Assert.AreEqual(1, pagedSource.CurrentPosition);
            Assert.AreEqual("B", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToPrevious()); // A
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);
        }

        [TestMethod]
        public void MoveCurrentToFirst()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext()); // B
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToNext()); // C
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToFirst()); // A
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);
        }

        [TestMethod]
        public void MoveCurrentToLast()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToLast()); // C
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
            Assert.AreEqual(2, pagedSource.CurrentPosition);
            Assert.AreEqual("C", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
        }


        [TestMethod]
        public void AddNewItem()
        {
            var list = new List<int> { 10 };
            var pagedSource = new PagedSource(list, 1);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(10, pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            pagedSource.AddNewItem(20);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(20, pagedSource.CurrentAddItem);
            Assert.AreEqual(20, pagedSource.CurrentItem);
            Assert.AreEqual(20, pagedSource[0]);
            //Assert.IsTrue(ReferenceEquals(pagedSource.CurrentAddItem, pagedSource.CurrentItem));
            //Assert.IsTrue(ReferenceEquals(list[0], pagedSource.CurrentItem));
            Assert.AreEqual(20, list[1]);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, pagedSource.TotalCount);
            Assert.AreEqual(1, pagedSource.ItemCount);
            Assert.AreEqual(2, pagedSource.PageCount);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPage);
            Assert.AreEqual(2, pagedSource.Start);
            Assert.AreEqual(20, pagedSource[0]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
        }

        [TestMethod]
        public void CancelNew()
        {
            var list = new List<int>();
            var pagedSource = new PagedSource(list, 1);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            pagedSource.AddNewItem(10);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(10, list[0]);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(0, pagedSource.CurrentPosition);

            pagedSource.CancelNew();
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(-1, pagedSource.CurrentPosition);
            Assert.AreEqual(null, pagedSource.CurrentItem);
            Assert.AreEqual(0, pagedSource.PageIndex);
        }

        [TestMethod]
        public void CancelNew_ChangePage()
        {
            var list = new List<int> { 10, 20 };
            var pagedSource = new PagedSource(list, 2);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            pagedSource.AddNewItem(30);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(30, pagedSource[0]);
            Assert.AreEqual(30, list[2]);
            Assert.AreEqual(3, list.Count);

            pagedSource.CancelNew();
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPosition);
        }

        [TestMethod]
        public void CancelNew_ChangePage_With_PageSize_1()
        {
            var list = new List<int> { 10, 20 };
            var pagedSource = new PagedSource(list, 1);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            pagedSource.AddNewItem(30);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(2, pagedSource.PageIndex);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual(30, pagedSource[0]);
            Assert.AreEqual(30, list[2]);
            Assert.AreEqual(3, list.Count);

            pagedSource.CancelNew();
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(1, pagedSource.PageIndex);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
        }

        [TestMethod]
        public void CommitNew()
        {
            var list = new List<int>();
            var pagedSource = new PagedSource(list, 10);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            pagedSource.AddNewItem(10);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(10, pagedSource[0]);
            Assert.AreEqual(10, list[0]);
            Assert.AreEqual(1, list.Count);

            pagedSource.CommitNew();
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void CancelNew_MoveToLastItem_Of_Page()
        {
            var list = new List<int> { 10, 20 };
            var pagedSource = new PagedSource(list, 3);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            pagedSource.AddNewItem(30);
            Assert.AreEqual(true, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(2, pagedSource.CurrentPosition);
            Assert.AreEqual(30, pagedSource[2]);
            Assert.AreEqual(30, list[2]);
            Assert.AreEqual(3, list.Count);

            pagedSource.CancelNew();
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPosition);
            Assert.AreEqual(20, pagedSource.CurrentItem);
        }

        [TestMethod]
        public void CancelEdit_With_IEditableObject()
        {
            var list = new List<MyItemEditable> { new MyItemEditable { MyString = "A" } };
            var pagedSource = new PagedSource(list, 1);
            Assert.AreEqual(true, pagedSource.CanAddNew);
            Assert.AreEqual(true, pagedSource.CanEditItem);

            var item = pagedSource[0] as MyItemEditable;
            pagedSource.EditItem(item);
            item.MyString = "A!";
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(true, pagedSource.IsEditingItem);

            Assert.AreEqual("A!", ((MyItemEditable)pagedSource.CurrentEditItem).MyString);
            Assert.AreEqual("A!", ((MyItemEditable)pagedSource.CurrentItem).MyString);

            pagedSource.CancelEdit();
            Assert.AreEqual("A", ((MyItemEditable)pagedSource.CurrentItem).MyString);
            Assert.AreEqual("A", item.MyString);
            Assert.AreEqual(false, pagedSource.IsAddingNew);
            Assert.AreEqual(false, pagedSource.IsEditingItem);
            Assert.AreEqual(null, pagedSource.CurrentEditItem);
        }

        [TestMethod]
        public void RemoveAt()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);


            pagedSource.RemoveAt(1);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual("C", list[1]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            pagedSource.RemoveAt(1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
        }

        [TestMethod]
        public void RemoveAt_On_Selection()
        {
            var list = new List<string> { "A", "B", "C" };
            var pagedSource = new PagedSource(list);

            Assert.AreEqual(list, pagedSource.SourceCollection);
            Assert.AreEqual(3, pagedSource.ItemCount);
            Assert.AreEqual(1, pagedSource.PageCount);
            Assert.AreEqual(0, pagedSource.PageIndex);
            Assert.AreEqual(1, pagedSource.CurrentPage);
            Assert.AreEqual(1, pagedSource.Start);
            Assert.AreEqual("A", pagedSource[0]);
            Assert.AreEqual("B", pagedSource[1]);
            Assert.AreEqual("C", pagedSource[2]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("A", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToNext);

            Assert.AreEqual(true, pagedSource.MoveCurrentToLast());
            Assert.AreEqual(2, pagedSource.CurrentPosition);
            Assert.AreEqual("C", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            pagedSource.RemoveAt(2); // remove last
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", list[0]);
            Assert.AreEqual("B", list[1]);
            Assert.AreEqual(1, pagedSource.CurrentPosition);
            Assert.AreEqual("B", pagedSource.CurrentItem);
            Assert.AreEqual(true, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            pagedSource.RemoveAt(0); // remove first
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("B", list[0]);
            Assert.AreEqual(0, pagedSource.CurrentPosition);
            Assert.AreEqual("B", pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);

            pagedSource.RemoveAt(0); // remove first
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(-1, pagedSource.CurrentPosition);
            Assert.AreEqual(null, pagedSource.CurrentItem);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToPrevious);
            Assert.AreEqual(false, pagedSource.CanMoveCurrentToNext);
        }

    }

    public class IntSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((int)x).CompareTo((int)y);
        }
    }

    public class MySortItemSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((MySortItem)x).MyString.CompareTo(((MySortItem)y).MyString);
        }
    }

    public class Item
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }

    public class MyItemEditable : IEditableObject
    {
        public string MyString { get; set; }

        private string clone;

        public void BeginEdit()
        {
            clone = MyString;
        }

        public void CancelEdit()
        {
            MyString = clone;
        }

        public void EndEdit()
        {
            clone = null;
        }
    }


    public class MySortItem
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }
}
