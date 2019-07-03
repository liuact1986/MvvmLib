using MvvmLib.Logger;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// <see cref="NavigationSource"/> for <see cref="ContentControl"/>.
    /// </summary>
    public class ContentControlNavigationSource : NavigationSource
    {
        private readonly string sourceName;
        /// <summary>
        /// The navigation source name.
        /// </summary>
        public string SourceName
        {
            get { return sourceName; }
        }

        private ContentControl control;
        /// <summary>
        /// The ContentControl.
        /// </summary>
        public ContentControl Control
        {
            get { return control; }
        }

        /// <summary>
        /// Creates the <see cref="ContentControlNavigationSource"/>.
        /// </summary>
        /// <param name="sourceName">The navigation source name</param>
        /// <param name="control">The ContentControl</param>
        public ContentControlNavigationSource(string sourceName, ContentControl control)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            this.sourceName = sourceName;
            this.control = control;
            this.control.Unloaded += OnContentControlUnloaded;
        }

        private void OnContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.control.Unloaded -= OnContentControlUnloaded;

            if (!NavigationManager.RemoveNavigationSource(sourceName, this))
                Logger.Log($"Unable to unregister a ControlControlNavigationSource for the source name \"{SourceName}\" on control unloaded", Category.Debug, Priority.High);
        }

        /// <summary>
        /// Changes the content of the ContentControl.
        /// </summary>
        /// <param name="source">The new source</param>
        protected override void SetCurrent(object source)
        {
            base.SetCurrent(source);
            this.control.Content = source;
        }
    }
}
