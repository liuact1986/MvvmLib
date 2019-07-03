using ModuleB.Services;
using ModuleSharedServices;
using MvvmLib.Mvvm;

namespace ModuleB.ViewModels
{
    public class ViewCViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewCViewModel(IMyService myService, IMySharedService mySharedService)
        {
            //this.Message = myService.GetMessage("ViewC [ModuleB]");
            this.Message = mySharedService.GetMessage("ViewC [ModuleB]");
        }
    }
}
