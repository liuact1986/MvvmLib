using MvvmLib.Message;
using System;

namespace NavigationSample.Wpf.Events
{
    public class DeleteItemEvent : ParameterizedEvent<int>
    {

    }

    public class ChangeTitleEvent : ParameterizedEvent<string>
    {

    }

    public class NavigateEvent : ParameterizedEvent<NavigateEventArgs>
    {

    }

    public class NavigateEventArgs
    {
        public Type SourceType { get; set; }

        public object Parameter { get; set; }
    }
}
