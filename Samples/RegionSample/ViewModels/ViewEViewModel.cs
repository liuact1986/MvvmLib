using RegionSample.Views;
using System;
using System.Windows;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{
    public class ViewEViewModel : BindableBase, INavigatable
    {
        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        public void OnNavigatingTo(object parameter)
        {
            Count++;
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {
            MessageBox.Show("OnNavigatingFrom (VIEWMODEL)", "ViewE");
        }
    }

   
}
