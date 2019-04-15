﻿using RegionSample.Views;
using System;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{
    public class ViewBViewModel : BindableBase, INavigatable, ISelectable, IRegionKnowledge
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        private RelayCommand updateMessageCommand;
        public RelayCommand UpdateMessageCommand =>
            updateMessageCommand ?? (updateMessageCommand = new RelayCommand(ExecuteUpdateMessageCommand));

        public ViewBViewModel()
        {
            message = "Initial ViewB message";
            count = 0;
        }

        void ExecuteUpdateMessageCommand()
        {
            Message += "!";
        }

        public void OnNavigatingTo(object parameter)
        {
            Count++;
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public bool IsTarget(Type viewType, object parameter)
        {
            return viewType == typeof(ViewB);
        }

        public void OnNavigatingFrom()
        {
            
        }

        public void GetRegion(IRegion region)
        {
            
        }
    }
}