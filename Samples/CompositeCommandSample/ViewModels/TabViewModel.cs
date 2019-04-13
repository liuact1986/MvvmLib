using CompositeCommandSample.Common;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Windows;

namespace CompositeCommandSample.ViewModels
{

    public class TabViewModel : BindableBase, INavigatable
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string saveMessage;
        public string SaveMessage
        {
            get { return saveMessage; }
            set { SetProperty(ref saveMessage, value); }
        }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set
            {
                if (SetProperty(ref canSave, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public IRelayCommand SaveCommand { get; set; }

        public TabViewModel(IApplicationCommands applicationCommands)
        {
            canSave = true;
            SaveCommand = new RelayCommand(OnSave, CheckCanSave);

            applicationCommands.SaveAllCommand.Add(SaveCommand);
        }

        private void OnSave()
        {
            var message = $"Save TabView {Title}! {DateTime.Now.ToLongTimeString()}";
            MessageBox.Show(message);
            SaveMessage = message;
        }

        private bool CheckCanSave()
        {
            return canSave;
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
            {
                Title = (string)parameter;
            }
        }

        public void OnNavigatedTo(object parameter)
        {

        }
    }

}
