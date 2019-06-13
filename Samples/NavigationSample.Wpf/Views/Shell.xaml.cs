using MvvmLib.Animation;
using System.Windows;
using System.Windows.Media.Animation;

namespace NavigationSample.Wpf.Views
{
    public partial class Shell : Window
    {
        private bool isPaneOpened;
        private bool isAnimating;

        public Shell()
        {
            isPaneOpened = true;
            isAnimating = false;

            InitializeComponent();
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var storyboardName = isPaneOpened ? "CloseMenu" : "OpenMenu";
            var storyboard = (Storyboard)HamburgerButton.FindResource(storyboardName);
            var storyboardAccessor = new StoryboardAccessor(storyboard);
            storyboardAccessor.HandleCompleted(() =>
            {
                storyboardAccessor.UnhandleCompleted();
                isAnimating = false;
                isPaneOpened = !isPaneOpened;
            });
            storyboard.Begin();
        }
    }
}
