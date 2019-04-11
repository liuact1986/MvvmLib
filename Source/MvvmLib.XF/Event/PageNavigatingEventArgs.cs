using System;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public class PageNavigatingEventArgs : EventArgs
    {
        public Page Page { get; }
        public PageNavigationType PageNavigationType { get; }

        public PageNavigatingEventArgs(Page page, PageNavigationType pageNavigationType)
        {
            Page = page;
            this.PageNavigationType = pageNavigationType;
        }
    }
}
