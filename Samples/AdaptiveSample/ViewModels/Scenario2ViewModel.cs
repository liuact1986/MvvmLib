using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmLib.Adaptive;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace AdaptiveSample.ViewModels
{
    public class Scenario2ViewModel : BindableBase, INavigatable
    {
        private Dictionary<string, object> active;
        public Dictionary<string, object> Active
        {
            get { return active; }
            set { SetProperty(ref this.active, value); }
        }

        private void OnActiveChanged(object sender, ActiveChangedEventArgs e)
        {
            this.Active = e.Active;
        }

        public  async void OnNavigatedTo(object parameter)
        {
            var binder = new BreakpointBinder();

            // with dictionary string : object
            binder.AddBreakpointWithBindings(0, new Dictionary<string, object> { { "TitleFontSize", "14" }, { "TitleColor", "Green" } });

            // with Adaptive container helper
            binder.AddBreakpointWithBindings(640, AdaptiveContainer.Create().Set("TitleFontSize", "80").Set("TitleColor", "Red").Get());
            binder.AddBreakpointWithBindings(960, AdaptiveContainer.Create().Set("TitleFontSize", "160").Set("TitleColor", "Blue").Get());

            // DEFERRED
             //await Task.Delay(4000);

            binder.ActiveChanged += OnActiveChanged;
        }

        public void OnNavigatingFrom()
        {
           
        }

        public void OnNavigatingTo(object parameter)
        {
            
        }
    }
}
