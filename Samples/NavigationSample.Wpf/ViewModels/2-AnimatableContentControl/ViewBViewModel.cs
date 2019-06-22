using MvvmLib.Mvvm;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewBViewModel : BindableBase
    {
        public Guid Guid { get; set; }


        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewBViewModel()
        {
            Message = "View B [ViewModel]";
            this.Guid = Guid.NewGuid();
        }
    }
}
