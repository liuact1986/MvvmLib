using System;
using System.Windows.Media.Animation;

namespace MvvmLib.Animation
{
    /// <summary>
    /// Allows to acces to an Storyboard.
    /// </summary>
    public class StoryboardAccessor
    {
        private Storyboard storyboard;
        /// <summary>
        /// The Storyboard.
        /// </summary>
        public Storyboard Storyboard
        {
            get { return storyboard; }
            set { storyboard = value; }
        }

        private Action onCompleted;
        /// <summary>
        /// The action invoked when the animation is completed.
        /// </summary>
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        /// <summary>
        /// Creates the <see cref="StoryboardAccessor"/>.
        /// </summary>
        /// <param name="storyboard">The storyboard</param>
        public StoryboardAccessor(Storyboard storyboard)
        {
            this.storyboard = storyboard ?? throw new ArgumentNullException(nameof(storyboard));
        }

        /// <summary>
        /// Handles the completed event of the Storyboard.
        /// </summary>
        /// <param name="onCompleted">The action invoked on complete</param>
        public void HandleCompleted(Action onCompleted)
        {
            this.onCompleted = onCompleted;
            this.storyboard.Completed += OnStoryboardCompleted;
        }

        /// <summary>
        /// Unhandles the completed event of the Storyboard.
        /// </summary>
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
