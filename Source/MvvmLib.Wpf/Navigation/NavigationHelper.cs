using System.Windows;

namespace MvvmLib.Navigation
{
    public class NavigationHelper
    {
        public static void OnNavigatingFromView(FrameworkElement currentView)
        {
            if (currentView is INavigatable p)
                p.OnNavigatingFrom();
        }

        public static void OnNavigatingFromContext(object currentContext)
        {
            if (currentContext is INavigatable p)
                p.OnNavigatingFrom();
        }

        public static void OnNavigatingToView(FrameworkElement view, object parameter)
        {
            if (view is INavigatable p)
                p.OnNavigatingTo(parameter);
        }

        public static void OnNavigatingToContext(object context, object parameter)
        {
            if (context is INavigatable p)
                p.OnNavigatingTo(parameter);
        }

        public static void OnNavigatedToView(FrameworkElement view, object parameter)
        {
            if (view is INavigatable p)
                p.OnNavigatedTo(parameter);
        }

        public static void OnNavigatedToContext(object context, object parameter)
        {
            if (context is INavigatable p)
                p.OnNavigatedTo(parameter);
        }

    }

}
