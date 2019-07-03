using MvvmLib.Mvvm;

namespace ModuleA.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewAViewModel()
        {
            this.Message = "ViewA [ModuleA]";
        }
    }
}
