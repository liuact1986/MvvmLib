using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class NavigationHistory : INavigationHistory
    {
        public Stack<NavigationEntry> BackStack { get; }
        public Stack<NavigationEntry> ForwardStack { get; }

        public NavigationEntry Root => this.BackStack.Count > 0 ? this.BackStack.LastOrDefault() : null;

        public NavigationEntry Previous => this.BackStack.Count > 0 ? this.BackStack.Peek() : null;

        public NavigationEntry Next => this.ForwardStack.Count > 0 ? this.ForwardStack.Peek() : null;

        public NavigationEntry Current { get; protected set; }

        public NavigationHistory()
        {
            this.BackStack = new Stack<NavigationEntry>();
            this.ForwardStack = new Stack<NavigationEntry>();
        }

        public void Clear()
        {
            this.ForwardStack.Clear();
            this.BackStack.Clear();
            this.Current = null;
        }

        public void Navigate(NavigationEntry entry)
        {
            if (this.Current != null)
            {
                this.BackStack.Push(this.Current);
            }

            this.Current = entry;

            this.ForwardStack.Clear();
        }

        public void NavigateToRoot()
        {
            this.Current = this.Root;
            this.BackStack.Clear();
            this.ForwardStack.Clear();
        }

        public NavigationEntry GoBack()
        {
            this.ForwardStack.Push(this.Current);
            this.Current = this.BackStack.Pop();
            return this.Current;
        }

        public NavigationEntry GoForward()
        {
            this.BackStack.Push(this.Current);
            this.Current = this.ForwardStack.Pop();
            return this.Current;
        }
    }
}
