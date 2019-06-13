using NavigationSample.Wpf.Controls;
using System;

namespace NavigationSample.Wpf.Views
{
    public class MenuItem
    {
        private string tag;
        public string Tag
        {
            get { return tag; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private IconKind icon;
        public IconKind Icon
        {
            get { return icon; }
        }

        private Action action;
        public Action Action
        {
            get { return action; }
        }

        public MenuItem(string name, IconKind icon)
        {
            this.name = name;
            this.icon = icon;
        }

        public MenuItem(string tag, string name, IconKind icon, Action action)
        {
            this.tag = tag;
            this.name = name;
            this.icon = icon;
            this.action = action;
        }
    }
}
