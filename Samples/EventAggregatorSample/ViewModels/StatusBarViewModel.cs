using EventAggregatorSample.Events;
using MvvmLib.Message;
using MvvmLib.Mvvm;

namespace EventAggregatorSample.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        private string message = "Ready";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public StatusBarViewModel()
        {
            var ea = Singleton<EventAggregator>.Instance;
            ea.GetEvent<DataSavedEvent>().Subscribe(OnDataSaved);
        }

        private void OnDataSaved(DataSavedEventArgs args)
        {
            Message = args.Message;
        }
    }
}
