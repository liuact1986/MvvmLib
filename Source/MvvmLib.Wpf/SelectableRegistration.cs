using System;

namespace MvvmLib.Navigation
{
    public class SelectableRegistration
    {
        private bool isView;
        /// <summary>
        /// Checks if selectable is view.
        /// </summary>
        public bool IsView
        {
            get { return isView; }
        }

        private readonly Type sourceType;
        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object viewOrObject;
        /// <summary>
        /// The view or object.
        /// </summary>
        public object Source
        {
            get { return viewOrObject; }
        }

        private object context;
        /// <summary>
        /// The context.
        /// </summary>
        public object Context
        {
            get { return context; }
        }

        public SelectableRegistration(bool isView, Type sourceType, object viewOrObject, object context)
        {
            //ViewA, instance viewA, context viewAViewModel or null => is view true
            //ViewAViewModel, instance viewAViewModel, null (viewAViewModel) => isView false
            this.isView = isView;
            this.sourceType = sourceType;
            this.viewOrObject = viewOrObject;
            this.context = context;
        }
    }
}
