using NavigationSample.Wpf.ViewModels;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class TransitioningContentControlSampleView : UserControl
    {
        public TransitioningContentControlSampleView()
        {
            InitializeComponent();

            TransitioningContentControl2.Content = new TransitoningControlViewModel();
        }

        private void OnReplayClick(object sender, System.Windows.RoutedEventArgs e)
        {
            TransitioningContentControl1.DoEnter();
            TransitioningContentControl2.DoEnter();
        }

        private void OnLeaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // TransitioningContentControl1.IsLeaving = true;
            TransitioningContentControl1.DoLeave();
            TransitioningContentControl2.DoLeave();
        }

        private void OnCancelClick(object sender, System.Windows.RoutedEventArgs e)
        {
            TransitioningContentControl1.CancelTransition();
            TransitioningContentControl2.CancelTransition();
        }

        private void OnResetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            TransitioningContentControl1.Reset();
            TransitioningContentControl2.Reset();
        }
    }
}
