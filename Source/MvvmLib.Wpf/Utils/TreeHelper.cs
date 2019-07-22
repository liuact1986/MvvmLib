using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MvvmLib.Utils
{
    /// <summary>
    /// Helps to find parent and children with the <see cref="VisualTreeHelper"/>.
    /// </summary>
    public class TreeHelper
    {
        /// <summary>
        /// Find the first parent of the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="child">The child</param>
        /// <returns>The parent of the type or null</returns>
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                else
                    return FindParent<T>(parent);
            }
            else
                return null;
        }

        /// <summary>
        /// Gets a list of children of the type. Useful for example with an ItemsControl.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="parent">The parent</param>
        /// <returns>A list of children of type</returns>
        public static List<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            var childrenOfType = new List<T>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    if (child is T)
                    {
                        childrenOfType.Add((T)child);
                    }
                    else
                    {
                        childrenOfType.AddRange(FindChildrenOfType<T>(child));
                    }
                }
            }
            return childrenOfType;
        }

        /// <summary>
        /// Finds the child of the type at the index.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="parent">The parent</param>
        /// <param name="index">The index</param>
        /// <returns>The child found or null</returns>
        public static T FindChild<T>(DependencyObject parent, int index) where T : DependencyObject
        {
            var childrenOfType = FindChildrenOfType<T>(parent);
            if (childrenOfType.Count > index)
                return childrenOfType[index];

            return null;
        }

        /// <summary>
        /// Finds the first child.
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <returns>The first child found or null</returns>
        public static object FindFirstChild(DependencyObject parent)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if(childrenCount > 0)
            {
                var child = VisualTreeHelper.GetChild(parent, 0);
                return child;
            }
            return null;
        }

        /// <summary>
        /// Gets a list of children of the type. Useful for example with an ItemsControl.
        /// </summary>
        /// <param name="targetType">The type</param>
        /// <param name="parent">The parent</param>
        /// <returns>A list of children of type</returns>
        public static List<object> FindChildrenOfType(Type targetType, DependencyObject parent)
        {
            var childrenOfType = new List<object>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    if (child.GetType() == targetType)
                    {
                        childrenOfType.Add(child);
                    }
                    else
                    {
                        childrenOfType.AddRange(FindChildrenOfType(targetType, child));
                    }
                }
            }
            return childrenOfType;
        }

        /// <summary>
        /// Finds the child of the type at the index.
        /// </summary>
        /// <param name="targetType">The type</param>
        /// <param name="parent">The parent</param>
        /// <param name="index">The index</param>
        /// <returns>The child found or null</returns>
        public static object FindChild(Type targetType, DependencyObject parent, int index)
        {
            var childrenOfType = FindChildrenOfType(targetType, parent);
            if (childrenOfType.Count > index)
                return childrenOfType[index];

            return null;
        }
    }

}
