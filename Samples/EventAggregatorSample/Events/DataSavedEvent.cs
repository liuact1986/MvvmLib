using MvvmLib.Message;

namespace EventAggregatorSample.Events
{
    public class DataSavedEvent : ParameterizedEvent<DataSavedEventArgs>
    { }

    public class DataSavedEventArgs
    {
        public string Message { get; set; }
    }

}
