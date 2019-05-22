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
}
