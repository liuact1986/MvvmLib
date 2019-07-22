using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvvmLib.Mvvm;

namespace MvvmLib.NETFwk.Tests
{
    [TestClass]
    public class ChangeTrackerTests
    {
        //[TestMethod]
        //public void Track_List_That_Change_With_Not_KeepAlive()
        //{
        //    var items = new List<string> { "A", "B" };

        //    var tracker = new ChangeTracker();
        //    tracker.TrackChanges(items);
        //    Assert.IsFalse(tracker.CheckChanges());

        //    // change
        //    items = null;
        //    GC.Collect();

        //    Assert.IsTrue(tracker.CheckChanges());
        //}
    }
}
