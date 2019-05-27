using MvvmLib.Message;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Events;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class SyncTitleViewModel : BindableBase, INavigatable
    {
        private readonly IEventAggregator eventAggregator;

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public SyncTitleViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void SetTitle()
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            eventAggregator.GetEvent<ChangeTitleEvent>().Publish(title);
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {

        }

        public void OnNavigatedTo(object parameter)
        {
            SetTitle();
        }
    }
}
