using System;

namespace MvvmLib.Animation
{
    public class NavigationQueueItem
    {
        private object content;
        public object Content
        {
            get { return content; }
            set { content = value; }
        }

        private Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        public NavigationQueueItem(object content, Action onCompleted)
        {
            Content = content;
            OnCompleted = onCompleted;
        }
    }

}
