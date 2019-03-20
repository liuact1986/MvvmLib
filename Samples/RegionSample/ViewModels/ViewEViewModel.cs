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

        public void OnNavigatedTo(object parameter)
        {
            Count++;
        }

        public void OnNavigatingFrom()
        {
            MessageBox.Show("OnNavigatingFrom (VIEWMODEL)", "ViewE");
        }
    }

   
}
