using System;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    public class StoryboardAccessor
    {
        private Storyboard storyboard;
        public Storyboard Storyboard
        {
            get { return storyboard; }
            set { storyboard = value; }
        }

        private Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        public StoryboardAccessor(Storyboard storyboard)
        {
            this.storyboard = storyboard ?? throw new ArgumentNullException(nameof(storyboard));
        }

        public void HandleCompleted(Action onCompleted)
        {
            this.onCompleted = onCompleted;
            this.storyboard.Completed += OnStoryboardCompleted;
        }

        public void UnhandleCompleted()
        {
            this.storyboard.Completed -= OnStoryboardCompleted;
        }

        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            onCompleted?.Invoke();
        }
    }

}
