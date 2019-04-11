using System;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    public class PageNavigatedEventArgs : EventArgs
    {
        public Page Page { get; }
        public object Parameter { get; }
        public PageNavigationType PageNavigationType { get; }

        public PageNavigatedEventArgs(Page page, object parameter, PageNavigationType pageNavigationType)
        {
            Page = page;
            Parameter = parameter;
            this.PageNavigationType = pageNavigationType;
        }
    }
}
