using EventAggregatorSample.Events;
using MvvmLib.Message;
using MvvmLib.Mvvm;
using System.Windows.Input;

namespace EventAggregatorSample.ViewModels
{
    public class DetailsViewModel
    {
        public ICommand SaveCommand { get; }

        public DetailsViewModel()
        {
            var ea = Singleton<EventAggregator>.Instance;

            SaveCommand = new RelayCommand(() => ea.GetEvent<DataSavedEvent>().Publish(new DataSavedEventArgs { Message = "Data saved." }));
        }
    }
}
