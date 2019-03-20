using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class ContentControlRegionAdapter : ContentRegionAdapterBase<ContentControl>
    {
        public override object GetContent(ContentControl control)
        {
            return control.Content;
        }

        public override void OnGoBack(ContentControl control, object previousView)
        {
            control.Content = previousView;
        }

        public override void OnGoForward(ContentControl control, object nextView)
        {
            control.Content = nextView;
        }

        public override void OnNavigate(ContentControl control, object view)
        {
            control.Content = view;
        }

    }
}